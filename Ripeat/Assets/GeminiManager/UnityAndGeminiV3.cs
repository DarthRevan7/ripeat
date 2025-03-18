using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;

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


public class UnityAndGeminiV3: MonoBehaviour
{
    [Header("JSON API Configuration")]
    public TextAsset jsonApi;
    private string apiKey = ""; 
    private string apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent"; // Edit it and choose your prefer model


    [Header("ChatBot Function")]
    public TMP_InputField inputField;
    public TMP_Text uiText;
    private Content[] chatHistory;

    [Header("Prompt Function")]
    [TextArea] public string prompt = "";



    void Start()
    {
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;   
        chatHistory = new Content[] { };
        StartCoroutine( SendPromptRequestToGemini(prompt));        
    }

    private IEnumerator SendPromptRequestToGemini(string promptText)
    {
        string url = $"{apiEndpoint}?key={apiKey}";
        // Costruisce il JSON senza le graffe in eccesso
        string jsonData = "{\"contents\": [{\"parts\": [{\"text\": \"" + promptText + "\"}]}]}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Request complete!");
                Risposta response = JsonUtility.FromJson<Risposta>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                {
                    // Questa è la risposta iniziale di Gemini
                    string text = response.candidates[0].content.parts[0].text;
                    Debug.Log(text);
                    // Mostra la risposta nel canvas
                    uiText.text = text;
                    
                    // (Opzionale) Aggiorna la cronologia della chat se necessario
                    List<Content> history = new List<Content>(chatHistory);
                    Content botContent = new Content
                    {
                        role = "death",
                        parts = new Part[]
                        {
                            new Part { text = text }
                        }
                    };
                    history.Add(botContent);
                    chatHistory = history.ToArray();
                }
                else
                {
                    Debug.Log("No text found.");
                }
            }
        }
    }

    public void enterChat()
    {
        if(inputField.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userMessage = inputField.text;
            StartCoroutine( SendChatRequestToGemini(inputField.text));
        }
    }
    public void SendChat()
    {
        string userMessage = inputField.text;
        StartCoroutine( SendChatRequestToGemini(userMessage));
    }

    private IEnumerator SendChatRequestToGemini(string newMessage)
    {

        string url = $"{apiEndpoint}?key={apiKey}";
     
        Content userContent = new Content
        {
            role = "user",
            parts = new Part[]
            {
                new Part { text = newMessage }
            }
        };

        List<Content> contentsList = new List<Content>(chatHistory);
        contentsList.Add(userContent);
        chatHistory = contentsList.ToArray(); 

        ChatRequest chatRequest = new ChatRequest { contents = chatHistory };

        string jsonData = JsonUtility.ToJson(chatRequest);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Create a UnityWebRequest with the JSON data
        using (UnityWebRequest www = new UnityWebRequest(url, "POST")){
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.LogError(www.error);
            } else {
                Debug.Log("Request complete!");
                Risposta response = JsonUtility.FromJson<Risposta>(www.downloadHandler.text);
                if (response.candidates.Length > 0 && response.candidates[0].content.parts.Length > 0)
                    {
                        //This is the response to your request
                        string reply = response.candidates[0].content.parts[0].text;
                        Content botContent = new Content
                        {
                            role = "death",
                            parts = new Part[]
                            {
                                new Part { text = reply }
                            }
                        };

                        Debug.Log(reply);
                        //This part shows the text in the Canvas
                        uiText.text = reply;
                        //This part adds the response to the chat history, for your next message
                        contentsList.Add(botContent);
                        chatHistory = contentsList.ToArray();
                    }
                else
                {
                    Debug.Log("No text found.");
                }
             }
        }  
    }
}


