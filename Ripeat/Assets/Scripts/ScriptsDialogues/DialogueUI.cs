using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private TMP_Text finalStringText1;
    [SerializeField] private TMP_Text finalStringText2;
    [SerializeField] private TMP_Text finalStringText3;
    [SerializeField] private GameObject finalImage;
    [SerializeField] private GameObject string1;
    [SerializeField] private GameObject string2;

    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject negativeFinalImage;
    [SerializeField] private GameObject responseBox;

    // MODIFICA: Aggiunto riferimento allo sfondo delle risposte
    [SerializeField] private GameObject sfondoRisposteMultiple;

    [SerializeField] private TMP_InputField freeResponseInput;

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private CharacterStats characterStats;
    private MenuScript menuScript;
    private GeminiPrompt geminiPrompt;

    private string risposteLibere = "";
    private string finalString = "Che il combattimento abbia inizio!";

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        menuScript = GetComponent<MenuScript>();
        geminiPrompt = GetComponent<GeminiPrompt>();

        // Ensure the free response input is hidden at start
        if (freeResponseInput != null)
        {
            freeResponseInput.gameObject.SetActive(false);
        }

        // MODIFICA: Assicuriamoci che anche lo sfondo delle risposte sia disattivato all'inizio
        if (sfondoRisposteMultiple != null)
        {
            sfondoRisposteMultiple.SetActive(false);
        }

        CloseDialogueBox();
        ShowDialogue(testDialogue);
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        dialogueBox.SetActive(true);
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < dialogueObject.DialogueLines.Length; i++)
        {
            DialogueLine currentLine = dialogueObject.DialogueLines[i];
            speakerText.text = currentLine.Speaker;

            yield return RunTypingEffect(currentLine.Line);

            textLabel.text = currentLine.Line;

            if (i == dialogueObject.DialogueLines.Length - 1 && (dialogueObject.HasResponses || dialogueObject.IsFreeResponse))
                break;

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        if (dialogueObject.IsFreeResponse)
        {
            if (freeResponseInput != null)
            {
                responseBox.SetActive(true);
                // MODIFICA: Quando si attiva l'input field, lo sfondo per le risposte multiple deve essere SPENTO
                sfondoRisposteMultiple.SetActive(false);

                freeResponseInput.gameObject.SetActive(true);
                freeResponseInput.text = "";

                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));

                freeResponseInput.gameObject.SetActive(false);
                responseBox.SetActive(false);
                string userResponse = freeResponseInput.text;
                risposteLibere += userResponse + "\n";
                geminiPrompt.SetName(risposteLibere);
                Debug.Log("Nome " + risposteLibere);

                if (dialogueObject.NextDialogue != null)
                {
                    ShowDialogue(dialogueObject.NextDialogue);
                }
                else if (dialogueObject.IsFinalDialogue)
                {
                    responseHandler.SendHistoryToGemini();
                }
                else
                {
                    CloseDialogueBox();
                }
            }
        }
        else if (dialogueObject.HasResponses)
        {
            // MODIFICA: Quando si mostrano le risposte multiple, lo sfondo deve essere ACCESO
            sfondoRisposteMultiple.SetActive(true);
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            if (dialogueObject.IsFinalDialogue)
            {
                responseHandler.SendHistoryToGemini();
            }
            else
            {
                CloseDialogueBox();
            }
        }
    }

    private IEnumerator RunTypingEffect(string dialogue)
    {
        typewriterEffect.Run(dialogue, textLabel);
        while (typewriterEffect.IsRunning)
        {
            yield return null;
        }
    }

    private void CloseDialogueBox()
    {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
        speakerText.text = string.Empty;
    }

    private void ShowNegativeFinalImage()
    {
        if (negativeFinalImage != null)
        {
            negativeFinalImage.SetActive(true);
        }
    }

    public IEnumerator ShowFinalString(string finalString1, string finalString2)
    {
        finalImage.SetActive(true);
/*

                Debug.Log("Final string: " + finalString1);
                typewriterEffect.Run(finalString1, finalStringText1);
                while (typewriterEffect.IsRunning)
                {
                    yield return null;
                }
                // Attendi click prima di mostrare la seconda stringa
                //yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        */
        //string1.SetActive(true);
        Debug.Log("Final string 2: " + finalString1);
        typewriterEffect.Run(finalString1, finalStringText2);
        while (typewriterEffect.IsRunning)
        {
            yield return null;
        }
        // Attendi click prima di mostrare la terza stringa
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        string2.SetActive(true);
        Debug.Log("Final string 3: " + finalString2);
        typewriterEffect.Run(finalString2, finalStringText3);
        while (typewriterEffect.IsRunning)
        {
            yield return null;
        }
        // Attendi click prima di cambiare scena
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        SceneManager.LoadScene("NewCombatScene");
    }
}