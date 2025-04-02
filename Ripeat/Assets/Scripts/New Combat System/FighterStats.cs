using System.Net.Mail;
using UnityEngine;

public class FighterStats : MonoBehaviour
{

    public int vita;
    public float movementSpeed = 5f;
    public int attacco = 5;
    public bool isDead;
    [SerializeField] private float colliderRadiusPunch = 0.7f, colliderRadiusKick = 0.9f;
    [SerializeField] private string targetName = "MyEnemyNew";
    [SerializeField] private CombatSystem combatSystem;



    
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


        Collider[] colliders = Physics.OverlapSphere(transform.position,colliderRadius);

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
                        other.vita -= attacco;
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
                        other.vita -= attacco;
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
            combatSystem.CurrentState = CombatSystem.CharacterState.DEAD;
        }
    }
}
