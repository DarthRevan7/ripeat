using UnityEngine;

public class CombatAnimSystem : MonoBehaviour
{
    public enum CombatAnimState
    {
        IDLE, PUNCH, KICK, BLOCK, MOVING, DEAD
    }

    [SerializeField] private CombatAnimState combatState;

    public CombatAnimState CurrentState
    {
        get { return combatState; }
        set
        {
            combatState = value;
        }
    }

    [SerializeField] private Animator animator;
    [SerializeField] private string punchAnimName, kickAnimName, blockAnimName, deadAnimName, idleAnimName, runAnimName;

    /*
    
    Serve per realizzare la reattività del fight:
    0 -> Animazione default (idle)
    1 -> Pre-Execution (l'azione può essere modificata)
    2 -> Execution
    3 -> Post-Execution (calcolo danni e knockback)

    Nel caso spostiamo calcolo danni in execution? Da capire.
    
    */
    [SerializeField] private int animState = 0;

    public void SetAnimState(int numState)
    {
        animState = numState;
        // AnimationTest();
    }

    public int GetAnimState()
    {
        return animState;
    }

    public void SetBlockBool(bool block)
    {
        animator.SetBool("Blocking", block);
    }



    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RequestStateChange(CombatAnimState state)
    {
        if (StateChangeCheck())
        {
            CurrentState = state;
            ExecuteAnimationChange();
        }
    }
    /*
    La window of opportunity per tentare di cambiare stato è in pre-execution (oppure anticipation).
    */
    public bool StateChangeCheck()
    {
        return animState < 2 && CurrentState != CombatAnimState.DEAD;
    }
    public void ExecuteAnimationChange()
    {
        if (!StateChangeCheck())
            return;
        //Ferma l'animazione corrente (devo trovare il metodo adatto da chiamare)
            switch (CurrentState)
            {
                case CombatAnimState.PUNCH:
                    animator.SetBool("Run", false);
                    animator.Play(punchAnimName);
                    break;
                case CombatAnimState.KICK:
                    animator.SetBool("Run", false);
                    animator.Play(kickAnimName);
                    break;
                case CombatAnimState.BLOCK:
                    animator.SetBool("Run", false);
                    animator.Play(blockAnimName);
                    break;
                case CombatAnimState.MOVING:
                    animator.SetBool("Run", true);
                    break;
                case CombatAnimState.DEAD:
                    animator.SetBool("Run", false);
                    animator.SetTrigger("Die");
                    break;
                default:
                    animator.SetBool("Run", false);
                    break;
            }
    }

    public void Die()
    {
        animator.SetBool("Run", false);
        // animator.SetBool("Blocking", false);
        combatState = CombatAnimState.DEAD;
        animator.SetTrigger("Die");
        animator.SetBool("IsDead", true);
    }
    
    #region Test
    void AnimationTest()
    {
        Debug.Log(animState.ToString() + " State");
    }
    #endregion
}
