using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterStats : MonoBehaviour
{

    //Vita e mana, nel caso avessimo degli attacchi speciali.
    [SerializeField] private int vita = 100, mana = 100;
    //Con la stessa logica in cui un personaggio più corazzato è lento, si potrebbe pensare di inserire una difesa in futuro.
    [SerializeField] private int attacco = 10, difesa = 0;
    //User Interface
    [SerializeField] private TMP_Text healthText, manaText;

    private RectTransform healthBarRect;
    private float maxHealthBarWidth;
    public bool isDead = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText = GameObject.Find("Text_Vita").GetComponent<TMP_Text>();
        manaText = GameObject.Find("Text_Mana").GetComponent<TMP_Text>();

        healthBarRect = GameObject.Find("HealthUI_PL").GetComponent<RectTransform>();
        maxHealthBarWidth = healthBarRect.sizeDelta.x;

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUI()
    {
        healthText.text = "Vita giocatore: " + vita.ToString();
        manaText.text = "Mana giocatore: " + mana.ToString();

        // Calcola il rapporto tra vita corrente e vita massima
        float normalizedHealth = (float)vita / 100f; // Assumendo che 100 sia la vita massima
        // Aggiorna la larghezza della barra
        Vector2 size = healthBarRect.sizeDelta;
        size.x = maxHealthBarWidth * normalizedHealth;
        healthBarRect.sizeDelta = size;
    }

    public void HitTarget(int damage)
    {
        

        vita -= damage;

        if(vita <= 0){
            vita = 0;
            GetComponent<Animator>().SetTrigger("Die");
            isDead = true;

            // LoadNextScene();
        }
        UpdateUI();

    }

    private void LoadNextScene()
    {
        
        // SceneManager.LoadScene("DialogueTest");
        GameObject.Find("FadingImage").GetComponent<MenuScript>().FadeIn();
        Debug.Log("Fading to next scene!");
    }
}



