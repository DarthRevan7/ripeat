using UnityEngine;

//Gestisce le animazioni e gli stati del personaggio.
public class CombatSystem : MonoBehaviour
{

    //Enum x gli stati in cui si trova il personaggio
    public enum CharacterState
    {
        IDLE, MOVING, KICK, PUNCH, BLOCK, DEAD
    };

    //Properties per gestire input e movimento del personaggio (se non è morto!)
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
    [SerializeField] private string punchAnimationName, kickAnimationName, blockAnimationName;
    [SerializeField] private string movingParameterName;

    //Sistema per evitare che il personaggio si muova mentre attacca
    public bool canMove = true;

    

    public void ToggleMove()
    {
        canMove = true;
    }

    //Aggiorna lo stato dell'Animator in base allo stato del personaggio (va chiamato in Update)
    void UpdateAnimationState()
    {
        //Se è morto, non faccio nulla.
        if(currentState == CharacterState.DEAD)
            return;

        //Se c'è un attacco o parata, il personaggio deve attaccare o parare
        if(currentState == CharacterState.KICK || currentState == CharacterState.PUNCH || currentState == CharacterState.BLOCK)
        {
            canMove = false;
            switch(currentState)
            {
                case CharacterState.PUNCH:
                    animator.Play(punchAnimationName);
                break;
                case CharacterState.KICK:
                    animator.Play(kickAnimationName);
                break;
                case CharacterState.BLOCK:
                    animator.Play(blockAnimationName);
                break;
                default:
                    Debug.Log("Default in switch updateanimationstate");
                break;
            }
        }
        //Se c'è un input di movimento e non di attacco, devo solo muovermi
        else if(movementInput.magnitude > 0.2f && canMove)
        {
            currentState = CharacterState.MOVING;
            animator.SetBool(movingParameterName, true);
        }
        else if(movementInput.magnitude <= 0.2f)
        {
            currentState = CharacterState.IDLE;
            animator.SetBool(movingParameterName, false);
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
