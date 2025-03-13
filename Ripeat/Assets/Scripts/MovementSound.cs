using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;


public class MovementSound : MonoBehaviour
{
    [SerializeField] private Vector3 playerMovement;
    [SerializeField] public EnemyBehaviour.EnemyStatus enemyStatus;

    private EventInstance playerFootsteps;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GameObject.FindAnyObjectByType<PlayerMovement>().movement;
        enemyStatus = GameObject.FindAnyObjectByType<EnemyBehaviour>().GetEnemyStatus();

        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
    }

    // Update is called once per frame
    void Update()
    {
        // Ricavo lo stato attuale del giocatore e del nemico.
        playerMovement = GameObject.FindAnyObjectByType<PlayerMovement>().movement;
        enemyStatus = GameObject.FindAnyObjectByType<EnemyBehaviour>().GetEnemyStatus();

        if (playerMovement != Vector3.zero)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }

            Debug.Log("Il giocatore si muove. Senti il suono dei passi");
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }


        if (enemyStatus == EnemyBehaviour.EnemyStatus.FOLLOWING_PLAYER)
        {
            // Inserisci qui le righe per riprodurre l'audio dei passi del nemico
            // Debug.Log("Il nemico si muove. Senti il suono dei passi");
        }
    }
}
