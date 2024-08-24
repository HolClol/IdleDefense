using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameEventListenerString : BaseGameEventListener
{
    public UnityEvent<string[]> StringResponse;

    public void OnEventStringRaised(string[] stringstat)
    {
        StringResponse?.Invoke(stringstat);
    }
}
