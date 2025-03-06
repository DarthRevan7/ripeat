using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Velocità di movimento del giocatore
    [SerializeField] private float playerSpeed = 5f;
    //Riferimenti al controller ed all'animator
    [SerializeField] CharacterController controller;
    [SerializeField] Animator anim;
    //Settare true se siamo col PC, altrimenti settare FALSE
    //Ho messo true perché non ho il controller 
    public bool inputModePC = true;
    //Riferimento ai due oggetti InputAction 
    [SerializeField] private InputActionAsset inputActionPC, inputActionController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        //Lo script originale qui bloccava il cursore al centro dello schermo
        //e lo rendeva invisibile.
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = BuildMovementVector(inputModePC);
        controller.Move(movement * playerSpeed * Time.deltaTime);
        
        if(movement != Vector3.zero)
        {
            gameObject.transform.forward = movement;
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }

    

    private Vector3 BuildMovementVector(bool inputMode)
    {
        Vector3 movement = Vector3.zero;

        if(inputMode)
        {
            if(inputActionPC == null)
            {
                Debug.Log("Assegnare input action x PC!!");
                return Vector3.zero;
            }

            // var asset = InputActionAsset.FromJson("InputSystem_PC");
            InputActionMap inputActionMap = inputActionPC.FindActionMap("Player");
            Vector2 inputMovement = inputActionMap.FindAction("Move").ReadValue<Vector2>();

            movement = new Vector3(inputMovement.x,0,inputMovement.y);
            return movement;
        }
        else
        {
            if(inputActionController == null)
            {
                Debug.Log("Assegnare input action x Controller!!");
                return Vector3.zero;
            }

            InputActionMap inputActionMap = inputActionController.FindActionMap("Player");
            Vector2 inputMovement = inputActionMap.FindAction("Move").ReadValue<Vector2>();

            movement = new Vector3(inputMovement.x,0,inputMovement.y);
            return movement;
        }
    }

}
