using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerStruct<T> : BaseGameEventListener
{
    public UnityEvent<T> Response;

    public void OnEventStructRaised(T data)
    { Response.Invoke(data); }
}
