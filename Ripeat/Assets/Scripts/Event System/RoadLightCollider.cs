using UnityEngine;

public class RoadLightCollider : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag.Equals("Player"))
        {
            EventHandler.Instance.streetlamp = gameObject;
        }
    }
}
