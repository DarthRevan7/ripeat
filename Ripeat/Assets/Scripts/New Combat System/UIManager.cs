using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] public FighterStats playerStats, enemyStats, secondEnemyStats;
    public bool secondEnemyActive = false;

    public RectTransform healthBarRectPlayer, healthBarRectEnemy, healthBarRectSecondEnemy;
    private float maxHealthBarWidth;



    private void UpdateUI(RectTransform healthBarRect, FighterStats stats)
    {
        int vita = stats.vita;
        // Calcola il rapporto tra vita corrente e vita massima
        float normalizedHealth = (float)vita / 100f; // Assumendo che 100 sia la vita massima
        // Aggiorna la larghezza della barra
        Vector2 size = healthBarRect.sizeDelta;
        size.x = maxHealthBarWidth * normalizedHealth;
        healthBarRect.sizeDelta = size;
    }

    void Awake()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<FighterStats>();
        enemyStats = GameObject.FindGameObjectWithTag("Main Enemy").GetComponent<FighterStats>();
        

        healthBarRectPlayer = GameObject.Find("HealthUI_PL").GetComponent<RectTransform>();
        healthBarRectEnemy = GameObject.Find("HealthUI_EN").GetComponent<RectTransform>();
        
        // if(secondEnemyActive){
        //     secondEnemyStats = GameObject.FindGameObjectWithTag("Secondary Enemy").GetComponent<FighterStats>();
        //     healthBarRectSecondEnemy = GameObject.Find("HealthUI_EN2").GetComponent<RectTransform>();
        // }

        maxHealthBarWidth = healthBarRectPlayer.sizeDelta.x;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI(healthBarRectPlayer, playerStats);
        UpdateUI(healthBarRectEnemy, enemyStats);
        if (secondEnemyActive)
        {
            UpdateUI(healthBarRectSecondEnemy, secondEnemyStats);
        }
    }
}
