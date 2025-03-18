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
    
    [Header("Prompt Function")]
    [TextArea] public string prompt = "";

    // Memorizza la cronologia della conversazione (parte fissa con il prompt iniziale + messaggi successivi)
    private string conversationHistory;

    void Start()
    {
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;

        // Imposta la cronologia iniziale con il prompt
        conversationHistory = prompt;
        // Invia la prima richiesta (opzionale) per impostare il contesto
        StartCoroutine(SendPromptRequestToGemini(prompt));
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
                    Debug.Log("Prompt Response: " + text);
                    uiText.text = text;
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
        conversationHistory += "\nUser: " + userMessage;
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
                    // Aggiorna la cronologia aggiungendo anche la risposta del modello
                    conversationHistory += "\nBot: " + reply;
                    
                    // Controllo sull'output di Gemini
                    if(reply.Contains("BASTA LA TUA VITA FINISCE QUI."))
                    {
                        // Esempio: se la risposta contiene "action1keyword", esegue l'azione 1.
                        yield return new WaitForSeconds(6f);
                        SceneManager.LoadScene("Menu");
                        Debug.Log("Eseguo Azione 1");
                        // Inserisci qui il codice dell'azione 1
                    }
                    else if(reply.Contains("HAI UN'ALTRA POSSIBILITA'"))
                    {
                        yield return new WaitForSeconds(6f);
                        SceneManager.LoadScene("FightingScene_Try");
                        Debug.Log("Eseguo Azione 2");
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
}


