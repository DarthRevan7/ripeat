using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputPlayer : MonoBehaviour
{
    [SerializeField] private CombatAnimSystem combatSystem;

    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private string inputPathPC = "InputActions\\InputSystem_PC",
                                    inputPathController = "InputActions\\InputSystem_Controller";
    [SerializeField] private string inputActionMapName = "Player",
                                    inputActionMovementName = "Move",
                                    inputActionPunchName = "Punch",
                                    inputActionKickName = "Kick",
                                    inputActionBlockName = "Block";

    [SerializeField] private bool inputPC = true;

    #region Exposed Vars
    public bool canMove = true, isScriptActive = true;
    #endregion

    #region Private Vars
    private Vector3 movement;
    #endregion

    // Callback per il blocco
    public void OnBlockStarted(InputAction.CallbackContext callbackContext)
    {
        // Se si inizia il blocco, richiedi lo stato BLOCK.
        combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.BLOCK);
        combatSystem.SetBlockBool(true); // Imposta il parametro booleano nell'Animator
        canMove = false; // Blocca il movimento durante il blocco
    }

    public void OnBlockCanceled(InputAction.CallbackContext callbackContext)
    {
        combatSystem.SetBlockBool(false); // Disattiva il parametro booleano nell'Animator
        canMove = true; // Permetti di nuovo il movimento

        // Quando il blocco viene rilasciato, torna a IDLE solo se era effettivamente in BLOCK
        // Questo è importante per non interrompere altre animazioni (es. attacchi).
        if (combatSystem.CurrentState == CombatAnimSystem.CombatAnimState.BLOCK)
        {
            combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.IDLE);
        }
    }

    // Callback per il pugno
    public void OnPunchPerformed(InputAction.CallbackContext callbackContext)
    {
        combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.PUNCH);
        canMove = false; // Blocca il movimento durante l'attacco
    }

    // Callback per il calcio
    public void OnKickPerformed(InputAction.CallbackContext callbackContext)
    {
        combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.KICK);
        canMove = false; 
    }

    //Chiamato da un evento dell'Animazione, in modo da consentire il movimento.
    public void EnableMovement()
    {
        canMove = true;
    }



    public void CharacterMovement()
    {
        Vector2 inputMovement = inputAction.FindActionMap(inputActionMapName).FindAction(inputActionMovementName).ReadValue<Vector2>();
        movement = new Vector3(inputMovement.x, 0, inputMovement.y);

        if (canMove && movement.magnitude > 0.2f)
        {
            // Aggiorna il forward del personaggio 
            transform.forward = movement;
            // Ricava la velocità di movimento
            float movementSpeed = GetComponent<FighterStats>().movementSpeed;
            // Fa muovere il personaggio
            GetComponent<CharacterController>().Move(movement * Time.deltaTime * movementSpeed);


            if (combatSystem.GetAnimState() < 2 && combatSystem.CurrentState != CombatAnimSystem.CombatAnimState.BLOCK) // Non muoverti se in esecuzione attacco o blocco
            {
                combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.MOVING);
            }
        }
        else if (combatSystem.CurrentState == CombatAnimSystem.CombatAnimState.MOVING && canMove)
        {
            if (combatSystem.GetAnimState() < 2 && combatSystem.CurrentState != CombatAnimSystem.CombatAnimState.BLOCK)
            {
                combatSystem.RequestStateChange(CombatAnimSystem.CombatAnimState.IDLE);
            }
        }
    }

    #region Awake & Update

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

        // Sottoscrivo i callback
        inputAction.FindActionMap(inputActionMapName).FindAction(inputActionBlockName).started += OnBlockStarted;
        inputAction.FindActionMap(inputActionMapName).FindAction(inputActionBlockName).canceled += OnBlockCanceled;
        inputAction.FindActionMap(inputActionMapName).FindAction(inputActionPunchName).performed += OnPunchPerformed; // Uso Performed
        inputAction.FindActionMap(inputActionMapName).FindAction(inputActionKickName).performed += OnKickPerformed;
    }

    void Update()
    {
        if (!isScriptActive)
        {
            return;
        }
        // L'unica chiamata che deve rimanere qui è la gestione del movimento.
        // Gli attacchi e il blocco sono gestiti dai callback.
        CharacterMovement();
    }
    #endregion
}