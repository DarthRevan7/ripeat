using UnityEngine;
using FreeflowCombatSpace;

namespace FreeflowCombatSpace
{
    public class HitEnemy : MonoBehaviour
    {
        public FlagAttack flagAttack;
        AudioSource hitSound;
        Animator enemyAnim;

        bool coolDown = false;
        float timer = 0f;

        public int playerDamage;


        void Start() {
            //hitSound = GetComponent<AudioSource>();
            playerDamage = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterStats>().attacco;
        }

        void Update()
        {
            if (coolDown) {
                timer += Time.deltaTime;
                if (timer >= 0.3f) {
                    coolDown = false;
                    timer = 0f;
                }
            }
        }

        void LateUpdate() 
        {
            CheckTrigger();
        }

        void CheckTrigger()
        {
            if (!flagAttack.hitting || coolDown) return;

            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
            
            foreach(var item in colliders) {
                if (item.transform.GetComponent<FreeflowCombatEnemy>()) {
                    //if (!hitSound.isPlaying) hitSound.Play();

                    item.GetComponent<Health>().Hit(playerDamage);

                    enemyAnim = item.transform.GetComponent<Animator>();
                    enemyAnim.SetTrigger("Hit");
                    coolDown = true;

                    flagAttack.hitting = false;

                    break;
                }
            }
        }
    }
}

