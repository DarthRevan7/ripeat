using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text textLabel;
    [SerializeField] private DialogueObject testDialogue;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject negativeFinalImage;

    private ResponseHandler responseHandler;
    private TypewriterEffect typewriterEffect;
    private CharacterStats characterStats;

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
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

        // Itera sull'array di DialogueLine
        for (int i = 0; i < dialogueObject.DialogueLines.Length; i++)
        {
            DialogueLine currentLine = dialogueObject.DialogueLines[i];
            // Imposta il nome del parlante per la riga corrente
            speakerText.text = currentLine.Speaker;

            yield return RunTypingEffect(currentLine.Line);
            
            textLabel.text = currentLine.Line;

            // Se sull'ultima riga e sono presenti risposte, esci dal ciclo
            if (i == dialogueObject.DialogueLines.Length - 1 && dialogueObject.HasResponses)
                break;

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        if (dialogueObject.HasResponses)
        {
            responseHandler.ShowResponses(dialogueObject.Responses);
        }
        else
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            
            // Se il dialogo è finale e negativo, mostra l'immagine di finale negativo,
            // altrimenti chiudi il dialogo normalmente
            if (dialogueObject.IsFinalDialogue && dialogueObject.DialogueFinalType == FinalDialogueType.Negative)
            {
                yield return new WaitForSeconds(1f);
                ShowNegativeFinalImage();
                yield return new WaitForSeconds(3f);
                SceneManager.LoadScene("Menu");
            }
            else if (dialogueObject.IsFinalDialogue && dialogueObject.DialogueFinalType == FinalDialogueType.Positive)
            {
                yield return new WaitForSeconds(1f);
                // Finito il dialogo fa il fade in e passa alla scena successiva
                // SceneManager.LoadScene("FightingScene_Try");
                GameObject.Find("FadingImage").GetComponent<MenuScript>().LoadScene();
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
}
