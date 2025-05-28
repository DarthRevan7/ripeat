using System.Net.Mail;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FighterStats : MonoBehaviour
{

    public int vita;
    public float movementSpeed = 5f;
    public int attacco = 5;
    public bool isDead;

    public static string lastKiller = "";
    [SerializeField] private float colliderRadiusPunch = 0.7f, colliderRadiusKick = 0.9f;
    [SerializeField] private string targetName = "MyEnemyNew";
    [SerializeField] private CombatSystem combatSystem;
    [SerializeField] private int hitCount = 0;

    private bool hitted = false;
    
    
    public void Hit()
    {
        if(isDead) return;

        float colliderRadius;
        
        if(combatSystem.CurrentState == CombatSystem.CharacterState.KICK)
        {
            colliderRadius = colliderRadiusKick;
        }
        else
        {
            colliderRadius = colliderRadiusPunch;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, colliderRadius);

        foreach (Collider collider in colliders)
        {
            //Controllo che il collider del combattente non sia rilevato nella stessa sfera
            //In questo caso impersono il giocatore che non deve colpire sè stesso
            if(gameObject.tag.Equals("Player"))
            {
                if(collider.gameObject.tag.Equals("Player"))
                {
                    continue;
                }
                else
                {
                    FighterStats other = collider.GetComponent<FighterStats>();
                    if(other != null)
                    {
                        //Controllo di essere nella direzione del nemico
                        Vector3 direzioneNemico = (other.transform.position - transform.position).normalized;
                        if(Vector3.Dot(transform.forward, direzioneNemico) < 0.5f)
                        {
                            break;
                        }
                        // La vita viene diminuita solo se non è in stato di block
                        if(!other.combatSystem.isBlocked)
                        {
                            other.vita -= attacco;
                            hitted = true;
                            // Debug.Log("Hitted: " + hitted);
                            other.hitCount = 0;
                        }
                        else{
                            if(other.hitCount >= 3){
                                other.vita -= attacco;
                                other.hitCount = 0;
                                other.combatSystem.isBlocked = false;
                                other.combatSystem.canMove = true;
                                other.combatSystem.CurrentState = CombatSystem.CharacterState.IDLE;
                                other.combatSystem.blockHeld = false;
                                other.combatSystem.animator.speed = 1;
                            }
                            other.hitCount++;
                            // Debug.Log("HitCount: " + hitCount);
                        }
                        
                        
                        lastKiller = collider.gameObject.name;
                        // Debug.Log("Last killer: " + lastKiller);
                        
                        //Senza questo break si potrebbero colpire più nemici alla volta
                        break;
                    }
                }
            }
            //In questo caso impersono il nemico che non deve colpire sè stesso
            //oppure altri nemici
            else
            {
                if(!collider.gameObject.tag.Equals("Player"))
                {
                    continue;
                }
                else
                {
                    FighterStats other = collider.GetComponent<FighterStats>();
                    if(other != null)
                    {
                        if(!other.combatSystem.isBlocked)
                        {
                            other.vita -= attacco;
                            hitted = true;
                            // Debug.Log("Hitted: " + hitted);
                            other.hitCount = 0;
                        }
                        else{
                            if(other.hitCount >= 2){
                                other.vita -= attacco;
                                other.hitCount = 0;
                                other.combatSystem.isBlocked = false;
                                other.combatSystem.canMove = true;
                                other.combatSystem.CurrentState = CombatSystem.CharacterState.IDLE;
                                other.combatSystem.blockHeld = false;
                                other.combatSystem.animator.speed = 1;
                            }
                            other.hitCount++;
                            Debug.Log("HitCount: " + hitCount);
                        }
                        lastKiller = gameObject.name;
                        //Senza questo break si potrebbero colpire più nemici alla volta
                        break;
                    }
                }
            }
        }
        combatSystem.CurrentState = CombatSystem.CharacterState.IDLE;
    }

    void Awake()
    {
        combatSystem = GetComponent<CombatSystem>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    

    // Update is called once per frame
    void Update()
    {
        isDead = vita <= 0;
        if(vita < 0) vita = 0;
        if(isDead)
        {
            if(combatSystem != null){
                if (!hitted)
                {
                    lastKiller = "MyEnemyNew";
                    // Debug.Log("Last killer: " + lastKiller);
                }
                combatSystem.CurrentState = CombatSystem.CharacterState.DEAD;
            }
            
        }
    }
}
