using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputPlayer : MonoBehaviour
{

    [SerializeField] private CombatAnimSystem combatSystem;

    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private string inputPathPC = "InputActions\\InputSystem_PC",
    inputPathController = "InputActions\\InputSystem_Controller";
    [SerializeField] private string inputActionMapName = "Player", inputActionMovementName = "Move";
    [SerializeField] private bool inputPC = true;

    #region Exposed Vars

    [SerializeField] private bool canMove = true;


    #endregion

    #region Private Vars

    private bool punch, kick, block;
    private Vector3 movement;

    #endregion

    public void OnBlockStarted(InputAction.CallbackContext callbackContext)
    {
        combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.BLOCK);
        combatSystem.SetBlockBool(true);
        canMove = false;
    }

    public void OnBlockCanceled(InputAction.CallbackContext callbackContext)
    {
        combatSystem.SetBlockBool(false);
        canMove = true;
    }

    public void HandleCharacterState()
    {


        //Ricavo i booleani che indicano se il pulsante è stato premuto
        punch = inputAction.FindActionMap("Player").FindAction("Punch").IsPressed();
        kick = inputAction.FindActionMap("Player").FindAction("Kick").IsPressed();
        // block = inputAction.FindActionMap("Player").FindAction("Block").WasPerformedThisFrame();

        //In ordine, faccio ritornare lo stato del character
        if (block)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.BLOCK);
        }
        else if (punch)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.PUNCH);
        }
        else if (kick)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.KICK);
        }
    }
    public void CharacterMovement()
    {
        //Ricavo il Vector2 del movimento sul controller
        Vector2 inputMovement = inputAction.FindActionMap(inputActionMapName).FindAction(inputActionMovementName).ReadValue<Vector2>();
        //Imposto il vettore movimento nel mondo
        movement = new Vector3(inputMovement.x, 0, inputMovement.y);


        //Se ho un movimento maggiore di 0.2f, come in CombatSystem.cs
        //E se posso muovere il personaggio
        if (movement.magnitude > 0.2f && canMove)
        {
            //Aggiorno il forward del personaggio (lo ruota violentemente in una direzione)
            transform.forward = movement;
            //Ricava la velocità di movimento
            float movementSpeed = GetComponent<FighterStats>().movementSpeed;
            //Aggiorna lo stato del combat system
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.MOVING);
            //Fa muovere il personaggio
            GetComponent<CharacterController>().Move(movement * Time.deltaTime * movementSpeed);
            
        }
        else
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.IDLE);
        }
        
        
    }
    public void CleanInputs()
    {
        bool inputs = block || punch || kick || (movement.magnitude > 0.2f && canMove);
        if (!inputs)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.IDLE);
        }
    }

    private void Awake()
    {
        combatSystem = GetComponent<CombatAnimSystem>();

        if (inputPC)
        {
            inputAction = Resources.Load<InputActionAsset>(inputPathPC);
        }
        else
        {
            inputAction = Resources.Load<InputActionAsset>(inputPathController);
        }
        inputAction.FindActionMap("Player").FindAction("Block").started += OnBlockStarted;
        inputAction.FindActionMap("Player").FindAction("Block").canceled += OnBlockCanceled;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleCharacterState();
        CharacterMovement();
        CleanInputs();
    }
}
