using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerFloat : BaseGameEventListener
{
    public UnityEvent<float[]> Response;

    public void OnEventFloatRaised(float[] floatstat)
    { Response.Invoke(floatstat); }

}
