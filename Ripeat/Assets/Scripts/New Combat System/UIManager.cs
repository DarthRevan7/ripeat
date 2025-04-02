using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private FighterStats playerStats, enemyStats;

    private RectTransform healthBarRectPlayer, healthBarRectEnemy;
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
        enemyStats = GameObject.Find("MyEnemyNew").GetComponent<FighterStats>();

        healthBarRectPlayer = GameObject.Find("HealthUI_PL").GetComponent<RectTransform>();
        healthBarRectEnemy = GameObject.Find("HealthUI_EN").GetComponent<RectTransform>();

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
    }
}
