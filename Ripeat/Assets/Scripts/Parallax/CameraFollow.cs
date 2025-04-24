using UnityEngine;

public class CameraFollowWithBounds : MonoBehaviour
{
    [SerializeField] private Transform target; // Il personaggio da seguire
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private float xOffset = 0f;

    [Header("Confini laterali")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;

    private Vector3 initialPosition;

    private void Start()
    {
        // Salva la posizione iniziale della camera per mantenere Y e Z
        initialPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Calcola la X desiderata con offset
        float desiredX = target.position.x + xOffset;

        // Applica Lerp per movimento smooth
        float smoothedX = Mathf.Lerp(transform.position.x, desiredX, smoothSpeed);

        // Applica limiti
        smoothedX = Mathf.Clamp(smoothedX, minX, maxX);

        // Mantieni Y e Z dalla posizione iniziale
        Vector3 newPosition = new Vector3(smoothedX, initialPosition.y, initialPosition.z);
        transform.position = newPosition;
    }
}
