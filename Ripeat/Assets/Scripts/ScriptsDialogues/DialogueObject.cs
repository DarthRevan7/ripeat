using UnityEngine;

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

    public DialogueLine[] DialogueLines => dialogueLines;
    public bool HasResponses => responses != null && responses.Length > 0;
    public Response[] Responses => responses;
}
