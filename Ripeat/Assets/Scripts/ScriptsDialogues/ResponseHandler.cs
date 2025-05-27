using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox; // Riferimento al box delle risposte
    [SerializeField] private RectTransform responseButtonTemplate; // Template per i pulsanti delle risposte
    [SerializeField] private RectTransform responseContainer; // Contenitore per i pulsanti delle risposte

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents; // Array di eventi di risposta

    private List<GameObject> tempResponseButtons = new List<GameObject>(); // Lista temporanea di pulsanti di risposta
    private ScoreManager scoreManager; // Riferimento allo ScoreManager
    private UnityAndGeminiV3 unityAndGeminiV3; // Riferimento a UnityAndGeminiV3

    public static string history = "Utilizzando le parole scelte dal giocatore, descrivi brevemente un inizio di combattimento tra un uomo al bar e un'altra persona. Utilizza le parole che il giocatore ha scelto per capire la sua indole. Stampa solo la breve descrizione dando del tu all'uomo al barista senza niente altro.\n "; // Storia delle risposte selezionate

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>(); // Ottiene il componente DialogueUI
        scoreManager = FindObjectOfType<ScoreManager>(); // Trova lo ScoreManager in scena
        unityAndGeminiV3 = GetComponent<UnityAndGeminiV3>(); // Ottiene il componente UnityAndGeminiV3
        
    }

    public void Restart()
    {
        history = "Utilizzando le parole scelte dal giocatore, descrivi brevemente un inizio di combattimento tra un uomo al bar e un'altra persona casuale. Il contesto è 'america anni 20'. Utilizza le parole che il giocatore ha scelto per capire la sua indole. Stampa solo la breve descrizione spiegando cosa succede nel bar senza niente altro.\n"; // Inizializza la storia delle risposte
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
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText; // Imposta il testo del pulsante di risposta
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex, initialPos)); // Aggiunge il listener per il click del pulsante
        
            tempResponseButtons.Add(responseButton); // Aggiunge il pulsante alla lista temporanea

            responseBoxHeight += responseButtonTemplate.sizeDelta.y; // Aggiorna l'altezza del box delle risposte
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight); // Imposta la dimensione del box delle risposte
        responseBox.anchoredPosition = new Vector2(responseBox.anchoredPosition.x, responseBox.anchoredPosition.y - (responseBoxHeight-responseButtonTemplate.sizeDelta.y));
        responseBox.gameObject.SetActive(true); // Mostra il box delle risposte
    }

    private void OnPickedResponse(Response response, int responseIndex, Vector2 initialPos)
    {
        history = history + response.ResponseText + ", "; // Aggiunge la risposta selezionata alla storia
        Debug.Log(history); // Stampa la storia delle risposte selezionate
        
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
        unityAndGeminiV3 = GetComponent<UnityAndGeminiV3>(); // Ottiene il componente UnityAndGeminiV3
        if (unityAndGeminiV3 == null)
        {
            Debug.LogError("UnityAndGeminiV3 non trovato!"); // Avvisa se UnityAndGeminiV3 non è trovato
            return;
        }
        StartCoroutine(unityAndGeminiV3.SendPromptRequestToGemini(history, 1)); // Invia la storia a Gemini
        Debug.Log("Storia inviata a Gemini: " + history);
    }
}
