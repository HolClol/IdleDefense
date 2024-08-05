using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseGameEventListener : MonoBehaviour
{
    public GameEvent Event;
    protected virtual void OnEnable()
    { Event.RegisterListener(this); }

    protected virtual void OnDisable()
    { Event.UnregisterListener(this); }

    public virtual void OnEventRaised()
    { 
    }
}
