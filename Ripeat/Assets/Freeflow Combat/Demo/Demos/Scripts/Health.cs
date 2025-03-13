using UnityEngine;
using TMPro;

namespace FreeflowCombatSpace
{
    public class Health : MonoBehaviour
    {
        public int health = 100;
        public TextMeshProUGUI healthUI;
        private RectTransform healthEnemyBarRect;
        private float maxHealthEnemyBarWidth;

        Animator anim;
        Vector3 startPos;
        public bool isDead;


        void Start()
        {
            healthUI.text = "Vita Nemico: " + health.ToString();
            anim = GetComponent<Animator>();
            startPos = transform.position;

            healthEnemyBarRect = GameObject.Find("HealthUI_EN").GetComponent<RectTransform>();
            maxHealthEnemyBarWidth = healthEnemyBarRect.sizeDelta.x;

        }


        public void Hit(int hitPoints)
        {
            health -= hitPoints;
            if (health < 0) health = 0;

            // Calcola il rapporto tra vita corrente e vita massima
            float normalizedEnemyHealth = (float)health / 100f; // Assumendo che 100 sia la vita massima
            // Aggiorna la larghezza della barra
            Vector2 size = healthEnemyBarRect.sizeDelta;
            size.x = maxHealthEnemyBarWidth * normalizedEnemyHealth;
            healthEnemyBarRect.sizeDelta = size;

            healthUI.text = "Vita Nemico: " + health.ToString();

            if (health <= 0) {
                Die();
            }
        }


        void Die()
        {
            if (isDead) return;
            
            // play die animation
            anim.SetTrigger("Die");
            

            // set the enemy as not attackable
            GetComponent<FreeflowCombatEnemy>().isAttackable = false;

            // deactivate rotation script
            GetComponent<LookAtPlayer>().enabled = false;

            isDead = true;
        }

        public void RefreshUI()
        {
            healthUI.text = health.ToString();
            transform.position = startPos;
            isDead = false;
        }
    }
}

