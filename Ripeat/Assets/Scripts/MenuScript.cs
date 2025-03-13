using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class MenuScript : MonoBehaviour
{
    private Button button;
    [SerializeField] private string sceneToLoad = "FightingScene_Try"; 

    [SerializeField] private Image targetImage;
    [SerializeField] private float fadeDuration = 2f;

    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private bool fading = false;
    
    
    private void Awake()
    {
        button = GetComponent<Button>();
        targetImage = GameObject.Find("FadingImage").GetComponent<Image>();

        if (button != null)
        {
            button.onClick.AddListener(() => LoadScene(sceneToLoad));
        }

        string sceneName = SceneManager.GetActiveScene().name;

        if(sceneName.Equals("FightingScene_Try") || sceneName.Equals("DialogueTest"))
        {
            StartCoroutine(FadeOut());
        }

        characterStats = GameObject.FindAnyObjectByType<CharacterStats>();
        

        
    }

    void Update()
    {
        if(characterStats != null)
        {
            if(characterStats.isDead && !fading)
            {
                fading = true;
                StartCoroutine(FadeIn());
            }
        }
    }



    public void LoadScene(string sceneName)
    {
        // StartCoroutine(LoadSceneAfterDelay(sceneName));
        StartCoroutine(FadeIn());
    }

    // private IEnumerator LoadSceneAfterDelay(string sceneName)
    // {
        
    //     yield return new WaitForSeconds(2f);
    //     // Carica la nuova scena
    //     SceneManager.LoadScene(sceneName);
    // }

    public IEnumerator FadeIn()
    {
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

        SceneManager.LoadScene(sceneToLoad);
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

