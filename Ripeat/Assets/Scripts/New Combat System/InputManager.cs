using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

     //Velocità di movimento del giocatore
    [SerializeField] private float playerSpeed = 5f;
    //Se attivo prende input PC
    public bool inputModePC = true;
    //Riferimento all'oggetto input action
    //Verrà caricato tramite Resources.Load() sulla base del precedente booleano
    [SerializeField] private InputActionAsset inputAction;
    [SerializeField] private string inputPathPC, inputPathController;

    [SerializeField] private Vector3 movement;


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
