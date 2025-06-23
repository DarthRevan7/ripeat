using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour
{
    private Button button;
    [SerializeField] public string sceneToLoad = "FightingScene_Try"; 
    [SerializeField] private GameObject ImageControl;   
    [SerializeField] private Image targetImage;
    [SerializeField] private float fadeDuration = 2f;

    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private FighterStats enemyStats;
    [SerializeField] private bool fading = false;
    
    private GeminiPrompt geminiPrompt;
    private ResponseHandler responseHandler;

    private void Awake()
    {
        button = GetComponent<Button>();
        targetImage = GameObject.Find("FadingImage").GetComponent<Image>();

        geminiPrompt = GetComponent<GeminiPrompt>();

        if (button != null)
        {
            button.onClick.AddListener(() => LoadScene());
            EventSystem.current.firstSelectedGameObject = button.gameObject;
        }

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Equals("CombatScene") || sceneName.Equals("NewCombatScene") || sceneName.Equals("Menu") || sceneName.Equals("NewIntro") || sceneName.Equals("DialogueWithAI") || sceneName.Equals("Elevator"))
        {
            StartCoroutine(FadeOut());
        }

        characterStats = GameObject.FindAnyObjectByType<CharacterStats>();

        SceneManager.sceneLoaded += OnLoadScene;
        
        
        // Time.timeScale = 1;


    }

    private void OnLoadScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneManager.GetActiveScene().name.Equals("CombatScene"))
        {
            enemyStats = GameObject.FindGameObjectWithTag("Main Enemy").GetComponent<FighterStats>();
        }
    }

    public void Stop()
    {
        button.onClick.RemoveAllListeners();
    }

    void Update()
    {
        if (characterStats != null)
        {
            if (characterStats.isDead && !fading)
            {
                fading = true;
                StartCoroutine(FadeIn());
            }
        }

        if (enemyStats != null)
        {
            if (enemyStats.isDead && !fading)
            {
                sceneToLoad = "FinalVideo";
                StartCoroutine(FadeIn());
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ImageControl.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame() called");
        Application.Quit();
    }

    public void LoadScene()
    {
        // Debug.Log("Called LoadScene()");
        // StartCoroutine(LoadSceneAfterDelay(sceneName));
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    // private IEnumerator LoadSceneAfterDelay(string sceneName)
    // {

    //     yield return new WaitForSeconds(2f);
    //     // Carica la nuova scena
    //     SceneManager.LoadScene(sceneName);
    // }

    public void Control()
    {
        Debug.Log("Control() called");
        ImageControl.SetActive(true);
    }

    public IEnumerator FadeIn()
    {
        // Debug.Log("Started Coroutine FadeIn()");
        float elapsedTime = 0f;
        Color color = targetImage.color;
        color.a = 0f; // Partiamo con alfa a 0 (trasparente)
        targetImage.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            targetImage.color = color;
            yield return null;
        }


        if (SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            Debug.Log("Cambiamento di scena: " + SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad)).ToString());
        }
        else
        {
            if (sceneToLoad.Equals("Menu"))
            {
                geminiPrompt.resetCicles();
                responseHandler.Restart();
            }
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public IEnumerator FadeOut()
    {
        // Time.timeScale = 0.0f;
        float elapsedTime = 0f;
        Color color = targetImage.color;
        color.a = 1f; // Partiamo con alfa a 1 (opaco)
        targetImage.color = color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            targetImage.color = color;
            yield return null;
        }

        // Time.timeScale = 1f;
    

        if(SceneManager.GetActiveScene().name.Equals("FightingScene_Try"))
        {
            GameObject.FindAnyObjectByType<EnemyBehaviour>().WaitBeforeFight();
        }

    }
}

