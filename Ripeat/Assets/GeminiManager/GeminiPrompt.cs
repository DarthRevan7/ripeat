using UnityEngine;

public class GeminiPrompt : MonoBehaviour
{
    [Header("Prompt")]
    [TextArea] public string prompt1 = "";
    [TextArea] public string prompt2 = "";
    [TextArea] public string prompt3 = "";
    [TextArea] public string prompt4 = "";

    public static int ciclesNumber;
    

    void Start()
    {
    
    } 
   
    public string getPrompt(){
        incrementCicles();
        string prompt = "";
        Debug.Log("Cicles number: " + ciclesNumber);
        if(ciclesNumber == 1){
            prompt = prompt1;
            Debug.Log("Prompt 1");
        }
        else if(ciclesNumber == 2){
            prompt = prompt2;
            Debug.Log("Prompt 2");
        }
        else if(ciclesNumber == 3){
            prompt = prompt3;
            Debug.Log("Prompt 3");
        }
        else if(ciclesNumber == 4){
            prompt = prompt4;
            Debug.Log("Prompt 4");
        }
        return prompt;
    }

     public void incrementCicles(){
        ciclesNumber++;
        Debug.Log("Cicles number: " + ciclesNumber);
    }
}
