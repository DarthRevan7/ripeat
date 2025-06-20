using UnityEngine;
using UnityEngine.SceneManagement;

public class GeminiPrompt : MonoBehaviour
{
    [Header("Prompt")]
    [TextArea] public string prompt1 = "";
    [TextArea] public string prompt2 = "";
    [TextArea] public string prompt3 = "";
    [TextArea] public string prompt4 = "";
    [TextArea] public string promptBonus = "";
    
    public static int ciclesNumber = 0;

    // Aggiungiamo i riferimenti necessari per leggere lastKiller
    private FighterStats fighterStats;
    public EventHandler eventHandler;
    private string killerName = "";
    public static string mainName = "";
    public bool bonus = false;
    int count = 0;

    void Start()
    {
        eventHandler = GetComponent<EventHandler>();
    }
    
    public int SwitchImplementation()
    {
        if(FightEventController.Instance == null)
        {
            return 0;
        }

        if(FightEventController.Instance.triggeredEventIndices.Count == 0)
        {
            return 1;
        }
        else
        {
            return FightEventController.Instance.triggeredEventIndices.Count%FightEventController.Instance.loadedEvents.Count +1;
        }
    }

    public void SetName(string name)
    {
        mainName = name;
    }
   
    public string getPrompt()
    {
        
        string prompt = "Il nome dell'anima è: " + mainName + "\n\n";
    
        
        switch(FightEventController.globalEventIndex)
        {
            case 0:
                prompt += prompt1;
                break;
            case 1:
                prompt += prompt2;
                break;
            case 2:
                prompt += prompt3;
                break;
            case 3:
                prompt += prompt4;
                break;
            default:
                break;
        }
        count++;

        return prompt;
    }

    public void incrementCicles(){
        ciclesNumber++;
        Debug.Log("Cicles number: " + ciclesNumber);
    }

    public void resetCicles(){
        ciclesNumber = 0;
        UnityAndGeminiV3.conversationHistory = "";
        mainName = "";
        Debug.Log("Cicles number reset: " + ciclesNumber);
    }
}
