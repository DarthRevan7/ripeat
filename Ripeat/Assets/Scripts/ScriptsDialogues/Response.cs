using UnityEngine;

[System.Serializable]
public class Response
{
    [SerializeField] private string responseText;
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private int pointValue;

    public string ResponseText => responseText;
    public DialogueObject DialogueObject => dialogueObject;
    public int PointValue => pointValue;
}
