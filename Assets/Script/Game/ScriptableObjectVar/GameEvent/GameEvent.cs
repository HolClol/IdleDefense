using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameEvent", menuName = "ScriptableObjects/VariablesType/GameEvent", order = 3)]
public class GameEvent : ScriptableObject
{
    private List<BaseGameEventListener> listeners = new List<BaseGameEventListener>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised();
    }

    public void Raise(int[] intstat)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            if (listeners[i] is GameEventListenerInt)
            {
                GameEventListenerInt gameEventListenerInt = listeners[i] as GameEventListenerInt;
                gameEventListenerInt.OnEventIntRaised(intstat);
            }
            
    }
    public void Raise(float[] floatstat)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            if (listeners[i] is GameEventListenerFloat)
            {
                GameEventListenerFloat gameEventListenerInt = listeners[i] as GameEventListenerFloat;
                gameEventListenerInt.OnEventFloatRaised(floatstat);
            }
    }

    public void Raise(GameObject target, int[] intstat, float[] floatstat)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            if (listeners[i] is GameEventListenerDamage)
            {
                GameEventListenerDamage gameEventListenerInt = listeners[i] as GameEventListenerDamage;
                gameEventListenerInt.OnEventDamageRaised(target, intstat, floatstat);
            }
    }


    public void RegisterListener(BaseGameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(BaseGameEventListener listener)
    {
        listeners.Remove(listener);
    }

}
