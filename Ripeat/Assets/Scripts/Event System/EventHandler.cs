using System.Collections.Generic;
using FightEventNamespace;
using UnityEngine;

public class EventHandler : MonoBehaviour
{

    [SerializeField] private List<FightEvent> fightEvents;

    [SerializeField] private int currentEventIndex = 0;
    [SerializeField] private int globalEventIndex;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
