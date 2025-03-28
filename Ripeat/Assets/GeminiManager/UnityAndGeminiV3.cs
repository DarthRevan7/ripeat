using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UnityAndGeminiKey
{
    public string key;
}

[System.Serializable]
public class Risposta
{
    public Candidate[] candidates;
}

[System.Serializable]
public class ChatRequest
{
    public Content[] contents;
}

[System.Serializable]
public class Candidate
{
    public Content content;
}

[System.Serializable]
public class Content
{
    public string role;
    public Part[] parts;
}

[System.Serializable]
public class Part
{
    public string text;
}

public class UnityAndGeminiV3 : MonoBehaviour
{
    [Header("JSON API Configuration")]
    public TextAsset jsonApi;
    private string apiKey = "";
    private string apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";

    [Header("ChatBot Function")]
    public TMP_InputField inputField;
    public TMP_Text uiText;
    public GameObject AIBox;
    public GameObject PLBox;
    
    [Header("Prompt Function")]
    [TextArea] public string testPrompt = "";
    [SerializeField] private GameObject negativeFinalImage;

    // Aggiungi questo campo nella parte iniziale della classe, ad esempio dopo i campi già esistenti
    public MenuScript menuScript;
    private TypewriterEffect typewriterEffect;
    private GeminiPrompt geminiPrompt;

    // Memorizza la cronologia della conversazione (parte fissa con il prompt iniziale + messaggi successivi)
    private static string conversationHistory;
    private string prompt = "";
    void Start()
    {
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;
        typewriterEffect = GetComponent<TypewriterEffect>();
        geminiPrompt = GetComponent<GeminiPrompt>();
        //conversationHistory += "PROMPT: " + testPrompt;
        
        prompt = geminiPrompt.getPrompt();
        conversationHistory += "\nPROMPT: " + prompt;
        Debug.Log("Prompt preso: " + prompt);
        StartCoroutine(SendPromptRequestToGemini(prompt));
        
        
        if(inputField != null)
            inputField.onSubmit.AddListener((string text) => { SendChat(); });
    }

    private IEnumerator SendPromptRequestToGemini(string promptText)
    {
        string url = $"{apiEndpoint}?key={apiKey}";
        // Invia soltanto il prompt iniziale
        string jsonData = "{\"contents\": [{\"parts\": [{\"text\": \"" + EscapeJson(promptText) + "\"}]}]}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Prompt Request Error: " + www.error);
            }
            else
            {
                Debug.Log("Prompt request complete!");
                Risposta response = JsonUtility.FromJson<Risposta>(www.downloadHandler.text);
                if (response != null && response.candidates != null && response.candidates.Length > 0 &&
                    response.candidates[0].content.parts != null && response.candidates[0].content.parts.Length > 0)
                {
                    string text = response.candidates[0].content.parts[0].text;
                    Debug.Log("\nMorte: " + text);
                    conversationHistory += "\nMorte: " + text;
                    yield return new WaitForSeconds(0.5f);
                    uiText.text = text;
                    uiText.color = new Color32(36, 36, 36, 255);
                    yield return StartCoroutine(AdjustTextBoxSize());
                    yield return RunTypingEffect(text);
                    
                    
                }
                else
                {
                    Debug.Log("No text found in prompt response.");
                }
            }
        }
    }

    private string EscapeJson(string text)
    {
        return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    public void SendChat()
    {
        string userMessage = inputField.text;
        if (string.IsNullOrEmpty(userMessage))
            return;

        // Aggiorna la cronologia con il messaggio utente
        conversationHistory += "\nAnima: " + userMessage;
        StartCoroutine(SendChatRequestToGemini(conversationHistory));
        inputField.text = "";
    }

    private IEnumerator SendChatRequestToGemini(string compositeMessage)
    {
        string url = $"{apiEndpoint}?key={apiKey}";

        // Costruisci il JSON con la cronologia completa
        string jsonData = "{\"contents\": [{\"parts\": [{\"text\": \"" + EscapeJson(compositeMessage) + "\"}]}]}";
        Debug.Log("Chat Request JSON: " + jsonData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Chat Request Error: " + www.error);
            }
            else
            {
                Debug.Log("Chat request complete!");
                Risposta response = JsonUtility.FromJson<Risposta>(www.downloadHandler.text);
                if (response != null && response.candidates != null && response.candidates.Length > 0 &&
                    response.candidates[0].content.parts != null && response.candidates[0].content.parts.Length > 0)
                {
                    string reply = response.candidates[0].content.parts[0].text;
                    Debug.Log("Death Reply: " + reply);
                    uiText.text = reply;
                    uiText.color = new Color32(36, 36, 36, 255);
                    yield return StartCoroutine(AdjustTextBoxSize());
                    yield return RunTypingEffect(reply);
                    // Appena impostato il testo:
                    //uiText.text = reply
                    // Aggiorna la cronologia aggiungendo anche la risposta del modello
                    conversationHistory += "\nMorte: " + reply;
                    
                    // Controllo sull'output di Gemini
                    if(reply.Contains("BASTA LA TUA VITA FINISCE QUI"))
                    {
                        
                        PLBox.SetActive(false);
                        yield return new WaitForSeconds(8f);
                        ShowNegativeFinalImage();
                        yield return new WaitForSeconds(3f);
                        geminiPrompt.resetCicles();
                        SceneManager.LoadScene("Menu");
                        
                    }
                    else if(reply.Contains("HAI UN'ALTRA POSSIBILITA'"))
                    {
                        PLBox.SetActive(false);
                        yield return new WaitForSeconds(8f);
                        SceneManager.LoadScene("FightingScene_Try");
                        
                    }
                    else
                    {
                        Debug.Log("Output non rilevante, nessuna azione eseguita.");
                    }
                }
                else
                {
                    Debug.Log("No text found in chat response.");
                }
            }
        }
    }

    IEnumerator AdjustTextBoxSize()
    {
    yield return new WaitForEndOfFrame();
    uiText.ForceMeshUpdate();
    RectTransform rt = AIBox.GetComponent<RectTransform>();
    rt.sizeDelta = new Vector2(rt.sizeDelta.x, uiText.preferredHeight+50);
    Debug.Log("Box size adjusted with: " + uiText.preferredHeight);
    }
    IEnumerator RunTypingEffect(string text)
    {
        uiText.color = new Color32(255, 255, 255, 255);
        typewriterEffect.Run(text, uiText);
        while (typewriterEffect.IsRunning)
        {
            yield return null;
        }    
    }

    private void ShowNegativeFinalImage()
    {
        
        if (negativeFinalImage != null)
        {
            negativeFinalImage.SetActive(true);
        }
    }

}




