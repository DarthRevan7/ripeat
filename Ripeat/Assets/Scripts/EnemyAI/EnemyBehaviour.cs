using UnityEngine;
using FreeFlowCombatSpace;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;
using System.Text.RegularExpressions;
using System;
using FreeflowCombatSpace;

public class EnemyBehaviour : MonoBehaviour
{

    //Riferimento al Player
    [SerializeField] private Transform playerTransform;
    //Raggio di attacco
    [SerializeField] private float attackRange = 2.5f;

    //Espandere per futuri sviluppi
    enum EnemyStatus {
        //Status dell'IA del nemico
        FOLLOWING_PLAYER, ATTACK, IDLE
    }

    [SerializeField] private EnemyStatus enemyStatus = EnemyStatus.IDLE;

    [SerializeField] private float pauseAttack = 2.0f;
    [SerializeField] private bool hasAttacked = false;

    //Danno del nemico
    [SerializeField] private int damage = 10;

    

    //Errore con la dicitura seguente:
    /*
    
    'Enemy' AnimationEvent 'StopHit' on animation 'combo_01_1' has no receiver! Are you missing a component?
    
    */

    //Risolto eliminando l'evento in un duplicato dell'animazione ed usando quello.

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if(GetComponent<Health>().isDead || playerTransform.GetComponent<CharacterStats>().isDead) return;

        if(enemyStatus == EnemyStatus.FOLLOWING_PLAYER)
        {
            MoveTowardsPlayer();
        }
        else if(enemyStatus == EnemyStatus.ATTACK)
        {
            AttackPlayer();
        }

        FollowAndAttack();
        
        

    }

    private void FollowAndAttack()
    {   

        if(enemyStatus != EnemyStatus.FOLLOWING_PLAYER && Vector3.Distance(transform.position, playerTransform.position) > attackRange)
        {
            enemyStatus = EnemyStatus.FOLLOWING_PLAYER;
            GetComponent<Animator>().SetInteger("AttackType", 0);
        }
        else if(enemyStatus != EnemyStatus.ATTACK && Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            enemyStatus = EnemyStatus.ATTACK;
        }

    }

    private void AttackPlayer()
    {
        
        GetComponent<Animator>().SetBool("Run", false);
        if(Vector3.Distance(transform.position, playerTransform.position) <= attackRange && !hasAttacked)
        {
            GetComponent<Animator>().SetInteger("AttackType", 1);
            StartCoroutine(AttackWaitingTime());
            hasAttacked = true;
        }

        
    }

    IEnumerator AttackWaitingTime()
    {
        GetComponent<Animator>().SetInteger("AttackType", 1);
        yield return new WaitUntil( () => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f );
        playerTransform.GetComponent<CharacterStats>().HitTarget(damage);
        yield return new WaitUntil( () => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f );
        GetComponent<Animator>().SetInteger("AttackType", 0);
        yield return new WaitForSeconds(pauseAttack);
        hasAttacked = false;
    }

    private void MoveTowardsPlayer()
    {
        if(Vector3.Distance(transform.position, playerTransform.position) > attackRange)
        {
            GetComponent<Animator>().SetBool("Run", true);
        }
        
    }





}
