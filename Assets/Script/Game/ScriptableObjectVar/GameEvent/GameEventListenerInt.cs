using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerInt : BaseGameEventListener
{
    public UnityEvent<int[]> Response;

    public void OnEventIntRaised(int[] intstat)
    { Response.Invoke(intstat); }

}

