using UnityEngine;

[RequireComponent(typeof(CombatSystem))]
public class SpawnedEnemyBehaviour : MonoBehaviour
{

    [SerializeField] private CombatSystem combatSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combatSystem = GetComponent<CombatSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(combatSystem.isDead)
        {
            FightEventController.globalEventIndex++;
            this.enabled = false;
        }
    }
}
