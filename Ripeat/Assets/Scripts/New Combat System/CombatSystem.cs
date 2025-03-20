using UnityEngine;

public class CombatSystem : MonoBehaviour
{

    //Enum x gli stati in cui si trova il personaggio
    public enum CharacterState
    {
        IDLE, MOVING, KICK, PUNCH, BLOCK, DEAD
    };

    //Properties per gestire input e stato del personaggio (se non è morto!)
    [SerializeField] private Vector3 movementInput;

    public Vector3 MovementInput
    {
        get { return movementInput; }
        set
        {
            if(currentState != CharacterState.DEAD)
            {
                movementInput = value;
            }
        }
    }

    [SerializeField] private CharacterState currentState = CharacterState.IDLE;

    public CharacterState CurrentState
    {
        get { return currentState; }
        set
        {
            if (currentState != CharacterState.DEAD) // Se sono morto, non posso cambiare stato
            {
                currentState = value;
                UpdateAnimationState();
            }
        }
    }

    //Riferimento all'Animator del personaggio
    [SerializeField] private Animator animator;



    //Aggiorna lo stato dell'Animator in base allo stato del personaggio (va chiamato in Update)
    void UpdateAnimationState()
    {
        switch (currentState)
        {
            case CharacterState.IDLE:
                
                break;
            case CharacterState.MOVING:
                
                break;
            case CharacterState.PUNCH:
                
                break;
            case CharacterState.KICK:
                
                break;
            case CharacterState.BLOCK:
                
                break;
            case CharacterState.DEAD:
                
                break;
        }
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimationState();
    }
}
