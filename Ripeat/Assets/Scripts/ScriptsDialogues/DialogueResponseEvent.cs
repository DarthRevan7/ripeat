using UnityEngine;
using System;

public class DialogueResponseEvent : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private ResponseEvent[] events; // Array di eventi di risposta

    public ResponseEvent[] Events => events; // Proprietà per accedere agli eventi

    public void OnValidate()
    {
        if (dialogueObject == null) return; // Controlla se dialogueObject è nullo
        if (dialogueObject.Responses == null) return; // Controlla se Responses è nullo
        if (events != null && events.Length == dialogueObject.Responses.Length) return; // Controlla se events ha la stessa lunghezza di Responses

        if (events == null)
        {
            events = new ResponseEvent[dialogueObject.Responses.Length]; // Inizializza events con la lunghezza di Responses
        }
        else
        {
            Array.Resize(ref events, dialogueObject.Responses.Length); // Ridimensiona events alla lunghezza di Responses
        }

        for(int i = 0; i < dialogueObject.Responses.Length; i++) // Itera attraverso Responses
        {
            Response response = dialogueObject.Responses[i]; // Ottiene la risposta corrente
            if( events[i] != null)
            {
                events[i].name = response.ResponseText; // Aggiorna il nome dell'evento se esiste già
                continue;
            }

            events[i] = new ResponseEvent() {name = response.ResponseText}; // Crea un nuovo evento con il nome della risposta
        }
    }
}
