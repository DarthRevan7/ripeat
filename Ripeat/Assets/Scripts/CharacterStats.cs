using UnityEngine;
using TMPro;

public class CharacterStats : MonoBehaviour
{

    //Vita e mana, nel caso avessimo degli attacchi speciali.
    [SerializeField] private int vita = 100, mana = 100;
    //Con la stessa logica in cui un personaggio più corazzato è lento, si potrebbe pensare di inserire una difesa in futuro.
    [SerializeField] private int attacco = 10, difesa = 0;
    //User Interface
    [SerializeField] private TMP_Text healthText, manaText;

    public bool isDead = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText = GameObject.Find("Text_Vita").GetComponent<TMP_Text>();
        manaText = GameObject.Find("Text_Mana").GetComponent<TMP_Text>();

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
    }

    public void HitTarget(int damage)
    {
        

        vita -= damage;

        if(vita <= 0){
            vita = 0;
            GetComponent<Animator>().SetTrigger("Die");
            isDead = true;
        }
        UpdateUI();

        
    }


}
