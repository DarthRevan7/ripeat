using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AI; 
using System.Collections;

public class FightEventController : MonoBehaviour {
    public static FightEventController Instance { get; private set; }

    public List<FightEvent> loadedEvents = new List<FightEvent>();
    [SerializeField] private float fightTimer, fightTimerSnapshot = float.MaxValue;
    [SerializeField] private bool fightTimerActive = false;
    [SerializeField] private string resourcesDirectory = "FightEvents";

    [SerializeField] private FighterStats playerStats, enemyStats;
    public bool isTriggered = false, eventFinished = false;
    [SerializeField] private EventHandler eventHandler;

    public int actualEventIndex;
    public int globalEventIndex = 0;

    public GameObject secondaryEnemy = null;
    private bool isMainEnemyReturning = false;
    public bool loading = false, triggered = false;

    public HashSet<int> triggeredEventIndices = new HashSet<int>();

    //Usa questo per fotografare il tempo quando serve, in modo da lanciare l'evento nel momento giusto!
    public void SetTimeSnapshot()
    {
        fightTimerSnapshot = fightTimer;
    }

    private void CheckEventFlow()
    {

        // If player is dead
        if (playerStats.gameObject.GetComponent<CombatAnimSystem>().CurrentState == CombatAnimSystem.CombatAnimState.DEAD && !loading)
        {
            loading = true;
            // Avvia la coroutine che aspetta 2 secondi prima di cambiare scena
            StartCoroutine(LoadSceneWithDelay());
            Debug.Log("Loading elevator Scene");
        }
    }

    IEnumerator LoadSceneWithDelay() {
        yield return new WaitForSeconds(2f);
        GameObject.Find("FadingImage").GetComponent<MenuScript>().LoadScene();
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

        enemyStats = GameObject.Find("Main Enemy").GetComponent<FighterStats>();
        playerStats = GameObject.Find("Player").GetComponent<FighterStats>();

        LoadAllEvents();

        SceneManager.sceneLoaded += OnLoadScene;
    }

    public void OnLoadScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name.Equals("CombatScene"))
        {
            // Resettiamo il flag del rientro del nemico principale ogni volta che la scena di combattimento inizia.
            isMainEnemyReturning = false;
            //Index for current event system
            actualEventIndex = 0;

            eventHandler = EventHandler.Instance;

            enemyStats = GameObject.Find("Main Enemy").GetComponent<FighterStats>();
            playerStats = GameObject.Find("Player").GetComponent<FighterStats>();

            //Get the references to player and enemy stats
            enemyStats = GameObject.Find("Main Enemy").GetComponent<FighterStats>();
            playerStats = GameObject.Find("Player").GetComponent<FighterStats>();

            isTriggered = false;
            eventFinished = false;

            fightTimer = 0;

            // LoadAllEvents();
        }
        loading = false;
        StopAllCoroutines();
    }

    //Carica gli eventi dalla directory Resources/
    private void LoadAllEvents() {
        FightEvent[] events = Resources.LoadAll<FightEvent>(resourcesDirectory);
        loadedEvents.AddRange(events);
    }




    private void Update()
    {

        if (!SceneManager.GetActiveScene().name.Equals("CombatScene"))
        {
            loading = false;
            return;
        }

        //If there are no events left
        if (eventFinished) return;

        if (actualEventIndex == loadedEvents.Count)
        {
            eventFinished = true;
        }

        CheckEventFlow();


        FightEvent fightEvent = null;

        //Serve solo un controllo x l'evento all'indice N
        if (actualEventIndex < loadedEvents.Count)
        {
            fightEvent = loadedEvents[actualEventIndex];
        }

        if (fightEvent != null)
        {
            if (TriggeredByTime(fightEvent) && !fightTimerActive)
            {
                fightTimer = 0;
                fightTimerActive = true;
            }
            if (fightTimerActive)
            {
                fightTimer += Time.deltaTime;
            }
            if (ShouldTrigger(fightEvent, false) && !isTriggered)
            {
                isTriggered = true;
                TriggerEvent(fightEvent);
                // actualEventIndex++;
            }
        }

        if (secondaryEnemy != null && secondaryEnemy.GetComponent<FighterStats>().vita <= 0 && !isMainEnemyReturning)
        {
            // Imposta il flag a true per evitare che questo blocco venga eseguito di nuovo
            isMainEnemyReturning = true;

            // Chiama la funzione UNA SOLA VOLTA
            EventHandler.Instance.TakeBackMainEnemy();
            
        }
        
        

        
        
    }
    //Controlla che l'evento debba essere triggerato dal tempo
    private bool TriggeredByTime(FightEvent fightEvent)
    {
        return fightEvent.triggerTime > 0;
    }

    //Controlla le condizioni di trigger degli eventi. Per ora è impostato sul check di
    //Salute e tempo. Quando una delle due condizioni si verifica, allora il metodo ritorna true.
    private bool ShouldTrigger(FightEvent fightEvent, bool checkOnPlayer)
    {

        bool healthCondition = false;
        if (checkOnPlayer)
            healthCondition = fightEvent.triggerHealthPercentage >= 0f && CheckHealthCondition(playerStats, fightEvent.triggerHealthPercentage);
        else
            healthCondition = fightEvent.triggerHealthPercentage >= 0f && CheckHealthCondition(enemyStats, fightEvent.triggerHealthPercentage);


        bool timeCondition = fightEvent.triggerTime >= 0f && (fightTimer - fightTimerSnapshot) >= fightEvent.triggerTime;

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
