using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerDamage : BaseGameEventListener
{
    public UnityEvent<GameObject, int[], float[]> ResponseDamage;

    public void OnEventDamageRaised(GameObject target, int[] intstat, float[] floatstat)
    { ResponseDamage.Invoke(target, intstat, floatstat); }
}
