using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public static EventHandler Instance { get; private set; }

    [SerializeField] private List<FightEvent> fightEvents;

    [SerializeField] private int currentEventIndex = 0;
    [SerializeField] private int globalEventIndex;

    [SerializeField] private Dictionary<string, Collider> entryPointColliders;

    [SerializeField] private GameObject mainEnemy, player;
    [SerializeField] private GameObject[] secondaryEnemies;
    [SerializeField] private string playerTag = "Player", enemyTag = "Main Enemy",
    secondaryEnemyTag = "Secondary Enemy";
    [SerializeField] private float mainEnemyXCoord = 20f, newEnemyXCoord = 0;
    [SerializeField] private GameObject secondaryEnemyHealthBar;




    public void HandleSpawnEvent(FightEvent fightEvent)
    {
        //Maybe insert here a check for event type
        string boundaryName = fightEvent.boundaryDirection.ToString();
        Debug.Log(boundaryName);
        //Reference to the characters
        player = GameObject.FindGameObjectWithTag(playerTag);
        mainEnemy = GameObject.FindGameObjectWithTag(enemyTag);
        // secondaryEnemies = GameObject.FindGameObjectsWithTag(secondaryEnemyTag);

        //Disable Enemy AI
        mainEnemy.GetComponent<MainEnemyAI>().enabled = false;
        //Disable Player Input
        player.GetComponent<InputManager>().enabled = false;
        //Disable boundary in that direction
        Collider colliderToDisable;
        bool colliderFound = 
        entryPointColliders.TryGetValue(fightEvent.boundaryDirection.ToString(), out colliderToDisable);
        colliderToDisable.gameObject.SetActive(false);

        //Call a Coroutine to complete the task
        StartCoroutine(SpawnHandler(fightEvent, colliderToDisable));
        //Make the main enemy go away

        //Spawn secondary enemy in position
        //Enable secondary Enemy AI
        //Check secondary enemy position
        //Enable Player Input
        //Enable Boundary again
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
        mainEnemy.GetComponent<CombatSystem>().enabled = false;
        //Spawn secondary enemy in position
        GameObject newEnemy = GameObject.Instantiate(fightEvent.prefabToSpawn, fightEvent.spawnPosition, Quaternion.identity);
        //Bring the secondary enemy in the scene
        while(newEnemy.transform.position.x >= mainEnemyXCoord)
        {
            newEnemy.GetComponent<CombatSystem>().canMove = true;
            newEnemy.GetComponent<CombatSystem>().MovementInput = Vector3.left;
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        //Enable Secondary Enemy Health Bar
        UIManager uIManager = GameObject.FindAnyObjectByType<UIManager>();
        secondaryEnemyHealthBar.SetActive(true);
        uIManager.secondEnemyStats = GameObject.FindGameObjectWithTag("Secondary Enemy").GetComponent<FighterStats>();
        uIManager.healthBarRectSecondEnemy = GameObject.Find("HealthUI_EN2").GetComponent<RectTransform>();
        uIManager.secondEnemyActive = true;
        yield return new WaitForSeconds(1.0f);
        //Check secondary enemy position
        //Enable Player Input
        player.GetComponent<CombatSystem>().enabled = true;
        player.GetComponent<InputManager>().enabled = true;
        //Enable secondary Enemy AI
        newEnemy.GetComponent<MainEnemyAI>().enabled = true;
        //Enable Boundary again
        colliderToDisable.gameObject.SetActive(true);

        yield return null;
    }

    void Awake()
    {

        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        entryPointColliders = new Dictionary<string, Collider>();
        GameObject colliderToAdd = GameObject.Find("Right");
        entryPointColliders.Add(colliderToAdd.name, colliderToAdd.GetComponent<Collider>());
        colliderToAdd = GameObject.Find("Left");
        entryPointColliders.Add(colliderToAdd.name, colliderToAdd.GetComponent<Collider>());
        Debug.Log("colliderToAdd.name = " + colliderToAdd.name.ToString());
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
