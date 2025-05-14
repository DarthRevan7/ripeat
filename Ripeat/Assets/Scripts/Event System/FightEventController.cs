using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AI;

public class FightEventController : MonoBehaviour {
    public static FightEventController Instance { get; private set; }

    public List<FightEvent> loadedEvents = new List<FightEvent>();
    [SerializeField] private float fightTimer;
    [SerializeField] private bool fightActive = false;
    [SerializeField] private string resourcesDirectory = "FightEvents";

    [SerializeField] private FighterStats playerStats, enemyStats;
    public bool isTriggered = false, eventFinished = false;
    [SerializeField] private EventHandler eventHandler;

    public int actualEventIndex;
    public static int globalEventIndex;

    public GameObject secondaryEnemy = null;
    public bool loading = false, triggered = false;

    public HashSet<int> triggeredEventIndices = new HashSet<int>();

    

    private void CheckEventFlow()
    {

        //If player is dead
        if(playerStats.gameObject.GetComponent<CombatSystem>().isDead && !loading)
        {
            loading = true;
            // //If player dead because of the event
            // if(actualEventIndex > globalEventIndex)
            // {
            //     //Update the global event index
            //     globalEventIndex = actualEventIndex;
            // }

            // Registra l'evento come già visto
            

            

            //Load Elevator Scene
            GameObject.Find("FadingImage").GetComponent<MenuScript>().LoadScene();
            Debug.Log("Loading elevator Scene");

        }
    }

    private void Awake() {

        // GameObject.Find("FadingImage").GetComponent<MenuScript>().FadeOut();

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //Index for current event system
        actualEventIndex = 0;

        eventHandler = EventHandler.Instance;

        enemyStats = GameObject.Find("Enemy").GetComponent<FighterStats>();
        playerStats = GameObject.Find("Player").GetComponent<FighterStats>();

        LoadAllEvents();

        SceneManager.sceneLoaded += OnLoadScene;
    }

    public void OnLoadScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name.Equals("CombatScene"))
        {
            //Index for current event system
            actualEventIndex = 0;

            eventHandler = EventHandler.Instance;

            enemyStats = GameObject.Find("Enemy").GetComponent<FighterStats>();
            playerStats = GameObject.Find("Player").GetComponent<FighterStats>();

            //Get the references to player and enemy stats
            enemyStats = GameObject.Find("Enemy").GetComponent<FighterStats>();
            playerStats = GameObject.Find("Player").GetComponent<FighterStats>();

            isTriggered = false;
            eventFinished = false;

            // LoadAllEvents();
        }
        loading = false;
    }

    //Carica gli eventi dalla directory Resources/
    private void LoadAllEvents() {
        FightEvent[] events = Resources.LoadAll<FightEvent>(resourcesDirectory);
        loadedEvents.AddRange(events);
    }


    //Fa partire il timer nel caso di condizioni dettate dal tempo
    public void StartFight() {
        fightTimer = 0f;
        fightActive = true;
    }

    //Stoppa il timer
    public void StopFight() {
        fightActive = false;
    }

    private void Update() {

        if(!SceneManager.GetActiveScene().name.Equals("CombatScene")) 
        {
            loading = false;
            return;
        }

        //If there are no events left
        if(eventFinished)   return;

        if(actualEventIndex == loadedEvents.Count)
        {
            eventFinished = true;
        }

        CheckEventFlow();
        

        FightEvent fightEvent = null;

        //Serve solo un controllo x l'evento all'indice N
        if(actualEventIndex < loadedEvents.Count)
        {
            fightEvent = loadedEvents[actualEventIndex];
        }

        if(fightEvent != null)
        {
            if (ShouldTrigger(fightEvent, false) && !isTriggered) {
                isTriggered = true;
                TriggerEvent(fightEvent);
                // actualEventIndex++;
            }
        }

        if(secondaryEnemy != null)
        {
            if(secondaryEnemy.GetComponent<FighterStats>().vita <= 0)
            {
                EventHandler.Instance.TakeBackMainEnemy();
            }
        }

        
        
    }

    //Controlla le condizioni di trigger degli eventi. Per ora è impostato sul check di
    //Salute e tempo. Quando una delle due condizioni si verifica, allora il metodo ritorna true.
    private bool ShouldTrigger(FightEvent fightEvent, bool checkOnPlayer) {

        bool healthCondition = false;
        if(checkOnPlayer)
            healthCondition = fightEvent.triggerHealthPercentage >= 0f && CheckHealthCondition(playerStats,fightEvent.triggerHealthPercentage);
        else
            healthCondition = fightEvent.triggerHealthPercentage >= 0f && CheckHealthCondition(enemyStats,fightEvent.triggerHealthPercentage);


        bool timeCondition = fightEvent.triggerTime >= 0f && fightTimer >= fightEvent.triggerTime;

        return (healthCondition || timeCondition) && !triggered;
    }

    //Serve a fare il check della salute.
    private bool CheckHealthCondition(FighterStats stats, float healthThreshold) {
        return stats.vita <= healthThreshold;
    }

    private void TriggerEvent(FightEvent fightEvent) {

        // Si dovrebbe occupare di chiamare un altro script che si occupa della gestione 
        // degli eventi in game. FightEventHandler magari.

        if(SceneManager.GetActiveScene().name != "CombatScene") {
            return;
        }

        if(fightEvent.eventType.Equals(FightEvent.FightEventType.SpawnEnemy))
        {
            Debug.Log("FightEvent: " + fightEvent.boundaryDirection.ToString());
            EventHandler.Instance.HandleSpawnEvent(fightEvent);
            //isTriggered = true;
        }
        // Debug.Log("Event Type: " + fightEvent.eventType.ToString());
        else if(fightEvent.eventType.Equals(FightEvent.FightEventType.Explosion))
        {
            EventHandler.Instance.HandleExplosion(fightEvent);
            //isTriggered = true;
        }
        else if(fightEvent.eventType.Equals(FightEvent.FightEventType.Storm)) {
            EventHandler.Instance.HandleStorm(fightEvent);
        }
    }

    private void ApplyExplosionDamage(Vector3 position, float radius) {
        // Esempio di danno ad area
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);
        foreach (var hit in hitColliders) {
            // Infliggi danni e fai play al particle system
        }
    }
}
