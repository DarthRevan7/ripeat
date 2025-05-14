using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private TMP_Text finalStringText;
    [SerializeField] private GameObject finalImage;
    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject negativeFinalImage;
    [SerializeField] private GameObject responseBox;
    
    // New input field for free responses (assign this from the Inspector)
    [SerializeField] private TMP_InputField freeResponseInput;

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private CharacterStats characterStats;
    private MenuScript menuScript;
    private GeminiPrompt geminiPrompt;
    
    // New field to store the conversation history (i.e. responses given)
    private string risposteLibere = "";

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        menuScript = GetComponent<MenuScript>();
        geminiPrompt = GetComponent<GeminiPrompt>();

        // Ensure the free response input is hidden at start
        if(freeResponseInput != null)
        {
            freeResponseInput.gameObject.SetActive(false);
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

        // Iterate through the DialogueLines array
        for (int i = 0; i < dialogueObject.DialogueLines.Length; i++)
        {
            DialogueLine currentLine = dialogueObject.DialogueLines[i];
            // Set the speaker name for the current line
            speakerText.text = currentLine.Speaker;

            yield return RunTypingEffect(currentLine.Line);
            
            textLabel.text = currentLine.Line;

            // If we're at the last line and there are responses/free response, exit the loop
            if (i == dialogueObject.DialogueLines.Length - 1 && (dialogueObject.HasResponses || dialogueObject.IsFreeResponse))
                break;

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        // Handle responses or free responses
        if (dialogueObject.IsFreeResponse)
        {
            // Enable the input field for free response and wait for submission (Enter key)
            if (freeResponseInput != null)
            {
                responseBox.SetActive(true);
                freeResponseInput.gameObject.SetActive(true);
                freeResponseInput.text = "";
                // Optionally set the input field as selected (for example, using EventSystem)
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
                freeResponseInput.gameObject.SetActive(false);
                responseBox.SetActive(false);
                string userResponse = freeResponseInput.text;
                risposteLibere += userResponse + "\n";
                geminiPrompt.SetName(risposteLibere);
                Debug.Log("Nome " + risposteLibere);
                
                
                // If a next dialogue is assigned, show it; otherwise, check if the dialogue is final.
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
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            // If the dialogue is final, determine the ending
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

    public IEnumerator ShowFinalString(string finalString)
    {
        finalImage.SetActive(true);
        typewriterEffect.Run(finalString, finalStringText);

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        SceneManager.LoadScene("NewCombatScene"); // Load the main menu scene, for example
    }
}
