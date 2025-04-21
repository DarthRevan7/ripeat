// File: EventTriggerEditor.cs
// Posizione: Assets/Editor/ (DEVE essere in una cartella Editor)
using UnityEngine;
using UnityEditor; // Necessario per gli script Editor

/// <summary>
/// Custom Editor per lo script EventTrigger.
/// Permette di mostrare un campo ObjectField specifico per l'evento
/// in base al valore dell'enum 'eventType' selezionato.
/// </summary>
[CustomEditor(typeof(EventTrigger))]
public class EventTriggerEditor : Editor
{
    // Proprietà serializzate per i campi di EventTrigger
    SerializedProperty eventTypeProp;
    SerializedProperty eventDataProp;
    SerializedProperty targetChannelProp;
    SerializedProperty testTriggerKeyProp; // Per l'esempio di trigger da tasto

    void OnEnable()
    {
        // Trova e collega le proprietà all'avvio
        eventTypeProp = serializedObject.FindProperty("eventType");
        eventDataProp = serializedObject.FindProperty("eventData");
        targetChannelProp = serializedObject.FindProperty("targetChannel");
        testTriggerKeyProp = serializedObject.FindProperty("testTriggerKey");
    }

    public override void OnInspectorGUI()
    {
        // Inizia il controllo delle modifiche per l'undo/redo
        serializedObject.Update();

        // --- Disegna i campi standard prima (o dopo) la logica custom ---
        EditorGUILayout.LabelField("Event Selection", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(eventTypeProp); // Disegna l'enum

        EditorGUILayout.Space(); // Spazio visivo

        // --- Logica Custom per il campo eventData ---
        SpecificEventType currentType = (SpecificEventType)eventTypeProp.enumValueIndex;
        System.Type expectedDataType = GetExpectedDataType(currentType);

        // Pulisci eventData se si seleziona "None" e c'era qualcosa assegnato
        if (currentType == SpecificEventType.None && eventDataProp.objectReferenceValue != null)
        {
            eventDataProp.objectReferenceValue = null;
            Debug.Log("Event data cleared because type was set to None.");
        }

        // Mostra il campo ObjectField appropriato
        if (expectedDataType != null)
        {
            EditorGUILayout.LabelField($"Event Data ({expectedDataType.Name})", EditorStyles.boldLabel);
            // Disegna l'ObjectField specificando il tipo atteso
            eventDataProp.objectReferenceValue = EditorGUILayout.ObjectField(
                GUIContent.none, // Nasconde la label predefinita del campo
                eventDataProp.objectReferenceValue,
                expectedDataType, // Forza l'accettazione solo di questo tipo!
                false // Non permettere asset di scena (tipicamente gli eventi sono Assets)
            );

            // Validazione: Controlla se l'asset assegnato (se esiste) corrisponde al tipo atteso
            if (eventDataProp.objectReferenceValue != null)
            {
                // Usiamo la funzione statica definita in EventTrigger per la validazione
                 EventTrigger triggerInstance = (EventTrigger)target; // Ottieni riferimento allo script ispezionato
                 if (!EventTrigger.IsEventTypeMatchingData(currentType, (GameEventSO)eventDataProp.objectReferenceValue))
                 {
                    EditorGUILayout.HelpBox($"Type Mismatch! L'asset assegnato ({eventDataProp.objectReferenceValue.GetType().Name}) non corrisponde al tipo selezionato ({currentType}). Rimuovi l'asset o cambia il tipo.", MessageType.Error);
                 }
            }
             else // Se nessun dato è assegnato ma un tipo è selezionato
             {
                  EditorGUILayout.HelpBox($"Assegna un asset di tipo '{expectedDataType.Name}' qui.", MessageType.Info);
             }
        }
        else // Se il tipo è None
        {
            EditorGUILayout.HelpBox("Nessun tipo di evento selezionato. Il trigger non farà nulla.", MessageType.Info);
        }

        EditorGUILayout.Space(); // Spazio visivo

        // --- Disegna gli altri campi ---
        EditorGUILayout.LabelField("Target Channel & Test", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(targetChannelProp); // Disegna il campo per il canale
        EditorGUILayout.PropertyField(testTriggerKeyProp); // Disegna il campo per il tasto di test


        // Applica tutte le modifiche registrate (fondamentale!)
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Funzione helper per ottenere il System.Type specifico dell'evento SO
    /// atteso in base al valore dell'enum selezionato.
    /// </summary>
    private System.Type GetExpectedDataType(SpecificEventType typeEnum)
    {
        switch (typeEnum)
        {
            case SpecificEventType.SpawnEnemy:   return typeof(SpawnEnemyEventSO);
            case SpecificEventType.Explosion:    return typeof(ExplosionEventSO);
             // Aggiungi altri case per i tuoi tipi
            case SpecificEventType.None:
            default:                           return null; // Nessun tipo specifico atteso
        }
    }
}