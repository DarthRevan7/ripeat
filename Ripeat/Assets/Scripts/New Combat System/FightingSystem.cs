using Unity.VisualScripting;
using UnityEngine;

//Gestisce le animazioni e gli stati del personaggio.
public class FightingSystem : MonoBehaviour
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

    public CharacterState currentState = CharacterState.IDLE;
    
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
    [SerializeField] public Animator animator;

    //Sistema per evitare che il personaggio si muova mentre attacca
    public bool canMove = true, isDead = false, isBlocked = false, pauseBlock = false;
    public bool blockHeld = false; // This flag must be set externally when block is held down

    private MenuScript menuScript;
    private InputManager inputManager;
    
    
    public void ToggleMove()
    {
        canMove = true;
    }
    public void ToggleBlock()
    {
        blockHeld = false;
    }


    //Aggiorna lo stato dell'Animator in base allo stato del personaggio (va chiamato in Update)
    public void UpdateAnimationState()
    {
        //Se è morto, non faccio nulla.
        if(currentState == CharacterState.DEAD)
        {
            if(!isDead)
            {
                animator.SetInteger("State", 5);
                isDead = true;
            }
            return;
        }
            

        //Se c'è un attacco o parata, il personaggio deve attaccare o parare
        if(currentState == CharacterState.KICK || currentState == CharacterState.PUNCH || currentState == CharacterState.BLOCK)
        {
            canMove = false;
            isBlocked = false;
            switch(currentState)
            {
                case CharacterState.PUNCH:
                    animator.SetInteger("State", 3);
                    // Debug.Log("Punch animation played");
                break;
                case CharacterState.KICK:
                    animator.SetInteger("State", 2);
                break;
                case CharacterState.BLOCK:
                    isBlocked = true;
                    animator.SetInteger("State", 4);
                    // Start the block animation at its halfway point if it's not already playing
                    // if (!animator.GetCurrentAnimatorStateInfo(0).IsName(blockAnimationName))
                    // {
                    //     animator.Play(blockAnimationName, 0, 0.5f);
                    // }
                    // // While the block button is held, freeze the animation
                    // if (blockHeld)
                    // {
                    //     animator.speed = 0.001f;
                    // }
                    // else
                    // {
                    //     // If the block button is clicked again (blockHeld toggled externally to false),
                    //     // resume the animation and finish it to end the block state.
                    //     animator.speed = 1;
                    //     animator.Play(blockAnimationName, 0, 1f);
                    //     currentState = CharacterState.IDLE;
                    // }
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
            animator.SetInteger("State", 1);
        }
        else if(movementInput.magnitude <= 0.2f)
        {
            currentState = CharacterState.IDLE;
            animator.SetInteger("State", 0);
        }
        
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        // menuScript = GameObject.Find("FadingImage").GetComponent<MenuScript>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimationState();
    }
}
