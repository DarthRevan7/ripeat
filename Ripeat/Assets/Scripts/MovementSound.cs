using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;


public class MovementSound : MonoBehaviour
{

    private EventInstance playerFootsteps, mainEnemyFootsteps, secondaryEnemyFootsteps;
    [SerializeField] private EventInstance playerPunch, mainEnemyPunch, secondaryEnemyPunch;
    [SerializeField] private CombatAnimSystem player, mainEnemy, secondaryEnemy;
    [SerializeField] private string playerGameObjectTag = "Player",
        mainEnemyGameObjectTag = "Main Enemy", 
        secondaryEnemyGameObjectTag = "Secondary Enemy";
    [SerializeField] private bool debugActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerGameObjectTag).GetComponent<CombatAnimSystem>();
        mainEnemy = GameObject.FindGameObjectWithTag(mainEnemyGameObjectTag).GetComponent<CombatAnimSystem>();

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
        mainEnemyFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.mainEnemyFootsteps);
        //MOD.ATTRIBUTES_3D aTTRIBUTES_3D = new FMOD.ATTRIBUTES_3D();
        //playerFootsteps.set3DAttributes(aTTRIBUTES_3D);

        playerPunch = AudioManager.instance.CreateInstance(FMODEvents.instance.playerPunch);
        mainEnemyPunch = AudioManager.instance.CreateInstance(FMODEvents.instance.mainEnemyPunch);
    }

    // Update is called once per frame
    void Update()
    {
        // Ricavo lo stato attuale del giocatore e del nemico.

        if (player.CurrentState == CombatAnimSystem.CombatAnimState.MOVING)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            //if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                playerFootsteps.start();
            }

            if (debugActive)
            {
                Debug.Log("Il giocatore si muove. Senti il suono dei passi");
            }
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }

        if (player.CurrentState == CombatAnimSystem.CombatAnimState.PUNCH ||
            player.CurrentState == CombatAnimSystem.CombatAnimState.KICK)
        {
            PLAYBACK_STATE playbackState;
            playerPunch.getPlaybackState(out playbackState);
            //if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                playerPunch.start();
            }

            if (debugActive)
            {
                Debug.Log("Il protag picchia. Senti il suono dei pugni");
            }
        }

        if (mainEnemy.CurrentState == CombatAnimSystem.CombatAnimState.MOVING)
        {
            PLAYBACK_STATE playbackState;
            mainEnemyFootsteps.getPlaybackState(out playbackState);
            //if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                mainEnemyFootsteps.start();
            }

            if (debugActive)
            {
                Debug.Log("Il nemico picchia. Senti il suono dei pugni");
            }
        }

        if (mainEnemy.CurrentState == CombatAnimSystem.CombatAnimState.PUNCH ||
            mainEnemy.CurrentState == CombatAnimSystem.CombatAnimState.KICK)
        {
            PLAYBACK_STATE playbackState;
            mainEnemyPunch.getPlaybackState(out playbackState); 
            //if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                mainEnemyPunch.start();
            }

            if (debugActive)
            {
                Debug.Log("Il nemico si muove. Senti il suono dei passi");
            }
        }

         // Inserisci qui le righe per riprodurre l'audio dei passi del nemico
        // Debug.Log("Il nemico si muove. Senti il suono dei passi");
    }
}

