using UnityEngine;

public enum FinalDialogueType
{
    None,       // Non è un dialogo finale
    Positive,   // Dialogo finale positivo
    Negative    // Dialogo finale negativo
}

[System.Serializable]
public class DialogueLine
{
    [SerializeField] [TextArea] private string line;
    [SerializeField] private string speaker;

    public string Line => line;
    public string Speaker => speaker;
}

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private Response[] responses;

    // Nuovi campi per il dialogo finale
    [SerializeField] private bool isFinalDialogue;
    [SerializeField] private FinalDialogueType finalDialogueType = FinalDialogueType.None;

    public DialogueLine[] DialogueLines => dialogueLines;
    public bool HasResponses => responses != null && responses.Length > 0;
    public Response[] Responses => responses;
    
    public bool IsFinalDialogue => isFinalDialogue;
    public FinalDialogueType DialogueFinalType => finalDialogueType;
}
