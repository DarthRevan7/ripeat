using UnityEngine;
using System.Collections;
using System.Linq; // Necessario per le Coroutine se vuoi usare delay tra le azioni

public class CustomizableAI : MonoBehaviour
{
    #region Variables
    [Header("Setup")]
    public string playerTag = "Player"; // Usiamo un tag per trovare il player
    public float attackRange = 1.5f;
    public float moveSpeed = 3.0f;
    public float stopMovingDistance = 1.0f;
    [Header("Difficulty")]
    [Tooltip("Tempo minimo (in secondi) tra una decisione e l'altra dell'IA.")]
    [SerializeField]
    private float decisionResponseTime = 0.5f;
    [Tooltip("Probabilità (da 0 a 1) di attaccare quando il player è a distanza di attacco.")]
    [SerializeField, Range(0f, 1f)]
    private float attackChance = 0.8f;
    private CharacterController characterController;

    [Tooltip("Probabilità (da 0 a 1) di provare a bloccare quando il player sta attaccando.")]
    [SerializeField, Range(0f, 1f)]
    private float blockChance = 0.6f;

    // Riferimenti privati
    private GameObject playerGameObject;
    private CombatAnimSystem playerCombatSystem;
    private CombatAnimSystem enemyCombatSystem;
    // private CharacterController characterController; // Aggiunto per il movimento

    private float lastDecisionTime;
    public bool isScriptActive = true, thinking = true, collided = false;

    [SerializeField] private CombatAnimSystem.CombatAnimState nextState;


    #endregion


    #region AI Methods

    private void EvaluateDecision()
    {
        CombatAnimSystem.CombatAnimState playerState = playerCombatSystem.CurrentState;
        float distanceToPlayer = Vector3.Distance(playerGameObject.transform.position, transform.position);

        // Se l'IA sta già eseguendo un'animazione di attacco/blocco (animState > 0),
        // oppure se sto correndo verso il player
        // non può cambiare la sua azione attuale. Resta nello stato corrente.
        if (enemyCombatSystem.GetAnimState() > 1 && enemyCombatSystem.CurrentState != CombatAnimSystem.CombatAnimState.MOVING)
        {
            nextState = enemyCombatSystem.CurrentState; // Mantiene lo stato attuale
            return; // L'IA è occupata, non prendere altre decisioni per ora.
        }

        // ----------------------- LOGICA DECISIONALE BASATA SULLA PROBABILITÀ E PRIORITÀ -----------------------

        // PRIORITÀ 1: Blocco (se il player sta attaccando)
        // L'IA tenta di bloccare indipendentemente dalla distanza se il player sta attaccando
        if (playerState == CombatAnimSystem.CombatAnimState.PUNCH || playerState == CombatAnimSystem.CombatAnimState.KICK)
        {
            if (Random.Range(0f, 1f) <= blockChance) // Controlla la probabilità di bloccare
            {
                nextState = CombatAnimSystem.CombatAnimState.BLOCK;
                // Debug.Log("IA: Decido di BLOCcare (probabilità superata)!");
                GetComponent<Animator>().SetBool("Blocking", true);
                StartCoroutine(EndBlock());
                return; // Decisione presa, esci.
            }
        }

        // PRIORITÀ 2: Attacco (se il player è a distanza)
        // L'IA attacca solo se è nel raggio di attacco e la probabilità lo permette.
        if (distanceToPlayer <= attackRange)
        {
            if (Random.Range(0f, 1f) <= attackChance) // Controlla la probabilità di attaccare
            {
                // Se decide di attaccare, sceglie casualmente tra PUNCH e KICK
                nextState = (Random.Range(0f, 1f) <= 0.5f) ? CombatAnimSystem.CombatAnimState.PUNCH : CombatAnimSystem.CombatAnimState.KICK;
                // Debug.Log($"IA: Decido di ATTACCARE: {nextState} (probabilità superata)!");
                return; // Decisione presa, esci.
            }
        }

        // PRIORITÀ 3: Movimento (se il player è troppo lontano)
        // Se non ha bloccato o attaccato, e il giocatore è lontano, l'IA si muove.
        if (distanceToPlayer > stopMovingDistance && !collided)
        {
            nextState = CombatAnimSystem.CombatAnimState.MOVING;
            // Debug.Log("IA: Decido di MUOVERMI!");
            return; // Decisione presa, esci.
        }

        // PRIORITÀ 4: IDLE (se nessuna delle condizioni precedenti è vera)
        // Se l'IA è vicina, il player non sta attaccando e l'IA non ha attaccato, allora sta ferma.
        nextState = CombatAnimSystem.CombatAnimState.IDLE;
        // Debug.Log("IA: Decido di stare IDLE.");
    }

    private void ExecuteDecision()
    {
        enemyCombatSystem.RequestStateChange(nextState);
        // Debug.Log($"Request State {nextState}\n");
    }

    private void HandleMovement()
    {
        float distanceToPlayer = Vector3.Distance(playerGameObject.transform.position, transform.position);

        // ---- INIZIO CONTROLLO DI SICUREZZA AGGIUNTO ----
        // Se siamo già più vicini (o uguali) alla distanza di stop,
        // diciamo all'IA di fermarsi (se si stava muovendo) e non eseguiamo il resto del codice.
        if (distanceToPlayer <= stopMovingDistance)
        {
            if (enemyCombatSystem.CurrentState == CombatAnimSystem.CombatAnimState.MOVING)
            {
                // Richiedi un cambio di stato a IDLE per fermare l'animazione di movimento.
                enemyCombatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.IDLE);
            }
            // Interrompe l'esecuzione del metodo qui, per non chiamare .Move() inutilmente.
            return;
        }
        // ---- FINE CONTROLLO DI SICUREZZA AGGIUNTO ----

        // Muovi solo se l'IA è nello stato MOVING e non sta eseguendo un'azione di attacco/blocco
        // (animState < 2)
        // if (enemyCombatSystem.CurrentState == CombatAnimSystem.CombatAnimState.MOVING && enemyCombatSystem.GetAnimState() < 2)
        if (enemyCombatSystem.CurrentState == CombatAnimSystem.CombatAnimState.MOVING)
        {
            // Calcola la direzione verso il player (solo sull'asse XZ)
            Vector3 directionToPlayer = playerGameObject.transform.position - transform.position;
            directionToPlayer.y = 0; // Ignora l'altezza
            directionToPlayer.Normalize(); // Normalizza per ottenere solo la direzione
            directionToPlayer.y = 0;

            // Muovi il personaggio usando il CharacterController
            //transform.Translate(directionToPlayer * moveSpeed * Time.deltaTime, Space.World);
            characterController.Move(directionToPlayer * moveSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region Utility Methods

    IEnumerator EndBlock()
    {
        thinking = false;
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetBool("Blocking", false);
        enemyCombatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.IDLE);
        thinking = true;
    }

    // Ritorna un'azione casuale NON DISTRUTTIVA per il combattimento tra le 4 possibili
    // (potrebbe non essere più necessaria con la logica di EvaluateDecision)
    private CombatAnimSystem.CombatAnimState RandomAction()
    {
        int action = Random.Range(0, 4);
        if (action == 0)
        {
            return CombatAnimSystem.CombatAnimState.IDLE;
        }
        else if (action == 1)
        {
            return CombatAnimSystem.CombatAnimState.PUNCH;
        }
        else if (action == 2)
        {
            return CombatAnimSystem.CombatAnimState.KICK;
        }
        else
        {
            return CombatAnimSystem.CombatAnimState.BLOCK;
        }
    }

    // Ritorna il counter perfetto per le azioni di combattimento del player
    // (potrebbe essere usata per una logica più avanzata di contrattacco)
    private CombatAnimSystem.CombatAnimState PerfectCounter(CombatAnimSystem.CombatAnimState state)
    {
        if (state == CombatAnimSystem.CombatAnimState.IDLE)
        {
            if (Random.Range(0f, 1f) <= 0.5f)
            {
                return CombatAnimSystem.CombatAnimState.PUNCH;
            }
            else
            {
                return CombatAnimSystem.CombatAnimState.KICK;
            }
        }
        if (state == CombatAnimSystem.CombatAnimState.PUNCH || state == CombatAnimSystem.CombatAnimState.KICK)
        {
            // In alternativa si può pensare di far scappare l'IA, in modo da evitare il danno.
            return CombatAnimSystem.CombatAnimState.BLOCK;
        }
        if (state == CombatAnimSystem.CombatAnimState.BLOCK)
        {
            if (Random.Range(0f, 1f) <= 0.5f)
            {
                return CombatAnimSystem.CombatAnimState.PUNCH;
            }
            else
            {
                return CombatAnimSystem.CombatAnimState.KICK;
            }
        }
        return CombatAnimSystem.CombatAnimState.IDLE;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == playerTag)
        {
            collided = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == playerTag)
        {
            collided = false;
        }
    }

    #endregion

    #region Awake&Update
    void Awake()
    {
        playerGameObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerGameObject == null)
        {
            // Debug.LogError($"CustomizableAI: Player GameObject with tag '{playerTag}' not found! Disabling AI.");
            enabled = false;
            return;
        }

        // Ottieni il CombatSystem del player
        playerCombatSystem = playerGameObject.GetComponent<CombatAnimSystem>();
        if (playerCombatSystem == null)
        {
            // Debug.LogError("CustomizableAI: Player GameObject found, but CombatSystem component is missing! Disabling AI.");
            enabled = false;
            return;
        }

        // Ottieni il CombatSystem del nemico stesso
        enemyCombatSystem = GetComponent<CombatAnimSystem>();
        if (enemyCombatSystem == null)
        {
            // Debug.LogError("CustomizableAI: Enemy GameObject found, but its own CombatSystem component is missing! Disabling AI.");
            enabled = false;
            return;
        }

         //Ottieni il CharacterController del nemico
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            //  Debug.LogError("CustomizableAI: CharacterController component is missing! Disabling AI.");
            enabled = false;
            return;
        }

        if (stopMovingDistance > attackRange)
        {
            // Debug.LogWarning("CustomizableAI: stopMovingDistance should not be greater than attackRange. Adjusting stopMovingDistance to equal attackRange.");
            stopMovingDistance = attackRange;
        }

        // Inizializza il tempo per la prima decisione
        lastDecisionTime = Time.time;
    }

    void Update()
    {
        if (!isScriptActive)
        {
            return;
        }

        if (!thinking)
        {
            return;
        }

        // Se il player è morto o se sei morto, non fare nulla
        if (enemyCombatSystem.CurrentState == CombatAnimSystem.CombatAnimState.DEAD)
        {
            return;
        }
        if (playerCombatSystem.CurrentState == CombatAnimSystem.CombatAnimState.DEAD)
        {
            enemyCombatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.IDLE);
            return;
        }

        // Punta verso il Player
        // transform.LookAt();

        // Gestione del movimento dell'IA
        HandleMovement();

        // Ciclo di decisione dell'IA
        if (Time.time - lastDecisionTime >= decisionResponseTime)
        {
            EvaluateDecision();
            lastDecisionTime = Time.time;
        }

        CombatAnimSystem.CombatAnimState oldState = enemyCombatSystem.CurrentState;
        if (enemyCombatSystem.StateChangeCheck() && oldState != nextState)
        {
            ExecuteDecision();
        }
    }
    #endregion
}