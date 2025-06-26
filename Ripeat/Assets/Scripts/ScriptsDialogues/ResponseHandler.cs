using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox; // Riferimento al box delle risposte
    [SerializeField] private RectTransform responseButtonTemplate; // Template per i pulsanti delle risposte
    [SerializeField] private RectTransform responseContainer; // Contenitore per i pulsanti delle risposte
    [SerializeField] private float maxResponseBoxHeight = 640f; // Altezza massima del box delle risposte

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents; // Array di eventi di risposta

    private List<GameObject> tempResponseButtons = new List<GameObject>(); // Lista temporanea di pulsanti di risposta
    private ScoreManager scoreManager; // Riferimento allo ScoreManager
    private UnityAndGeminiV3 unityAndGeminiV3; // Riferimento a UnityAndGeminiV3

    public static string history1 = "Utilizzando le parole scelte dal giocatore, descrivi brevemente un inizio di combattimento tra un uomo al bar e un'altra persona casuale, descrivila parlando al protagonista. Il contesto è 'america anni 20'. Utilizza le parole che il giocatore ha scelto per capire la sua indole ma non dirlo. Scrivi solo l'inizio, ovvero quando il nemico apre la porta ed entra al bar. Scrivi pochissime frasi senza descrizioni tra parentesi. Solo quello che accade.\n"; // Storia delle risposte
    public static string history2 = "Continua la frase descrivendo il combattimento. Non scrivere tanto. Frase da continuare: \n"; 
    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>(); // Ottiene il componente DialogueUI
        scoreManager = FindObjectOfType<ScoreManager>(); // Trova lo ScoreManager in scena
        unityAndGeminiV3 = GetComponent<UnityAndGeminiV3>(); // Ottiene il componente UnityAndGeminiV3
        
    }

    public void Restart()
    {
        history1 = "Utilizzando le parole scelte dal giocatore, descrivi brevemente un inizio di combattimento tra un uomo al bar e un'altra persona casuale, descrivila parlando all'uomo. Il contesto è 'america anni 20'. Utilizza le parole che il giocatore ha scelto per capire la sua indole non scriverle subito. Scrivi solo l'inizio, ovvero quando il nemico apre la porta ed entra al bar. Scrivi pochissime frasi.\n"; // Storia delle risposte
        history2 = "Continua la frase descrivendo il combattimento. Non scrivere tanto. Frase da continuare: \n";
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents; // Aggiunge gli eventi di risposta
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0; // Altezza del box delle risposte
        Vector2 initialPos = responseBox.anchoredPosition;

        for(int i = 0; i < responses.Length; i++)
        {
            Response response = responses[i]; // Ottiene la risposta corrente
            int responseIndex = i;

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer); // Istanzia un nuovo pulsante di risposta
            responseButton.gameObject.SetActive(true);
            TMP_Text buttonText = responseButton.GetComponent<TMP_Text>();
            buttonText.text = response.ResponseText; // Imposta il testo del pulsante di risposta
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex, initialPos)); // Aggiunge il listener per il click del pulsante
        
            tempResponseButtons.Add(responseButton); // Aggiunge il pulsante alla lista temporanea

            responseBoxHeight += responseButtonTemplate.sizeDelta.y; // Aggiorna l'altezza del box delle risposte
        }

        // Limita l'altezza del response box
        responseBoxHeight = Mathf.Min(responseBoxHeight, maxResponseBoxHeight);
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight); // Imposta la dimensione del box delle risposte

        // Abilita lo scrolling se l'altezza del contenuto supera l'altezza massima
        ScrollRect scrollRect = responseBox.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.vertical = responseBoxHeight < maxResponseBoxHeight;
        }

        responseBox.anchoredPosition = new Vector2(responseBox.anchoredPosition.x, initialPos.y - (responseBoxHeight - responseButtonTemplate.sizeDelta.y) / 2);
        responseBox.gameObject.SetActive(true); // Mostra il box delle risposte
    }

    private void OnPickedResponse(Response response, int responseIndex, Vector2 initialPos)
    {
        history1 = history1 + response.ResponseText + ", "; // Aggiunge la risposta selezionata alla storia
        
        
        responseBox.gameObject.SetActive(false); // Nasconde il box delle risposte
        
         responseBox.anchoredPosition = initialPos;

        foreach (GameObject button in tempResponseButtons)
        {
            Destroy(button); // Distrugge i pulsanti temporanei
        }
        tempResponseButtons.Clear(); // Pulisce la lista temporanea

        if (responseEvents != null && responseIndex <= responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke(response.PointValue); // Invoca l'evento di risposta se esiste
        }
        if (scoreManager != null)
        {
            scoreManager.AddPoints(response.PointValue); // Aggiunge i punti allo ScoreManager
        }
        else
        {
            Debug.LogWarning("ScoreManager non trovato nella scena."); // Avvisa se lo ScoreManager non è trovato
        }
        dialogueUI.ShowDialogue(response.DialogueObject); // Mostra il nuovo dialogo
    }

    public void SendHistoryToGemini()
    {
        unityAndGeminiV3 = GetComponent<UnityAndGeminiV3>();
        if (unityAndGeminiV3 == null)
        {
            Debug.LogError("UnityAndGeminiV3 non trovato!");
            return;
        }
        StartCoroutine(SendHistoryCoroutine());
    }

    IEnumerator SendHistoryCoroutine()
    {
        // Prima parte
        yield return StartCoroutine(unityAndGeminiV3.SendPromptRequestToGemini(history1, 1));
        history2 = history2 + " " + unityAndGeminiV3.backStory1;
        // Seconda parte
        yield return StartCoroutine(unityAndGeminiV3.SendPromptRequestToGemini(history2, 3));
        Debug.Log("Storia inviata a Gemini");
    }
}
