 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance { get; private set; }

    [SerializeField] private List<FightEvent> fightEvents;

    [SerializeField] public int currentEventIndex = 0;
    [SerializeField] private int globalEventIndex = 0;

    [SerializeField] private Dictionary<string, Collider> entryPointColliders;
    [SerializeField] private List<GameObject> colliders;

    [SerializeField] private GameObject mainEnemy, player;
    [SerializeField] private GameObject[] secondaryEnemies;
    [SerializeField] private string playerTag = "Player", enemyTag = "Main Enemy",
    secondaryEnemyTag = "Secondary Enemy";
    [SerializeField] private float mainEnemyXCoord = 20f, newEnemyXCoord = 6f, comingBackCoord = 7f;
    [SerializeField] private GameObject secondaryEnemyHealthBar;
    [SerializeField] private string boundaryName;
    

    

    void Awake()
    {

        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        entryPointColliders = new Dictionary<string, Collider>();
        GameObject colliderToAdd = GameObject.Find("Right");
        entryPointColliders.Add(colliderToAdd.name, colliderToAdd.GetComponent<Collider>());
        colliderToAdd = GameObject.Find("Left");
        entryPointColliders.Add(colliderToAdd.name, colliderToAdd.GetComponent<Collider>());
        Debug.Log("colliderToAdd.name = " + colliderToAdd.name.ToString());

        colliders = GameObject.FindGameObjectsWithTag("Boundary").ToList<GameObject>();

        SceneManager.sceneLoaded += OnLoadScene;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    private void OnLoadScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(SceneManager.GetActiveScene().name.Equals("CombatScene"))
        {
            entryPointColliders = new Dictionary<string, Collider>();
            GameObject colliderToAdd = GameObject.Find("Right");
            entryPointColliders.Add(colliderToAdd.name, colliderToAdd.GetComponent<Collider>());
            Debug.Log("colliderToAdd.name = " + colliderToAdd.name.ToString());
            colliderToAdd = GameObject.Find("Left");
            entryPointColliders.Add(colliderToAdd.name, colliderToAdd.GetComponent<Collider>());
            Debug.Log("colliderToAdd.name = " + colliderToAdd.name.ToString());

            //Find healthBar with Canvas parent transform.
            //DO NOT CHANGE THE CHILD'S POSITION!
            secondaryEnemyHealthBar = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
            
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public bool FirstEncounter()
    {
        return !FightEventController.Instance.triggeredEventIndices.Contains(FightEventController.Instance.actualEventIndex);
    }

    private void AssignStats(GameObject newEnemy, FightEvent fightEvent)
    {
        FighterStats stats = newEnemy.GetComponent<FighterStats>();
        if(FirstEncounter())
        {
            stats.attacco = fightEvent.firstEncounterAttack;
        }
        else
        {
            stats.attacco = fightEvent.ordinaryAttack;
        }
    }

    public void HandleExplosion(FightEvent fightEvent)
    {
        bool isFirstEncounter = FirstEncounter();
        //Disable Enemy AI
        mainEnemy.GetComponent<MainEnemyAI>().isScriptActive = false;
        //Disable Player Input
        player.GetComponent<InputManager>().isScriptActive = false;

        //Instantiate the Particle System
        ParticleSystem explosionPS =
        GameObject.Instantiate(fightEvent.explosionEffect, fightEvent.explosionPosition, Quaternion.identity);

        //Play the particle system explosion
        explosionPS.Play();

        //First time -> Kill player and halve enemy life
        //Other times -> Halve both character's life
        if(isFirstEncounter)
        {
            Debug.Log("Explosion Killer");
            player.GetComponent<FighterStats>().vita = 0;
            mainEnemy.GetComponent<FighterStats>().vita = 20;
        }
        else
        {
            Debug.Log("Explosion halver");
            player.GetComponent<FighterStats>().vita /= 2;
            mainEnemy.GetComponent<FighterStats>().vita /= 2;
        }

        //Enable Enemy AI
        mainEnemy.GetComponent<MainEnemyAI>().isScriptActive = true;
        //Enable Player Input
        player.GetComponent<InputManager>().isScriptActive = true;


        if(isFirstEncounter)
        {
            FightEventController.Instance.triggeredEventIndices.Add(FightEventController.Instance.actualEventIndex);
        }

        FightEventController.Instance.isTriggered = false;
        FightEventController.Instance.actualEventIndex++;
        
        
    }

    public void TakeBackMainEnemy()
    {
        //Disable Enemy AI
        mainEnemy.GetComponent<MainEnemyAI>().isScriptActive = false;
        //Disable Player Input
        player.GetComponent<InputManager>().isScriptActive = false;
        //Disable boundary in that direction
        Collider colliderToDisable;
        bool colliderFound = 
        entryPointColliders.TryGetValue(boundaryName, out colliderToDisable);
        colliderToDisable.gameObject.SetActive(false);

        //Coroutine to finish the job
        StartCoroutine(BringMainEnemyBack(colliderToDisable));
    }

    IEnumerator BringMainEnemyBack(Collider colliderToDisable)
    {
        //Make the main enemy come back
        mainEnemy.transform.LookAt(player.transform.position);
        mainEnemy.GetComponent<CombatSystem>().canMove = true;
        mainEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.left;
        mainEnemy.GetComponent<CombatSystem>().enabled = true;

        while(mainEnemy.transform.position.x >= comingBackCoord)
        {
            mainEnemy.GetComponent<CombatSystem>().canMove = true;
            mainEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.left;
            yield return null;
        }

        //Enable Player Input
        player.GetComponent<InputManager>().isScriptActive = true;
        // Debug.Log("Player Input Manager: " + player.GetComponent<InputManager>().isScriptActive.ToString());
        //Enable secondary Enemy AI
        mainEnemy.GetComponent<MainEnemyAI>().isScriptActive = true;
        //Enable Boundary again
        colliderToDisable.gameObject.SetActive(true);
    }

    public void HandleSpawnEvent(FightEvent fightEvent)
    {
        //Maybe insert here a check for event type
        boundaryName = fightEvent.boundaryDirection.ToString();
        // Debug.Log(boundaryName);
        //Reference to the characters
        player = GameObject.FindGameObjectWithTag(playerTag);
        mainEnemy = GameObject.FindGameObjectWithTag(enemyTag);

        //Disable Enemy AI
        mainEnemy.GetComponent<MainEnemyAI>().isScriptActive = false;
        //Disable Player Input
        player.GetComponent<InputManager>().isScriptActive = false;
        //Disable boundary in that direction
        GameObject colliderParent = GameObject.Find("EnvironmentColliders");
        Collider colliderToDisable = null;
        for(int i = 0; i < colliderParent.transform.childCount; i++)
        {
            if(colliderParent.transform.GetChild(i).gameObject.name.Equals(boundaryName))
            {
                colliderToDisable = colliderParent.transform.GetChild(i).gameObject.GetComponent<Collider>();
            }
        }
        // bool colliderFound = entryPointColliders.TryGetValue(boundaryName, out colliderToDisable);
        

        if(colliderToDisable != null)
        {
            if(fightEvent != null)
            {
                //Call a Coroutine to complete the task
                StartCoroutine(SpawnHandler(fightEvent, colliderToDisable));
            }
            else
            {
                Debug.Log("Event null!!");
            }
        }
        else
        {
            Debug.Log("Collider null!!");
        }
    }

    IEnumerator SpawnHandler(FightEvent fightEvent, Collider colliderToDisable)
    {
        //Make the main enemy go away
        mainEnemy.transform.LookAt(fightEvent.spawnPosition);
        mainEnemy.GetComponent<CombatSystem>().canMove = true;
        mainEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.right;
        mainEnemy.GetComponent<CombatSystem>().enabled = true;

        

        while(mainEnemy.transform.position.x <= mainEnemyXCoord)
        {
            mainEnemy.GetComponent<CombatSystem>().canMove = true;
            mainEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.right;
            yield return null;
        }

        //Movement Input Vector Equals Zero
        mainEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.zero;

        //Spawn secondary enemy in position
        GameObject newEnemy = GameObject.Instantiate(fightEvent.prefabToSpawn, fightEvent.spawnPosition, Quaternion.identity);
        AssignStats(newEnemy, fightEvent);

        //Bring the secondary enemy in the scene
        while(newEnemy.transform.position.x >= newEnemyXCoord)
        {
            newEnemy.GetComponent<CombatSystem>().canMove = true;
            newEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.left;
            yield return null;
        }


        //Enable Secondary Enemy Health Bar
        UIManager uIManager = GameObject.FindAnyObjectByType<UIManager>();
        secondaryEnemyHealthBar.SetActive(true);
        uIManager.secondEnemyStats = newEnemy.GetComponent<FighterStats>();
        uIManager.healthBarRectSecondEnemy = GameObject.Find("HealthUI_EN2").GetComponent<RectTransform>();
        uIManager.secondEnemyActive = true;

        //Enable Player Input
        player.GetComponent<InputManager>().isScriptActive = true;
        // Debug.Log("Player Input Manager: " + player.GetComponent<InputManager>().isScriptActive.ToString());
        //Enable secondary Enemy AI
        newEnemy.GetComponent<MainEnemyAI>().isScriptActive = true;
        //Enable Boundary again
        colliderToDisable.gameObject.SetActive(true);

        FightEventController.Instance.secondaryEnemy = newEnemy;
        
        Debug.Log("Is 1st encounter = " + FirstEncounter().ToString());
        
        //Update Global Event Index
        if(FirstEncounter())
        {
            FightEventController.globalEventIndex++;
            Debug.Log("GlobalEventIndex = " + FightEventController.globalEventIndex.ToString());
            FightEventController.Instance.triggeredEventIndices.Add(FightEventController.Instance.actualEventIndex);
        }
        
        FightEventController.Instance.isTriggered = false;
        FightEventController.Instance.actualEventIndex++;

        yield return null;
    }
}
