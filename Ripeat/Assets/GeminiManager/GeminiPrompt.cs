using UnityEngine;

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
    public bool bonus = false;

    void Start()
    {
        eventHandler = GetComponent<EventHandler>();
    }
   
    public string getPrompt()
    {
        
        // fighterStats = GetComponent<FighterStats>();
        // if(fighterStats == null)
        // {
        //     Debug.LogWarning("FighterStats non trovato su " + gameObject.name);
        // }
        // else
        // {
        //     Debug.Log("FighterStats trovato su " + gameObject.name);
        // }
        // // Legge la variabile lastKiller dal componente FighterStats (se presente)
        // if(fighterStats != null)
        // {
        //     killerName = FighterStats.lastKiller;
        // }
        // else
        // {
        //     killerName = "unknown";
        // }
        // Debug.Log("Killer: " + killerName);

        // // Incrementa il contatore dei prompt
        // if((killerName != "MyEnemyNew" && ciclesNumber <= 1)  || ciclesNumber <= 1){
        //    incrementCicles(); 
        // }
        
        string prompt = "";
        // Debug.Log("Cicles number: " + ciclesNumber);

        // if(ciclesNumber == 1)
        // {
        //     prompt = prompt1;
        //     Debug.Log("Prompt 1");
        // }
        // else if(killerName == "MyEnemyNew"){
        //     if(ciclesNumber == 2)
        //     {
        //         prompt = prompt2;
        //         Debug.Log("Prompt 2");
        //     }
        //     else if(ciclesNumber == 3)
        //     {
        //         prompt = prompt3;
        //         Debug.Log("Prompt 3");
        //     }
        //     else if(ciclesNumber == 4)
        //     {
        //         prompt = prompt4;
        //         Debug.Log("Prompt 4");
        //     }
        //     prompt += promptBonus;
        //     bonus = true;
        //     Debug.Log("Prompt Bonus aggiunto ");
        // }
        // else
        // {
        //     if(ciclesNumber == 2)
        //     {
        //         prompt = prompt2;
        //         Debug.Log("Prompt 2");
        //     }
        //     else if(ciclesNumber == 3)
        //     {
        //         prompt = prompt3;
        //         Debug.Log("Prompt 3");
        //     }
        //     else if(ciclesNumber == 4)
        //     {
        //         prompt = prompt4;
        //         Debug.Log("Prompt 4");
        //     }   
        // }
        
        switch(eventHandler.globalEventIndex)
        {
            case 0:
                prompt = prompt1;
                break;
            case 1:
                prompt = prompt2;
                break;
            case 2:
                prompt = prompt3;
                break;
            case 3:
                prompt = prompt4;
                break;
            default:
                break;
        }

        return prompt;
    }

    public void incrementCicles(){
        ciclesNumber++;
        Debug.Log("Cicles number: " + ciclesNumber);
    }

    public void resetCicles(){
        ciclesNumber = 0;
        UnityAndGeminiV3.conversationHistory = "";
        Debug.Log("Cicles number reset: " + ciclesNumber);
    }
}
