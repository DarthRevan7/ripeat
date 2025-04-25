using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseConditions : MonoBehaviour
{
    [SerializeField] private CombatSystem player, enemy;
    [SerializeField] private bool isLoading = false;
    [SerializeField] private string winningScene = "", losingScene = "";
    [SerializeField] private MenuScript menuScript;

    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CombatSystem>();
        enemy = GameObject.FindGameObjectWithTag("Main Enemy").GetComponent<CombatSystem>();
        menuScript = GameObject.FindAnyObjectByType<MenuScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isDead && !isLoading)
        {
            menuScript.LoadScene();
            isLoading = true;
        }
        else if(enemy.isDead && !isLoading)
        {
            if(winningScene != "")
            {
                menuScript.sceneToLoad = winningScene;
            }
            menuScript.LoadScene();
            isLoading = true;
        }
    }
}
