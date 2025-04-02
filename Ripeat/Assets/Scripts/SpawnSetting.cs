using UnityEngine;

public class SpawnSetting : MonoBehaviour
{
    private FighterStats fighterStats;
    private MainEnemyAI mainEnemyAI;
    private CombatSystem combatSystem;
    private InputManager inputManager;
    public Collider rightCollider;              // Riferimento al collider "destro"
    public float moveSpeed = 5f;                 // Velocità di movimento verso destra
    public MonoBehaviour inputController;        // Script o componente che gestisce gli input

    // Animator per gestire l'animazione di camminata
    public Animator enemyAnimator;
    // Posizione verso cui il nemico deve guardare (impostata dall'Inspector)
    public int targetPosition;

    private bool hasRotated = false;

    void Start()
    {
        fighterStats = GetComponent<FighterStats>();
        mainEnemyAI = GetComponent<MainEnemyAI>();  
        combatSystem = GetComponent<CombatSystem>();  
        inputManager = GetComponent<InputManager>();

        // Se l'animator non è assegnato, tenta di prenderlo dallo stesso GameObject
        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Quando la vita scende al di sotto di 25
        if (fighterStats.vita <= 25)
        {
            // Disattiva componenti non necessari
            rightCollider.enabled = false;
            inputController.enabled = false;
            mainEnemyAI.enabled = false;

            // Ruota il nemico una sola volta per puntare verso la posizione target
            if (!hasRotated)
            {
                float direction = (targetPosition - transform.eulerAngles.y);
                transform.Rotate(0,direction,0);
                hasRotated = true;
            }

            // Imposta lo stato MOVING e attiva l'animazione della camminata
            combatSystem.currentState = CombatSystem.CharacterState.MOVING;
            if (enemyAnimator != null)
                enemyAnimator.SetBool("Run", true);

            // Muove il nemico verso l'avanti (nella direzione locale, che ora punta verso targetPosition)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
