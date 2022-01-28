using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


class EventManager : MonoBehaviour
{

    private Dictionary<string, UnityEvent> eventDict;
    public static EventManager Instance;

    void Awake()
    {
	if(Instance != null) DestroyImmediate(this);
        Instance = this;

        eventDict = new Dictionary<string, UnityEvent>();
    }

    public void StartListening(string eventName, UnityAction listener)
    {
	UnityEvent thisEvent = null;
        if (eventDict.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        } else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventDict.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (eventDict.TryGetValue(eventName, out thisEvent))
        {
	    thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (eventDict.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

}
