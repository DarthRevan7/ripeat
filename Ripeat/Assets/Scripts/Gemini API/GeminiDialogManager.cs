using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using GeminiAPI;

public class GeminiDialogManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;
    public TMP_Text uiOutputText;
    public GameObject playerInputBox;

    [Header("Visual Effects (Clocks)")]
    public GameObject[] clockObjects; // Trascina qui tutti i clock in ordine!
    public GameObject[] clockLights;  // Trascina qui tutte le luci in ordine!

    [Header("Game Logic")]
    [TextArea] public string basePrompt;
    private int currentTurn = 0;
    private string conversationHistory = "";

    // Metodo per avviare la conversazione (es. chiamato dal Barista)
    public void StartConversation(string soulName)
    {
        conversationHistory = $"Il nome dell'anima è: {soulName}\n\n {basePrompt}";
        ExecuteRequest(conversationHistory);
    }

    public void OnSubmitText()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;
        
        string userText = inputField.text;
        inputField.text = "";
        conversationHistory += $"\nUtente: {userText}\n";
        
        UpdateClocks();
        ExecuteRequest(conversationHistory);
    }

    private void ExecuteRequest(string fullPrompt)
    {
        StartCoroutine(GeminiRequester.SendRequest(fullPrompt, 
            (response) => {
                conversationHistory += $"\nAI: {response}\n";
                // Qui chiami il tuo effetto macchina da scrivere
                uiOutputText.text = response; 
                Debug.Log("Gemini ha risposto!");
            },
            (errorCode) => {
                if (errorCode == 429) uiOutputText.text = "Sistema sovraccarico, riprova tra un istante...";
                if (errorCode == 404) uiOutputText.text = "Errore di connessione (Endpoint errato).";
            }
        ));
    }

    private void UpdateClocks()
    {
        currentTurn++;
        if (currentTurn < clockObjects.Length)
        {
            clockObjects[currentTurn].SetActive(true);
            StartCoroutine(FlashLight(currentTurn));
        }
    }

    IEnumerator FlashLight(int index)
    {
        if (index < clockLights.Length)
        {
            clockLights[index].SetActive(true);
            yield return new WaitForSeconds(1f);
            clockLights[index].SetActive(false);
        }
    }
}