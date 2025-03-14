using UnityEngine;
using UnityEngine.InputSystem;

public class SetPlayerInput : MonoBehaviour
{

    //Riferimento all'oggetto freeflowcombat
    FreeflowCombat freeFlowCombat;
    //Riferimento al "whoosh" sound
    public AudioSource whooshSound;
    //Riferimento al trail renderer
    public TrailRenderer tr;

    //Riferimento all'InputActionAsset
    [SerializeField] private InputActionAsset inputActionPC, inputActionController;

    //Riferimento alla variabile inputModePC di PlayerMovement
    [SerializeField] private bool inputModePC;
    [SerializeField] private bool isDead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        freeFlowCombat = GetComponent<FreeflowCombat>();
        inputModePC = GetComponent<PlayerMovement>().inputModePC;
    }

    // Update is called once per frame
    void Update()
    {
        isDead = GetComponent<CharacterStats>().isDead;
        //Se non sono morto, allora gestisci gli input.
        if(!isDead)
        {
            HandleInputActions();
        }       
    }

    private void HandleInputActions()
    {
        if(inputModePC)
        {
            InputAction mouseSXPressed = inputActionPC.FindActionMap("Player").FindAction("Attack");

            //Attacco col pulsante sx del mouse
            if (mouseSXPressed.WasPressedThisFrame()) {
                freeFlowCombat.Attack();
                Debug.Log("Attack!");
            }

            //Setta gli input nel freeFlowCombat Script
            freeFlowCombat.xInput = inputActionPC.FindActionMap("Player").FindAction("Move").ReadValue<Vector2>().x;
            freeFlowCombat.yInput = inputActionPC.FindActionMap("Player").FindAction("Move").ReadValue<Vector2>().y;
        }
        else
        {
            InputAction attackButtonPressed = inputActionController.FindActionMap("Player").FindAction("Attack");

            //Attacco col pulsante sx del mouse
            if (attackButtonPressed.WasPressedThisFrame() && !isDead) {
                freeFlowCombat.Attack();
                Debug.Log("Attack!");
            }

            //Setta gli input nel freeFlowCombat Script
            freeFlowCombat.xInput = inputActionController.FindActionMap("Player").FindAction("Move").ReadValue<Vector2>().x;
            freeFlowCombat.yInput = inputActionController.FindActionMap("Player").FindAction("Move").ReadValue<Vector2>().y;
        }

        //Whoosh sound
        // if (freeFlowCombat.isTraversing) {
        //     if (!whooshSound.isPlaying) whooshSound.Play();
        // }
        // else {
        //     whooshSound.Stop();
        // }

        //Abilita il trail se si sta attraversando o attaccando
        if (freeFlowCombat.isTraversing || freeFlowCombat.isAttacking) tr.enabled = true;
        else tr.enabled = false;
        
    }
}
