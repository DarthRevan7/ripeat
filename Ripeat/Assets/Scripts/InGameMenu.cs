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

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private InputActionAsset inputActions;

    void Awake()
    {
        escPanel = GameObject.Find("EscPanel");
        escPanel.transform.Find("BackGameButton").GetComponent<Button>().onClick.AddListener( () => TriggerMenu() );
        escPanel.transform.Find("BackMenuButton").GetComponent<Button>().onClick.AddListener( () => BackToMainMenu() );
        escPanel.SetActive(false);

        eventSystem = EventSystem.current;

        inputActions = InputSystem.actions;

        inputActions.FindActionMap("Player").FindAction("Start").performed += MenuTrigger;
        enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuTrigger(InputAction.CallbackContext context)
    {
        TriggerMenu();
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(menuSceneName);
    }

    public void TriggerMenu()
    {
        menuTriggered = !menuTriggered;
        if(menuTriggered)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        escPanel.SetActive(menuTriggered);

        eventSystem.firstSelectedGameObject = GameObject.Find("BackGameButton");
    }

}
