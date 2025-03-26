using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

     //Velocità di movimento del giocatore
    [SerializeField] private float playerSpeed = 5f;
    //Se attivo prende input PC
    public bool inputModePC = false;
    //Riferimento all'oggetto input action
    //Verrà caricato tramite Resources.Load() sulla base del precedente booleano
    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private string inputPathPC, inputPathController;
    [SerializeField] private string inputActionMapName = "Player", inputActionMovementName = "Move";

    [SerializeField] private Vector3 movement;

    //Riferimento al combat system
    [SerializeField] private CombatSystem combatSystem;


    void Awake()
    {
        //Carico da Resources le input action in base al booleano
        if(inputModePC)
        {
            inputAction = Resources.Load<InputActionAsset>(inputPathPC);
        }
        else
        {
            inputAction = Resources.Load<InputActionAsset>(inputPathController);
        }

        //Trovo il combat system tra i component dell'oggetto
        combatSystem = GetComponent<CombatSystem>();
    }

    CombatSystem.CharacterState HandleCharacterState()
    {
        bool punch, kick, block;
        
        //Ricavo i booleani che indicano se il pulsante è stato premuto
        punch = inputAction.FindActionMap("Player").FindAction("Punch").IsPressed();
        kick = inputAction.FindActionMap("Player").FindAction("Kick").IsPressed();
        block = inputAction.FindActionMap("Player").FindAction("Block").IsPressed();

        //In ordine, faccio ritornare lo stato del character
        if(block)
        {
            return CombatSystem.CharacterState.BLOCK;
        }
        if(punch)
        {
            return CombatSystem.CharacterState.PUNCH;
        }
        if(kick)
        {
            return CombatSystem.CharacterState.KICK;
        }

        return combatSystem.CurrentState;
    }

    Vector3 BuildMovementVector()
    {

        Vector3 movement;
        //Ricavo il Vector2 del movimento sul controller
        Vector2 inputMovement = inputAction.FindActionMap(inputActionMapName).FindAction(inputActionMovementName).ReadValue<Vector2>();
        //Imposto il vettore movimento nel mondo
        movement = new Vector3(inputMovement.x,0,inputMovement.y);

        
        //Se ho un movimento maggiore di 0.2f, come in CombatSystem.cs
        if(movement.magnitude > 0.2f)
        {
            //Aggiorno il forward del personaggio (lo ruota violentemente in una direzione)
            transform.forward = movement;
            //Ricava la velocità di movimento
            float movementSpeed = GetComponent<FighterStats>().movementSpeed;
            //Fa muovere il personaggio
            GetComponent<CharacterController>().Move(movement * Time.deltaTime * movementSpeed);
        }
        

        return movement;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Passo al Combat System l'input tramite property
        combatSystem.MovementInput = BuildMovementVector();

        //Mi occupo di settare lo stato del personaggio tramite la property
        combatSystem.CurrentState = HandleCharacterState();
    }




    


}
