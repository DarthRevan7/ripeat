using System.Net.Mail;
using UnityEngine;

public class FighterStats : MonoBehaviour
{

    public int vita;
    public float movementSpeed = 5f;
    public int attacco = 5;

    [SerializeField] private float colliderRadiusPunch = 0.7f, colliderRadiusKick = 0.9f;


    public void Hit()
    {
        float colliderRadius;
        
        if(GetComponent<CombatSystem>().CurrentState == CombatSystem.CharacterState.KICK)
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
            if(collider.gameObject.name.Equals("MyEnemyNew"))
            {
                collider.gameObject.GetComponent<FighterStats>().vita -= attacco;
                Debug.Log("Nemico Colpito!!");
            }
        }
        GetComponent<CombatSystem>().CurrentState = CombatSystem.CharacterState.IDLE;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
