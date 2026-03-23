using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using GeminiAPI;


public class ResponseHandlerV2 : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox; // Riferimento al box delle risposte
    [SerializeField] private RectTransform responseButtonTemplate; // Template per i pulsanti delle risposte
    [SerializeField] private RectTransform responseContainer; // Contenitore per i pulsanti delle risposte
    [SerializeField] private float maxResponseBoxHeight = 640f; // Altezza massima del box delle risposte

    private DialogueUI dialogueUI;
    private ResponseEvent[] responseEvents; // Array di eventi di risposta

    private List<GameObject> tempResponseButtons = new List<GameObject>(); // Lista temporanea di pulsanti di risposta
    private ScoreManager scoreManager; // Riferimento allo ScoreManager
    
    public static string history = "Utilizzando le parole scelte dal giocatore, descrivi brevemente un inizio di combattimento tra un uomo al bar e un'altra persona casuale, descrivila parlando all'uomo. Il contesto è 'america anni 20'. Utilizza le parole che il giocatore ha scelto per capire la sua indole. Stampa solo la breve descrizione spiegando cosa succede nel bar senza niente altro.\n"; // Storia delle risposte

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>(); // Ottiene il componente DialogueUI
        scoreManager = FindObjectOfType<ScoreManager>(); // Trova lo ScoreManager in scena
        
        
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

        // 1. Controllo di sicurezza: il Core deve essere attivo
        if (GeminiCore.Instance == null)
        {
            Debug.LogError("ERRORE: GeminiCore non trovato in scena! L'API non può partire.");
            return;
        }

        Debug.Log("<color=green>Ponte API attivato:</color> Invio storia all'endpoint Gemini: " + GeminiCore.Instance.apiEndpoint);

        // 2. Chiamata al nuovo Requester (molto più pulito!)
        StartCoroutine(GeminiRequester.SendRequest(history, 
            (response) => {
                // SUCCESS: Gemini ha risposto correttamente
                Debug.Log("Gemini ha generato la scena di combattimento!");
                
                if (dialogueUI != null)
                {
                    // Mostriamo la risposta finale usando la tua logica esistente
                    StartCoroutine(dialogueUI.ShowFinalString(response)); 
                }
            },
            (errorCode) => {
                // ERROR: Gestione pulita dei codici errore (404, 429, 503)
                Debug.LogError($"Errore API Gemini: {errorCode}. Controlla la chiave o l'URL nel Core.");
            }
        ));

        // unityAndGeminiV3 = GetComponent<UnityAndGeminiV3>(); // Ottiene il componente UnityAndGeminiV3
        // if (unityAndGeminiV3 == null)
        // {
        //     Debug.LogError("UnityAndGeminiV3 non trovato!"); // Avvisa se UnityAndGeminiV3 non è trovato
        //     return;
        // }
        // StartCoroutine(unityAndGeminiV3.SendPromptRequestToGemini(history, 1)); // Invia la storia a Gemini
        // Debug.Log("Storia inviata a Gemini: " + history);
    }
}
