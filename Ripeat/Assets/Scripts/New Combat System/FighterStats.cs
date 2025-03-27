using UnityEngine;

public class FighterStats : MonoBehaviour
{

    public int vita;
    public float movementSpeed = 5f;

    [SerializeField] private float colliderRadius = 5f;


    public void Hit()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position,colliderRadius);

        foreach (Collider collider in colliders)
        {
            if(collider.gameObject.name.Equals("MyEnemyNew"))
            {
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
