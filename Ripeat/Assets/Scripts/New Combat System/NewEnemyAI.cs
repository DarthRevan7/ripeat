using UnityEngine;
using System.Collections; // Necessario per le Coroutine se vuoi usare delay tra le azioni

public class NewEnemyAI : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Tag del GameObject del player (es. 'Player')")]
    public string playerTag = "Player"; // Usiamo un tag per trovare il player
    
    [Tooltip("Distanza minima dal player per considerare un attacco.")]
    public float attackRange = 1.5f;
    
    [Tooltip("Velocità di movimento del nemico.")]
    public float moveSpeed = 2.0f;

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

        // Inizia il tempo per la prima decisione
        lastDecisionTime = Time.time;
    }

    void Update()
    {

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
        
    }

    void MakeDecision()
    {
        // Ottieni lo stato attuale del player e la distanza
        CombatSystem.CharacterState playerState = playerCombatSystem.currentState;
        Vector3 playerDistance = transform.position - playerGameObject.transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, playerGameObject.transform.position);

        // Priorità 1: Il player sta attaccando? Prova a bloccare.
        if (playerState == CombatSystem.CharacterState.KICK || playerState == CombatSystem.CharacterState.PUNCH)
        {
            // Controlla se il player è abbastanza vicino per l'attacco e se la probabilità di blocco si verifica
            if (distanceToPlayer <= attackRange && Random.value < blockChance) // Moltiplica attackRange per dare un piccolo margine
            {
                enemyCombatSystem.currentState = CombatSystem.CharacterState.BLOCK;
                return; // Decisione presa, esci
            }
        }

        // Priorità 2: Gestisci movimento e attacco
        if (distanceToPlayer > attackRange)
        {
            // Il player è lontano, muovi verso di lui
            //enemyCombatSystem.currentState = CombatSystem.CharacterState.MOVING;
            //GetComponent<Animator>().SetBool("Run", true);
            // combatSystem.CurrentState = CombatSystem.CharacterState.MOVING;
            enemyCombatSystem.MovementInput = playerDistance.normalized;
        }
        else // Il player è a distanza di attacco o molto vicino
        {
            // Decidi se attaccare
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
                // Non attaccare questa volta, sta fermo (o gestisci leggero riposizionamento)
                 enemyCombatSystem.currentState = CombatSystem.CharacterState.IDLE;
                 // Potresti aggiungere qui logica per muoversi leggermente o arretrare
                 // if (Random.value < 0.3f) ChangeState(MOVING, direction = AwayFromPlayer)
            }
        }
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