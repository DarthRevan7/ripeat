using UnityEngine;
using System.Collections;

public class MainEnemyAI : MonoBehaviour
{

    //Riferimento al Player
    [SerializeField] private Transform playerTransform;
    //Raggio di attacco
    [SerializeField] private float attackRange = 2.5f;

    //Espandere per futuri sviluppi
    public enum EnemyStatus {
        //Status dell'IA del nemico
        FOLLOWING_PLAYER, ATTACK, IDLE
    }

    [SerializeField] private EnemyStatus enemyStatus = EnemyStatus.IDLE;

    //Gestione attacchi
    [SerializeField] private float pauseAttack = 2.0f;
    [SerializeField] private bool hasAttacked = false;

    //Sospensione dell'IA all'inizio della scena
    [SerializeField] private bool startFight = false;
    [SerializeField] private float secondsToStartFight = 1.5f;

    //Riferimento al CombatSystem dell'IA
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private FighterStats fighterStats;
    [SerializeField] private bool isRunning = false;
    public bool isScriptActive = true;



    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        isScriptActive = true;

        combatSystem = GetComponent<CombatSystem>();
        fighterStats = GetComponent<FighterStats>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(!isScriptActive) return;
        

        if(fighterStats.isDead || playerTransform.GetComponent<FighterStats>().isDead) return;

        if(!startFight) return;

        //Punta verso il Player
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));

        FollowAndAttack();

        if(enemyStatus == EnemyStatus.FOLLOWING_PLAYER)
        {
            MoveTowardsPlayer();
        }
        else if(enemyStatus == EnemyStatus.ATTACK)
        {
            AttackPlayer();
        }


    }
    
    public EnemyStatus GetEnemyStatus()
    {
        return enemyStatus;
    }

    public void HitTarget()
    {
        // playerTransform.GetComponent<CharacterStats>().HitTarget(damage);
    }

    public void WaitBeforeFight()
    {
        StartCoroutine(WaitBeforeFighting());
    }

    public IEnumerator WaitBeforeFighting()
    {
        yield return new WaitForSeconds(secondsToStartFight);
        startFight = true;
    }

    private void FollowAndAttack()
    {   
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if(enemyStatus != EnemyStatus.FOLLOWING_PLAYER && distance > attackRange)
        {
            enemyStatus = EnemyStatus.FOLLOWING_PLAYER;
            // GetComponent<Animator>().SetInteger("AttackType", 0);
            combatSystem.CurrentState = CombatSystem.CharacterState.MOVING;
            
        }
        else if(enemyStatus != EnemyStatus.ATTACK && distance <= attackRange)
        {
            enemyStatus = EnemyStatus.ATTACK;
        }
        if(distance > attackRange && enemyStatus==EnemyStatus.FOLLOWING_PLAYER)
        {
            Vector3 movement = transform.position - playerTransform.position;
            movement.Normalize();
            combatSystem.MovementInput = movement;
        }
        else
        {
            combatSystem.MovementInput = Vector3.zero;
        }
    }

    private void AttackPlayer()
    {
        
        // GetComponent<Animator>().SetBool("Run", false);
        combatSystem.CurrentState = CombatSystem.CharacterState.IDLE;
        if(Vector3.Distance(transform.position, playerTransform.position) <= attackRange && !hasAttacked && !isRunning)
        {
            // GetComponent<Animator>().SetInteger("AttackType", 1);
            StartCoroutine(AttackWaitingTime());
            isRunning = true;
            // hasAttacked = true;
        }
    }

    public void Hit()
    {
        hasAttacked = true;
    }

    public void ToggleMove()
    {

    }

    IEnumerator AttackWaitingTime()
    {
        //Setta l'animazione x l'attacco
        // GetComponent<Animator>().SetInteger("AttackType", 1);
        int attackType = Random.Range(1,3);
        if(attackType == 1)
        {
            combatSystem.CurrentState = CombatSystem.CharacterState.PUNCH;
        }
        else
        {
            combatSystem.CurrentState = CombatSystem.CharacterState.KICK;
        }
        //Aspetta che l'evento dell'animazione sia catturato dal metodo Hit
        yield return new WaitUntil( () => hasAttacked == true ); 
        //Disattiva animazione di attacco
        // combatSystem.CurrentState = CombatSystem.CharacterState.IDLE;
        //Aspetta il tempo di ricarica dell'attacco
        yield return new WaitForSeconds(pauseAttack);
        //Resetta tutto
        hasAttacked = false;
        isRunning = false;
    }

    private void MoveTowardsPlayer()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) > attackRange)
        {
            GetComponent<Animator>().SetBool("Run", true);
            // combatSystem.CurrentState = CombatSystem.CharacterState.MOVING;
            
        }
        
    }





}
