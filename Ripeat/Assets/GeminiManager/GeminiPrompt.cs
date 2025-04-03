using UnityEngine;

public class GeminiPrompt : MonoBehaviour
{
    [Header("Prompt")]
    [TextArea] public string prompt1 = "";
    [TextArea] public string prompt2 = "";
    [TextArea] public string prompt3 = "";
    [TextArea] public string prompt4 = "";
    
    public static int ciclesNumber;

    // Aggiungiamo i riferimenti necessari per leggere lastKiller
    private FighterStats fighterStats;
    private string killerName = "";

    void Start()
    {
        // Prova a cercare il componente FighterStats sullo stesso GameObject
        fighterStats = GetComponent<FighterStats>();
        if(fighterStats == null)
        {
            Debug.LogWarning("FighterStats non trovato su " + gameObject.name);
        }
    }
   
    public string getPrompt()
    {
        // Legge la variabile lastKiller dal componente FighterStats (se presente)
        if(fighterStats != null)
        {
            killerName = fighterStats.lastKiller;
        }
        else
        {
            killerName = "unknown";
        }
        Debug.Log("Killer: " + killerName);

        // Incrementa il contatore dei prompt
        incrementCicles();
        string prompt = "";
        Debug.Log("Cicles number: " + ciclesNumber);
        if(ciclesNumber == 1)
        {
            prompt = prompt1;
            Debug.Log("Prompt 1");
        }
        else if(ciclesNumber == 2)
        {
            prompt = prompt2;
            Debug.Log("Prompt 2");
        }
        else if(ciclesNumber == 3)
        {
            prompt = prompt3;
            Debug.Log("Prompt 3");
        }
        else if(ciclesNumber == 4)
        {
            prompt = prompt4;
            Debug.Log("Prompt 4");
        }
        
        // Ad esempio, concateno anche il killer al prompt
        prompt += "\nKilled by: " + killerName;
        return prompt;
    }

    public void incrementCicles(){
        ciclesNumber++;
        Debug.Log("Cicles number: " + ciclesNumber);
    }

    public void resetCicles(){
        ciclesNumber = 0;
        Debug.Log("Cicles number reset: " + ciclesNumber);
    }
}
