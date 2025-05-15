using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FightEvent))]
public class FightEventEditor : Editor {
    SerializedProperty eventName;
    SerializedProperty eventType;
    SerializedProperty triggerHealthPercentage;
    SerializedProperty triggerTime;
    SerializedProperty targetReference;
    SerializedProperty prefabToSpawn;
    SerializedProperty spawnPosition;
    SerializedProperty explosionEffect;
    SerializedProperty explosionRadius;
    SerializedProperty explosionPosition;
    SerializedProperty boundaryDirection;
    SerializedProperty firstEncounterAttack, ordinaryAttack;

    SerializedProperty stormParticle, lightningStrikeFX;

    void OnEnable() {
        eventName = serializedObject.FindProperty("eventName");
        eventType = serializedObject.FindProperty("eventType");
        triggerHealthPercentage = serializedObject.FindProperty("triggerHealthPercentage");
        triggerTime = serializedObject.FindProperty("triggerTime");
        targetReference = serializedObject.FindProperty("targetReference");
        prefabToSpawn = serializedObject.FindProperty("prefabToSpawn");
        spawnPosition = serializedObject.FindProperty("spawnPosition");
        explosionEffect = serializedObject.FindProperty("explosionEffect");
        explosionRadius = serializedObject.FindProperty("explosionRadius");
        explosionPosition = serializedObject.FindProperty("explosionPosition");
        boundaryDirection = serializedObject.FindProperty("boundaryDirection");
        firstEncounterAttack = serializedObject.FindProperty("firstEncounterAttack");
        ordinaryAttack = serializedObject.FindProperty("ordinaryAttack");

        stormParticle = serializedObject.FindProperty("stormParticle");
        lightningStrikeFX =  serializedObject.FindProperty("lightningStrikeFX");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(eventName);
        EditorGUILayout.PropertyField(eventType);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Trigger Conditions", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(triggerHealthPercentage);
        EditorGUILayout.PropertyField(triggerTime);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Event Parameters", EditorStyles.boldLabel);

        FightEvent.FightEventType type = (FightEvent.FightEventType)eventType.enumValueIndex;

        switch (type) {
            case FightEvent.FightEventType.SpawnEnemy:
                EditorGUILayout.PropertyField(prefabToSpawn);
                EditorGUILayout.PropertyField(targetReference);
                EditorGUILayout.PropertyField(spawnPosition);
                EditorGUILayout.PropertyField(boundaryDirection);
                EditorGUILayout.PropertyField(firstEncounterAttack);
                EditorGUILayout.PropertyField(ordinaryAttack);
                break;
            case FightEvent.FightEventType.SpawnObject:
                EditorGUILayout.PropertyField(prefabToSpawn);
                EditorGUILayout.PropertyField(targetReference);
                EditorGUILayout.PropertyField(spawnPosition);
                break;
            case FightEvent.FightEventType.Explosion:
                EditorGUILayout.PropertyField(explosionEffect);
                EditorGUILayout.PropertyField(explosionRadius);
                EditorGUILayout.PropertyField(explosionPosition);
                EditorGUILayout.PropertyField(targetReference);
                break;
            case FightEvent.FightEventType.Storm:
                EditorGUILayout.PropertyField(stormParticle);
                EditorGUILayout.PropertyField(lightningStrikeFX);
                EditorGUILayout.PropertyField(spawnPosition);
            break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
