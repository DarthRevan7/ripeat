using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;


public class MovementSound : MonoBehaviour
{
   
    private EventInstance playerFootsteps;
    [SerializeField] private CombatSystem player, mainEnemy, secondaryEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("MyPlayerNew").GetComponent<CombatSystem>();
        mainEnemy = GameObject.Find("MyEnemyNew").GetComponent<CombatSystem>();

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
       //MOD.ATTRIBUTES_3D aTTRIBUTES_3D = new FMOD.ATTRIBUTES_3D();
      //playerFootsteps.set3DAttributes(aTTRIBUTES_3D);

    }

    // Update is called once per frame
    void Update()
    {
        // Ricavo lo stato attuale del giocatore e del nemico.
       
        if (player.currentState == CombatSystem.CharacterState.MOVING)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            //if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            if(playbackState == PLAYBACK_STATE.STOPPED)
            {
                playerFootsteps.start();
            }

            Debug.Log("Il giocatore si muove. Senti il suono dei passi");
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }


        if (mainEnemy.currentState == CombatSystem.CharacterState.MOVING)
        {
            // Inserisci qui le righe per riprodurre l'audio dei passi del nemico
            // Debug.Log("Il nemico si muove. Senti il suono dei passi");
        }
    }
}
