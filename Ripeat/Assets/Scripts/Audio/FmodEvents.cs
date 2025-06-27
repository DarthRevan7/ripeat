using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    [field: SerializeField] public EventReference mainEnemyFootsteps { get; private set; }
    [field: SerializeField] public EventReference playerPunch { get; private set; }
    [field: SerializeField] public EventReference mainEnemyPunch { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene.");
        }
        instance = this;
    }
}