using UnityEngine;


    [CreateAssetMenu(fileName = "NewFightEvent", menuName = "Fight/Fight Event")]
    public class FightEvent : ScriptableObject {

        public enum FightEventType 
        {
            SpawnEnemy,
            SpawnObject,
            Explosion,
            Storm
        }

        public enum BoundaryDirection
        {
            Right, Left, Up, Down
        }

        // Comuni
        public string eventName;
        public FightEventType eventType;

        // Condizioni
        public float triggerHealthPercentage = -1f; // -1 = disattivato
        public float triggerTime = -1f; // -1 = disattivato

        
        //Se uso un placeholder nella scena per indicare il punto di spawn dell'oggetto/nemico
        public Transform targetReference; 

        // Spawn Enemy/Object
        public GameObject prefabToSpawn;
        public Vector3 spawnPosition;

        //Spawn nemico
        public BoundaryDirection boundaryDirection;
        public int firstEncounterAttack, ordinaryAttack;


        // Esplosione
        public ParticleSystem explosionEffect;
        public float explosionRadius;
        public Vector3 explosionPosition;

    //Storm
    public ParticleSystem stormParticle;
    public GameObject lightningStrikeFX;
        

    }

