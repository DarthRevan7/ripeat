using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnSetting : MonoBehaviour
{
    private FighterStats fighterStats;
    private MainEnemyAI mainEnemyAI;
    private CombatSystem combatSystem;
    private InputManager inputManager;
    

    public Collider rightCollider;              // Riferimento al collider "destro"
    public float moveSpeed = 5f;                 // Velocità di movimento verso destra
    public MonoBehaviour inputController;        // Script o componente che gestisce gli input
    public Animator enemyAnimator;               // Animator per gestire l'animazione di camminata
    public GameObject secondEnemyBar;            // Barra della vita del secondo nemico

    public int targetRotationY;
    public float targetPositionVectorX;
    public float targetPositionVectorY;
    public float targetPositionVectorZ;

   
    public bool isPausedEnemy = false; 

    private bool hasRotated = false;
    private static bool start = false;

    void Start()
    {
        fighterStats = GetComponent<FighterStats>();
        mainEnemyAI = GetComponent<MainEnemyAI>();  
        combatSystem = GetComponent<CombatSystem>();  
        inputManager = GetComponent<InputManager>();
        secondEnemyBar.SetActive(false); // Disabilità la barra della vita del secondo nemico all'inizio

        if (enemyAnimator == null)
            enemyAnimator = GetComponent<Animator>();
        if (isPausedEnemy)
        {
            inputController.enabled = false;
            mainEnemyAI.enabled = false;
        }
    }

    void Update()
    {
        
        if (fighterStats.vita <= 25 && !isPausedEnemy)
        {
            start = true;
            rightCollider.enabled = false;
            inputController.enabled = false;
            mainEnemyAI.enabled = false;

            StartCoroutine(MoveToTargetPosition());
        }
        if (start){
            ResumeMovement();
        }
    }

    private IEnumerator MoveToTargetPosition()
    {
        if (!hasRotated)
        {
            float direction = (targetRotationY - transform.eulerAngles.y);
            transform.Rotate(0, direction, 0);
            hasRotated = true;
        }
        // Imposta lo stato MOVING
        combatSystem.currentState = CombatSystem.CharacterState.MOVING;
        while (Vector3.Distance(transform.position, new Vector3(targetPositionVectorX, targetPositionVectorY, targetPositionVectorZ)) > 0.1f)
        {
            if (enemyAnimator != null)
                enemyAnimator.SetBool("Run", true);
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(targetPositionVectorX, targetPositionVectorY, targetPositionVectorZ), 
                moveSpeed * Time.deltaTime);
            yield return null;
        }
        if (enemyAnimator != null)
            enemyAnimator.SetBool("Run", false);
        rightCollider.enabled = true;
    }

    // Metodo per sbloccare il nemico "pausato" mediante un comando esterno
    public void ResumeMovement()
    {
        // In questo caso, annulla il flag che lo teneva in pausa e avvia il movimento
        if(isPausedEnemy)
        {
            secondEnemyBar.SetActive(true); // Abilita la barra della vita del secondo nemico
            inputController.enabled = true;
            mainEnemyAI.enabled = true;
        }
    }
}
