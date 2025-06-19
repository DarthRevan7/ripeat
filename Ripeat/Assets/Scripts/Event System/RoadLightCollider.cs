using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadLightCollider : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag.Equals("Player") && SceneManager.GetActiveScene().name == "CombatScene")
        {
            EventHandler.Instance.streetlamp = gameObject;
        }
    }
}
