using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuScript : MonoBehaviour
{
    private Button button;
    [SerializeField] private string sceneToLoad = "FightingScene_Try"; 
    
    
    private void Awake()
    {
        button = GetComponent<Button>();


        if (button != null)
        {
            button.onClick.AddListener(() => LoadScene(sceneToLoad));
        }
    }

    public void LoadScene(string sceneName)
    {

    
        StartCoroutine(LoadSceneAfterDelay(sceneName));
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        
        yield return new WaitForSeconds(2f);
        // Carica la nuova scena
        SceneManager.LoadScene(sceneName);
    }
}

