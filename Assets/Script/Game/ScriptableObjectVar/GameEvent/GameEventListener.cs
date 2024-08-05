using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : BaseGameEventListener
{
    public UnityEvent Response;

    public override void OnEventRaised()
    { Response.Invoke(); }

}

