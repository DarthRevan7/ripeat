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
            if(collider.gameObject.name.Equals(targetName))
            {
                collider.gameObject.GetComponent<FighterStats>().vita -= attacco;
                Debug.Log("Nemico Colpito!!");
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
