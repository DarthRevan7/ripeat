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
    [SerializeField] private bool isDead = false;
    [SerializeField] private GameObject negativeFinalImage;
    [SerializeField] private GameObject positiveFinalImage;
    [SerializeField] private GameObject clock1;
    [SerializeField] private GameObject clock2;
    [SerializeField] private GameObject clock3;
    [SerializeField] private GameObject clock4;
    [SerializeField] private GameObject clock5;
    [SerializeField] private GameObject clock6;
    [SerializeField] private GameObject clock7;
    [SerializeField] private GameObject clock8;
    [SerializeField] private GameObject clock9;
    [SerializeField] private GameObject clock10;
    [SerializeField] private GameObject clock11;
    [SerializeField] private GameObject clock12;
    [SerializeField] private GameObject clock13;

    // Aggiungi questo campo nella parte iniziale della classe, ad esempio dopo i campi già esistenti
    public MenuScript menuScript;
    private TypewriterEffect typewriterEffect;
    private GeminiPrompt geminiPrompt;
    private DialogueUI dialogueUI;

    // Memorizza la cronologia della conversazione (parte fissa con il prompt iniziale + messaggi successivi)
    public static string conversationHistory;
    private string prompt = "";
    private int counter = 1;

    void Start()
    {
        if (clock1 != null)
        {
            ChangeClock(0);
        }
        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;
        typewriterEffect = GetComponent<TypewriterEffect>();
        geminiPrompt = GetComponent<GeminiPrompt>();
        dialogueUI = GetComponent<DialogueUI>();
        Debug.Log("Tutto ok");
        //conversationHistory += "PROMPT: " + testPrompt;
        if(isDead){
            prompt += "\nSe scrivo 001100 allora scrivi HAI UN'ALTRA POSSIBILITA'.\n";
            prompt = geminiPrompt.getPrompt();
            
            conversationHistory += "\nPROMPT: " + prompt;
            Debug.Log("Prompt preso: " + prompt);
            StartCoroutine(SendPromptRequestToGemini(prompt, true));
        }
    
        if(inputField != null)
            inputField.onSubmit.AddListener((string text) => { SendChat(); });
    }

    public IEnumerator SendPromptRequestToGemini(string promptText, bool dead)
    {
        Debug.Log("Prompt Request: " + promptText);
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
                    if(dead)
                    {
                        string text = response.candidates[0].content.parts[0].text;
                        Debug.Log("\nMorte: " + text);
                        conversationHistory += "\n%" + text + "%\n";
                        yield return new WaitForSeconds(0.5f);
                        uiText.text = text;
                        uiText.color = new Color32(36, 36, 36, 255);
                        yield return StartCoroutine(AdjustTextBoxSize());
                        yield return RunTypingEffect(text);
                    }
                    else
                    { 
                        string text = response.candidates[0].content.parts[0].text;
                        Debug.Log("\nText: " + text);
                        StartCoroutine(dialogueUI.ShowFinalString(text));

                    }
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
            userMessage = "...";

        // Aggiorna la cronologia con il messaggio utente
        conversationHistory += "\n" + userMessage;
        counter++;
        ChangeClock(counter);
        Debug.Log("Counter: " + counter);
        if (counter >= 13)
        {
            conversationHistory += "\nPROMPT: Ora decidi cosa fare ma non essere troppo cattivo: scrivi BASTA LA TUA VITA FINISCE QUI se pensi che non sia meritevole, oppure HAI UN'ALTRA POSSIBILITA' se pensi che sia meritevole! Solo una di queste frasi e nient'altro!!\n";
        }
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
                    conversationHistory += "\n%" + reply + "%\n";
                    
                    // Controllo sull'output di Gemini
                    if(reply.Contains("BASTA LA TUA VITA FINISCE QUI")||reply.Contains("Basta la tua vita finisce qui"))
                    {
                        
                        PLBox.SetActive(false);
                        yield return new WaitForSeconds(8f);
                        ShowNegativeFinalImage();
                        yield return new WaitForSeconds(3f);
                        geminiPrompt.resetCicles();
                        SceneManager.LoadScene("Menu");
                        
                        
                        
                    }
                    else if(reply.Contains("HAI UN'ALTRA POSSIBILITA'")||reply.Contains("Hai un'altra possibilità"))
                    {
                        
                        PLBox.SetActive(false);
                        yield return new WaitForSeconds(8f);
                        if(FightEventController.globalEventIndex > 2)
                        {
                            ShowPositiveFinalImage();
                            yield return new WaitForSeconds(3f);
                            geminiPrompt.resetCicles();
                            SceneManager.LoadScene("Menu");
                        }
                        else 
                        {
                            SceneManager.LoadScene("CombatScene");  
                        }
                        
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

    public void ChangeClock(int count){
        switch (count)
        {
            case 0:
                clock1.SetActive(true);
                clock2.SetActive(false);
                clock3.SetActive(false);
                clock4.SetActive(false);
                clock5.SetActive(false);
                clock6.SetActive(false);
                clock7.SetActive(false);
                clock8.SetActive(false);
                clock9.SetActive(false);
                clock10.SetActive(false);
                clock11.SetActive(false);
                clock12.SetActive(false);
                clock13.SetActive(false);
                
                break;

            case 2:
                clock2.SetActive(true);
                break;
            case 3:
                clock3.SetActive(true);
                break;
            case 4:
                clock4.SetActive(true);
                break;
            case 5:
                clock5.SetActive(true);
                break;
            case 6:
                clock6.SetActive(true);
                break;
            case 7:
                clock7.SetActive(true);
                break;
            case 8:
                clock8.SetActive(true);
                break;
            case 9:
                clock9.SetActive(true);
                break;
            case 10:
                clock10.SetActive(true);
                break;
            case 11:
                clock11.SetActive(true);
                break;
            case 12:
                clock12.SetActive(true);
                break;
            case 13:
                clock13.SetActive(true);
                break;
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

    private void ShowPositiveFinalImage()
    {
        if (positiveFinalImage != null)
        {
            positiveFinalImage.SetActive(false);
        }
    }

}




