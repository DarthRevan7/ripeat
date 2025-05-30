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
    [SerializeField] private GameObject clockLight1;
    [SerializeField] private GameObject clockLight2;
    [SerializeField] private GameObject clockLight3;
    [SerializeField] private GameObject clockLight4;
    [SerializeField] private GameObject clockLight5;
    [SerializeField] private GameObject clockLight6;
    [SerializeField] private GameObject clockLight7;
    [SerializeField] private GameObject clockLight8;
    [SerializeField] private GameObject clockLight9;
    [SerializeField] private GameObject clockLight10;
    [SerializeField] private GameObject clockLight11;
    [SerializeField] private GameObject clockLight12;
    [SerializeField] private GameObject clockLight13;
    [SerializeField] private GameObject clockLight14;
  
    [SerializeField] private GameObject death1;
    [SerializeField] private GameObject death2;
    [SerializeField] private GameObject death3;

    // Aggiungi questo campo nella parte iniziale della classe, ad esempio dopo i campi già esistenti
    public MenuScript menuScript;
    private TypewriterEffect typewriterEffect;
    private GeminiPrompt geminiPrompt;
    private DialogueUI dialogueUI;

    // Memorizza la cronologia della conversazione (parte fissa con il prompt iniziale + messaggi successivi)
    public static string conversationHistory;
    private string prompt = "";
    private string feedback = "";
    private string messages = "";
    public static string backStory = "";
    
    private int counter = 1;
    private int sum = 0;

    IEnumerator Start()
    {
        if (clock1 != null)
        {
            Debug.Log("Clock1 found, starting with it active.");
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
            clockLight1.SetActive(false);
            clockLight2.SetActive(false);
            clockLight3.SetActive(false);
            clockLight4.SetActive(false);
            clockLight5.SetActive(false);
            clockLight6.SetActive(false);
            clockLight7.SetActive(false);
            clockLight8.SetActive(false);
            clockLight9.SetActive(false);
            clockLight10.SetActive(false);
            clockLight11.SetActive(false);
            clockLight12.SetActive(false);
            clockLight13.SetActive(false);
            clockLight14.SetActive(false);
            yield return StartCoroutine(ChangeClock(0));
        }
        if (death1 != null)
        {
            death2.SetActive(false);
            death3.SetActive(false);
            death1.SetActive(true);
        }

        UnityAndGeminiKey jsonApiKey = JsonUtility.FromJson<UnityAndGeminiKey>(jsonApi.text);
        apiKey = jsonApiKey.key;
        typewriterEffect = GetComponent<TypewriterEffect>();
        geminiPrompt = GetComponent<GeminiPrompt>();
        dialogueUI = GetComponent<DialogueUI>();
        Debug.Log("Tutto ok");
        //conversationHistory += "PROMPT: " + testPrompt;
        if(isDead){
            prompt = "NON SCRIVERE MAI questo simbolo %. \n";
            prompt += geminiPrompt.getPrompt();
            prompt += "\nBackstory dell' anima: " + backStory + "\n";
            
            conversationHistory = "PROMPT: " + prompt + "\n\nCronologia della conversazione:";
            Debug.Log("Prompt preso: " + prompt);
            StartCoroutine(SendPromptRequestToGemini(prompt, 0));
            feedback = "Tu sei la morte. Giudica la risposta con un voto da 1 a 3 dove 1 vuol dire che l'anima è meritevole e 3 non meritevole. Scrivi solo il numero.\n";
        }
    
        if(inputField != null)
            inputField.onSubmit.AddListener((string text) => { SendChat(); });
    }

    void Update()
    {
        // Check if the Tab key is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene("CombatScene");
        }
    }

    public IEnumerator SendPromptRequestToGemini(string promptText, int type)
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
                Debug.LogError("Prompt Request Error: " + www.error + ", Response Code: " + www.responseCode);
                Debug.LogError("Prompt Request Error: " + www.error);
            }
            else
            {
                Debug.Log("Prompt request complete!");
                Risposta response = JsonUtility.FromJson<Risposta>(www.downloadHandler.text);
                if (response != null && response.candidates != null && response.candidates.Length > 0 &&
                    response.candidates[0].content.parts != null && response.candidates[0].content.parts.Length > 0)
                {
                    switch(type)
                    {
                        case 0:
                            string text0 = response.candidates[0].content.parts[0].text;
                            Debug.Log("\nMorte: " + text0);
                            conversationHistory += "\n%" + text0 + "%\n";
                            yield return new WaitForSeconds(0.5f);
                            if (uiText != null)
                            {
                                uiText.text = text0;
                                uiText.color = new Color32(36, 36, 36, 255);
                            }
                            yield return StartCoroutine(AdjustTextBoxSize());
                            yield return RunTypingEffect(text0);
                            break;
                        case 1:
                            string text1 = response.candidates[0].content.parts[0].text;
                            backStory = text1;
                            Debug.Log("\nText: " + text1);
                            StartCoroutine(dialogueUI.ShowFinalString(text1));
                            break;
                        case 2:
                            string text2 = response.candidates[0].content.parts[0].text;
                            int n = int.Parse(text2);
                            sum += n;
                            Debug.Log("Sum: " + sum);
                            if (text2.Contains("1"))
                            {
                                Debug.Log("Risposta 1");
                                death2.SetActive(false);
                                death3.SetActive(false);
                                death1.SetActive(true);
                            }
                            else if (text2.Contains("2"))
                            {
                                Debug.Log("Risposta 2");
                                death1.SetActive(false);
                                death3.SetActive(false);
                                death2.SetActive(true);
                            }
                            else if (text2.Contains("3"))
                            {
                                Debug.Log("Risposta 3");
                                death1.SetActive(false);
                                death2.SetActive(false);
                                death3.SetActive(true);
                            }
                            else
                            {
                                Debug.Log("Risposta non valida: " + text2);
                            }
                            break;
                        default:
                            break;
                    }      

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
        messages += userMessage + "\n";
        feedback += messages;;
        conversationHistory += "\n" + userMessage + "\n";
        counter++;
        StartCoroutine(ChangeClock(counter));
        Debug.Log("Counter: " + counter);

        if (counter >= 7)
        {
            string finalRequest = "";
            if (sum < 15)
            {
                finalRequest = "\nPROMPT: Ora scrivi HAI UN'ALTRA POSSIBILITA'\n";
            }
            else
            {
                finalRequest = "\nPROMPT: Ora scrivi NON AVRAI ALTRE POSSIBILITA'!\n";
            }
            StartCoroutine(SendChatRequestToGemini(finalRequest));
        }
        else
        {
            StartCoroutine(SendChatRequestToGemini(conversationHistory));
        }

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

                    Debug.Log("Feedback: " + feedback);
                    StartCoroutine(SendPromptRequestToGemini(feedback, 2));

                    feedback = "Giudica la risposta con un voto da 1 a 3 dove 1 vuol dire che l'anima è meritevole e 3 non meritevole. Considera anche il senso della frase. Se scrive solo una parola senza senso o ripete sempre la stessa allora non è meritevole! Scrivi solo il numero.\n";

                    yield return StartCoroutine(AdjustTextBoxSize());
                    yield return RunTypingEffect(reply);
                    // Appena impostato il testo:
                    //uiText.text = reply
                    // Aggiorna la cronologia aggiungendo anche la risposta del modello
                    conversationHistory += "\n%" + reply + "%\n";

                    // Controllo sull'output di Gemini
                    if (reply.Contains("NON AVRAI ALTRE POSSIBILITA'") || reply.Contains("Non avrai altre possibilità"))
                    {

                        PLBox.SetActive(false);
                        yield return new WaitForSeconds(8f);
                        ShowNegativeFinalImage();
                        yield return new WaitForSeconds(3f);
                        geminiPrompt.resetCicles();
                        SceneManager.LoadScene("Menu");



                    }
                    else if (reply.Contains("HAI UN'ALTRA POSSIBILITA'") || reply.Contains("Hai un'altra possibilità"))
                    {

                        PLBox.SetActive(false);
                        yield return new WaitForSeconds(8f);
                        if (FightEventController.globalEventIndex > 3)
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
                
                    
                }
                else
                {
                    Debug.Log("No text found in chat response.");
                }
            }
        }
    }

    IEnumerator ChangeClock(int count){
        switch (count)
        {
            case 0:
                clock1.SetActive(true);
                
                Debug.Log("Clock activated");
                
                break;

            case 2:
                clock2.SetActive(true);

                clockLight2.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight2.SetActive(false);

                break;
            case 3:
                clock3.SetActive(true);

                clockLight3.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight3.SetActive(false);

                break;
            case 4:
                clock4.SetActive(true);

                clockLight4.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight4.SetActive(false);

                break;
            case 5:
                clock5.SetActive(true);

                clockLight5.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight5.SetActive(false);

                break;
            case 6:
                clock6.SetActive(true);

                clockLight6.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight6.SetActive(false);

                break;
            case 7:
                clock7.SetActive(true);

                clockLight7.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight7.SetActive(false);

                break;
            case 8:
                clock8.SetActive(true);

                clockLight8.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight8.SetActive(false);

                break;
            case 9:
                clock9.SetActive(true);

                clockLight9.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight9.SetActive(false);

                break;
            case 10:
                clock10.SetActive(true);

                clockLight10.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight10.SetActive(false);

                break;
            case 11:
                clock11.SetActive(true);

                clockLight11.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight11.SetActive(false);

                break;
            case 12:
                clock12.SetActive(true);

                clockLight12.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight12.SetActive(false);

                break;
            case 13:
                clock13.SetActive(true);

                clockLight13.SetActive(true);
                yield return new WaitForSeconds(1f);
                clockLight13.SetActive(false);

                break;
        }
    }
    IEnumerator AdjustTextBoxSize()
    {
    yield return new WaitForEndOfFrame();
    if(uiText != null && AIBox != null) {
        uiText.ForceMeshUpdate();
        RectTransform rt = AIBox.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, uiText.preferredHeight+50);
        Debug.Log("Box size adjusted with: " + uiText.preferredHeight);
    }
    
    }
    IEnumerator RunTypingEffect(string text)
    {
        if (uiText != null)
        {
            uiText.color = new Color32(255, 255, 255, 255);
            typewriterEffect.Run(text, uiText);
            while (typewriterEffect.IsRunning)
            {
                yield return null;
            }
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




