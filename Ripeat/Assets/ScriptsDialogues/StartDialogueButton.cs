using UnityEngine;
using UnityEngine.UI;

public class StartDialogueButton : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;         // Riferimento al componente che gestisce il dialogo
    [SerializeField] private DialogueObject dialogueToStart;  // Il DialogueObject da usare all'avvio
    [SerializeField] private GameObject backgroundPanel; 
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(StartDialogue);
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI != null && dialogueToStart != null)
        {
            Destroy(backgroundPanel);
            Destroy(gameObject);
            dialogueUI.ShowDialogue(dialogueToStart);
        }
    }
}

