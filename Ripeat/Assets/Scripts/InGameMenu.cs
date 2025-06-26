using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private bool menuTriggered = false;
    [SerializeField] private GameObject escPanel;
    [SerializeField] private GameObject ControllerPanel;

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private GameObject player, enemy, secondEnemy;

    void Awake()
    {
        escPanel = GameObject.Find("EscPanel");
        escPanel.transform.Find("Riprendi").GetComponent<Button>().onClick.AddListener(() => { Debug.Log("Menu triggerato"); TriggerMenu(); });
        escPanel.transform.Find("Menu").GetComponent<Button>().onClick.AddListener(() => BackToMainMenu());
        escPanel.SetActive(false);

        eventSystem = EventSystem.current;

        inputActions = InputSystem.actions;

        inputActions.FindActionMap("Player").FindAction("Start").performed += MenuTrigger;
        enabled = true;

        if (SceneManager.GetActiveScene().name.Equals("CombatScene"))
        {
            player = GameObject.FindGameObjectWithTag("Player");
            enemy = GameObject.FindGameObjectWithTag("Main Enemy");
            secondEnemy = GameObject.FindGameObjectWithTag("Secondary Enemy");
        }


    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuTrigger(InputAction.CallbackContext context)
    {
        if(!ControllerPanel.activeSelf)
        {
            TriggerMenu();
            Debug.Log("Menu triggerato");
        }
        else
        {
            Debug.Log("ControllerPanel is active");
        }
    }

    public void BackToMainMenu()
    {
        Debug.Log("Menu triggerato");
        Time.timeScale = 1;
        SceneManager.LoadScene(menuSceneName);
    }

    public void TriggerMenu()
    {
        menuTriggered = !menuTriggered;
        Debug.Log("Menu triggerato");
        if (menuTriggered)
        {
            Time.timeScale = 0;
            /*
            if (SceneManager.GetActiveScene().name.Equals("CombatScene"))
            {
                player.GetComponent<InputManager>().isScriptActive = false;
                enemy.GetComponent<AI.MainEnemyAI>().isScriptActive = false;
                enemy.GetComponent<CombatSystem>().MovementInput = Vector3.zero;
                if (secondEnemy != null)
                {
                    secondEnemy.GetComponent<AI.MainEnemyAI>().isScriptActive = false;
                    secondEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.zero;
                }
            }
            */
        }
        else
            {
            Time.timeScale = 1;
            /*
            if (SceneManager.GetActiveScene().name.Equals("CombatScene"))
            {
                player.GetComponent<InputManager>().isScriptActive = true;
                enemy.GetComponent<AI.MainEnemyAI>().isScriptActive = true;
                if (secondEnemy != null)
                {
                    secondEnemy.GetComponent<AI.MainEnemyAI>().isScriptActive = true;
                }
            }
            */
        }
        escPanel.SetActive(menuTriggered);

        eventSystem.firstSelectedGameObject = GameObject.Find("BackGameButton");
        // eventSystem.currentSelectedGameObject.GetComponent<Button>()
    }
}
