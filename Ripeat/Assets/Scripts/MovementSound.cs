using UnityEngine;
using UnityEngine.InputSystem;

public class MovementSound : MonoBehaviour
{
    [SerializeField] private Vector3 playerMovement;
    [SerializeField] private EnemyBehaviour.EnemyStatus enemyStatus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = GameObject.FindAnyObjectByType<PlayerMovement>().movement;
        enemyStatus = GameObject.FindAnyObjectByType<EnemyBehaviour>().GetEnemyStatus();
    }

    // Update is called once per frame
    void Update()
    {
        // Ricavo lo stato attuale del giocatore e del nemico.
        playerMovement = GameObject.FindAnyObjectByType<PlayerMovement>().movement;
        enemyStatus = GameObject.FindAnyObjectByType<EnemyBehaviour>().GetEnemyStatus();

        if (playerMovement != Vector3.zero)
        {
            // Inserisci qui le righe per riprodurre l'audio dei passi del giocatore
            // Debug.Log("Il giocatore si muove. Senti il suono dei passi");
        }

        if (enemyStatus == EnemyBehaviour.EnemyStatus.FOLLOWING_PLAYER)
        {
            // Inserisci qui le righe per riprodurre l'audio dei passi del nemico
            // Debug.Log("Il nemico si muove. Senti il suono dei passi");
        }
    }
}