using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ResponseEvent 
{
    [HideInInspector] public string name;
    [SerializeField] private UnityEvent<int> onPickedResponse;
    

    public UnityEvent<int> OnPickedResponse => onPickedResponse;
}
