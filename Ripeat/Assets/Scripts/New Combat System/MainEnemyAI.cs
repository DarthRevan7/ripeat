using UnityEngine;
using System.Collections; // Necessario per le Coroutine se vuoi usare delay tra le azioni

namespace AI
{
    public class MainEnemyAI : MonoBehaviour
    {
        [Header("Setup")]
        [Tooltip("Tag del GameObject del player (es. 'Player')")]
        public string playerTag = "Player"; // Usiamo un tag per trovare il player
        
        [Tooltip("Distanza minima dal player per considerare un attacco.")]
        public float attackRange = 1.5f;
        
        [Tooltip("Velocità di movimento del nemico.")]
        public float moveSpeed = 2.0f;

        [Tooltip("Layer del player. Usato per filtrare l'overlap sphere.")]
        [SerializeField]
        private LayerMask playerLayer; // <-- AGGIUNGERE QUESTA

        [Tooltip("Raggio della sfera interna. Se il player è dentro o su questa soglia, il nemico si fermerà (soglia interna per isteresi).")]
        [SerializeField] // Parametro aggiunto per l'isteresi con overlap
        public float stopMovingDistance = 1.0f; // <-- AGGIUNGERE QUESTA

        [Header("Difficulty")]
        [Tooltip("Tempo minimo (in secondi) tra una decisione e l'altra dell'IA.")]
        [SerializeField]
        private float decisionResponseTime = 0.5f; 

        [Tooltip("Probabilità (da 0 a 1) di attaccare quando il player è a distanza di attacco.")]
        [SerializeField, Range(0f, 1f)]
        private float attackChance = 0.8f; 

        [Tooltip("Probabilità (da 0 a 1) di provare a bloccare quando il player sta attaccando.")]
        [SerializeField, Range(0f, 1f)]
        private float blockChance = 0.6f;

        // Riferimenti privati
        private GameObject playerGameObject;
        private CombatSystem playerCombatSystem;
        private CombatSystem enemyCombatSystem; 

        private float lastDecisionTime;
        public bool isScriptActive = true;

        

        void Awake()
        {
            // Trova il player all'inizio
            playerGameObject = GameObject.FindGameObjectWithTag(playerTag);

            if (playerGameObject == null)
            {
                Debug.LogError($"NewEnemyAI: Player GameObject with tag '{playerTag}' not found! Disabling AI.");
                enabled = false; // Disabilita lo script se il player non viene trovato
                return;
            }

            // Ottieni il CombatSystem del player
            playerCombatSystem = playerGameObject.GetComponent<CombatSystem>();
            if (playerCombatSystem == null)
            {
                Debug.LogError("NewEnemyAI: Player GameObject found, but CombatSystem component is missing! Disabling AI.");
                enabled = false;
                return;
            }

            // Ottieni il CombatSystem del nemico stesso
            enemyCombatSystem = GetComponent<CombatSystem>();
            if (enemyCombatSystem == null)
            {
                Debug.LogError("NewEnemyAI: Enemy GameObject found, but its own CombatSystem component is missing! Disabling AI.");
                enabled = false;
                return;
            }

            if (stopMovingDistance > attackRange) // <-- AGGIUNGERE QUESTE RIGHE
            {
                Debug.LogWarning("NewEnemyAI: stopMovingDistance should not be greater than attackRange. Adjusting stopMovingDistance to equal attackRange.");
                stopMovingDistance = attackRange;
            }


            // Inizia il tempo per la prima decisione
            lastDecisionTime = Time.time;
        }

        void Update()
        {
            if(!isScriptActive) {
                return;
            }

            // Se il nemico è morto, non fare nulla
            if (enemyCombatSystem.currentState == CombatSystem.CharacterState.DEAD || playerCombatSystem.CurrentState == CombatSystem.CharacterState.DEAD)
            {
                return;
            }

            //Punta verso il Player
            transform.LookAt(new Vector3(playerGameObject.transform.position.x, transform.position.y, playerGameObject.transform.position.z));

            // Controlla se è passato abbastanza tempo dall'ultima decisione
            if (Time.time - lastDecisionTime >= decisionResponseTime)
            {
                MakeDecision();
                lastDecisionTime = Time.time; // Resetta il timer
            }

            // Debug visuale delle sfere di range
            Debug.DrawLine(transform.position, transform.position + transform.forward * attackRange, Color.red);
            Debug.DrawLine(transform.position, transform.position + transform.forward * stopMovingDistance, Color.blue);
            
        }

        void MakeDecision()
        {
            // Ottieni lo stato attuale del player e la distanza
            CombatSystem.CharacterState playerState = playerCombatSystem.currentState;
            float distanceToPlayer = Vector3.Distance(transform.position, playerGameObject.transform.position);

            // Calcola la direzione verso il player per il movimento
            Vector3 directionToPlayer = (playerGameObject.transform.position - transform.position).normalized;
            directionToPlayer.y = 0; // Assumi movimento su un piano XZ
            if (directionToPlayer.magnitude > 0.01f)
            {
                directionToPlayer.Normalize();
            } else {
                directionToPlayer = Vector3.forward; // O Vector3.zero
            }

            // Determina se il player sta attualmente "minacciando" con un attacco o blocco nel raggio reazione del blocco
            bool playerThreateningBlock = (playerState == CombatSystem.CharacterState.KICK || playerState == CombatSystem.CharacterState.PUNCH || playerState == CombatSystem.CharacterState.BLOCK) &&
                                        distanceToPlayer <= attackRange * 1.2f; // Usiamo un range leggermente più ampio per reagire in difesa


            // --- Gestione Stato Blocco ---

            // Se l'IA è attualmente nello stato di BLOCCO
            if (enemyCombatSystem.currentState == CombatSystem.CharacterState.BLOCK)
            {
                // L'IA dovrebbe smettere di bloccare?
                // Sì, se il player NON sta più "minacciando" con un attacco nel raggio di blocco.
                if (!playerThreateningBlock)
                {
                    enemyCombatSystem.blockHeld = false; // Segnala al CombatSystem di smettere di "tenere" il blocco
                    // Il CombatSystem, in base alla tua logica, dovrebbe poi gestire la fine dell'animazione di blocco
                    // e la transizione allo stato IDLE quando blockHeld diventa false.
                    // L'IA prenderà una nuova decisione (Muovi/Attacca/Idle) nel *prossimo* ciclo di MakeDecision,
                    // trovandosi nello stato IDLE.
                }
                // Se playerThreateningBlock è ancora true, l'IA continua a rimanere nello stato di BLOCCO,
                // e blockHeld rimane true, mantenendo l'animazione di blocco ferma.
                return; // Decisione presa per questo ciclo: gestire lo stato di blocco. Non fare altre decisioni (Move/Attack/Idle).
            }
            // --- Fine Gestione Stato Blocco ---


            // --- Se l'IA NON è attualmente nello stato di BLOCCO, decidere se DEVE ENTRARE IN BLOCCO o fare altro ---

            // Se il player STA "minacciando" con un attacco/blocco nel raggio
            // E la probabilità di blocco casuale ha successo
            if (playerThreateningBlock && Random.value < blockChance)
            {
                // Decide di ENTRARE nello stato di BLOCCO
                enemyCombatSystem.currentState = CombatSystem.CharacterState.BLOCK;
                enemyCombatSystem.blockHeld = true; // Segnala al CombatSystem di iniziare a "tenere" il blocco
                enemyCombatSystem.MovementInput = Vector3.zero; // Smetti di muoverti quando blocchi
                return; // Decisione presa per questo ciclo: ENTRARE NEL BLOCCO.
            }
            // Se il player NON STA "minacciando" OPPURE la probabilità di blocco non è successa,
            // l'IA continua a decidere tra Muovi/Attacca/Idle.


            // --- Logica di Decisone: Muovi / Attacca / Idle (solo se non in/entrando in Blocco) ---

            // Assicurati che blockHeld sia falso se non stiamo nello stato di BLOCCO in questo ciclo.
            // Questo è importante per coprire transizioni da altri stati (es. fine attacco)
            // che non portano direttamente al blocco.
            enemyCombatSystem.blockHeld = false;

            // Logica di isteresi basata sulla distanza per Muovi/Attacca/Idle
            // Caso 1: Player lontano (distanza > attackRange) -> DECIDI DI MUOVERE VERSO DI LUI
            if (distanceToPlayer > attackRange)
            {
                enemyCombatSystem.currentState = CombatSystem.CharacterState.MOVING;
                enemyCombatSystem.MovementInput = directionToPlayer; // Usa la direzione CORRETTA
            }
            // Caso 2: Player molto vicino (distanza <= stopMovingDistance) -> DECIDI DI FERMARTI e Attacca/Idle
            // stopMovingDistance è la soglia INFERIORE per SMETTERE di muovere e iniziare a considerare attacco/idle.
            else if (distanceToPlayer <= stopMovingDistance)
            {
                enemyCombatSystem.MovementInput = Vector3.zero; // Azzera l'input di movimento

                // Ora decidi tra Attacco e IDLE
                if (Random.value < attackChance)
                {
                    // Scegli a caso tra calcio e pugno
                    if (Random.value < 0.5f)
                    {
                        enemyCombatSystem.currentState = CombatSystem.CharacterState.KICK;
                    }
                    else
                    {
                        enemyCombatSystem.currentState = CombatSystem.CharacterState.PUNCH;
                    }
                }
                else
                {
                    enemyCombatSystem.currentState = CombatSystem.CharacterState.IDLE;
                }
                // L'input di movimento è già stato azzerato sopra in questo blocco.
            }
            // Caso 3: Player a distanza intermedia (stopMovingDistance < distanza <= attackRange)
            // Questa è la banda di isteresi. L'IA NON CAMBIA la sua decisione principale di movimento/azione
            // presa nel ciclo di decisione precedente se è entrata nella banda.
            else // (distanceToPlayer > stopMovingDistance && distanceToPlayer <= attackRange)
            {
                // Se lo stato attuale NON è MOVING (cioè, eri IDLE, un Attacco è finito, BLOCK, ecc. prima di entrare nella banda)
                // assicurati che l'input sia zero.
                if (enemyCombatSystem.currentState != CombatSystem.CharacterState.MOVING)
                {
                    enemyCombatSystem.MovementInput = Vector3.zero;
                    // Lo stato (IDLE, o quello che era) si mantiene.
                }
                // Se currentState È MOVING (cioè sei entrato nella banda mentre stavi muovendo),
                // l'input è già impostato correttamente dal blocco (distanceToPlayer > attackRange) in un ciclo precedente
                // e verrà mantenuto finché non si scende <= stopMovingDistance.
            }

            // Non aggiungere un catch-all finale per MovementInput = zero identico a quello sopra,
            // perché la logica dentro l'else della banda di isteresi e l'impostazione
            // blockHeld=false all'inizio del blocco Move/Attack/Idle già gestiscono l'azzeramento necessario.
        }

        void MoveTowardsPlayer()
        {
            if (playerGameObject != null)
            {
                // Calcola la direzione verso il player
                Vector3 direction = (playerGameObject.transform.position - transform.position).normalized;

                // Ignora il movimento sull'asse Y per un gioco 2D o un picchiaduro su un piano
                direction.y = 0;
                // Normalizza di nuovo nel caso l'asse Y sia stato modificato
                direction.Normalize();


                // Muovi il nemico
                transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);

                // Opzionale: fai guardare il nemico verso il player
                if (direction.magnitude > 0.01f) // Evita di guardare in direzioni non definite
                {
                    // Per un gioco 3D: Quaternion lookRotation = Quaternion.LookRotation(direction);
                    // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed); // Aggiungi rotationSpeed

                    // Per un gioco 2D su un piano: Controlla solo lo scaling sull'asse X o Y per flip
                    if (direction.x > 0 && transform.localScale.x < 0)
                    {
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                    else if (direction.x < 0 && transform.localScale.x > 0)
                    {
                        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                }
            }
        }

        // Puoi aggiungere qui altre funzioni helper se necessario
        // Es. bool IsAttackIncoming(Player player) per una logica di blocco più sofisticata
    }
}