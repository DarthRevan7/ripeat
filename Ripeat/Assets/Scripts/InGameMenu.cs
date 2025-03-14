using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private bool menuTriggered = false;
    [SerializeField] private GameObject escPanel;

    void Awake()
    {
        escPanel = GameObject.Find("EscPanel");
        escPanel.transform.Find("BackGameButton").GetComponent<Button>().onClick.AddListener( () => TriggerMenu() );
        escPanel.transform.Find("BackMenuButton").GetComponent<Button>().onClick.AddListener( () => BackToMainMenu() );
        escPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TriggerMenu();
        }
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
    }

}
