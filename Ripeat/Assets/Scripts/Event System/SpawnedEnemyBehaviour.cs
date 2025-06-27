using UnityEngine;

public class SpawnedEnemyBehaviour : MonoBehaviour
{

    [SerializeField] private CombatAnimSystem combatSystem;

    
    void Awake()
    {
        combatSystem = GetComponent<CombatAnimSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(combatSystem.CurrentState == CombatAnimSystem.CombatAnimState.DEAD)
        {
            // FightEventController.Instance.globalEventIndex++;
            this.enabled = false;
        }
    }
}
