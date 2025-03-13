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

    private void Start()
    {
        dialogueUI = GetComponent<DialogueUI>(); // Ottiene il componente DialogueUI
        scoreManager = FindObjectOfType<ScoreManager>(); // Trova lo ScoreManager in scena
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
}
