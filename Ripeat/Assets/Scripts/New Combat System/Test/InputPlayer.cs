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

    #region Variables

    [SerializeField] private bool canMove = true;

    #endregion


    public void HandleCharacterState()
    {
        bool punch, kick, block;

        //Ricavo i booleani che indicano se il pulsante è stato premuto
        punch = inputAction.FindActionMap("Player").FindAction("Punch").IsPressed();
        kick = inputAction.FindActionMap("Player").FindAction("Kick").IsPressed();
        block = inputAction.FindActionMap("Player").FindAction("Block").IsPressed();

        //In ordine, faccio ritornare lo stato del character
        if (block)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.BLOCK);
        }
        if (punch)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.PUNCH);
        }
        if (kick)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.KICK);
        }
    }
    public void CharacterMovement()
    {
        Vector3 movement;
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
            //Fa muovere il personaggio
            GetComponent<CharacterController>().Move(movement * Time.deltaTime * movementSpeed);
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.MOVING);
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleCharacterState();
    }
}
