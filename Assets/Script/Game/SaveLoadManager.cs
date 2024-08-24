using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SavedGameState
{
    public int Version = 1;
}

public enum ESaveSlot
{
    None,

    Slot1,
    Slot2,
    Slot3,
    Slot4,
    Slot5
}

public enum ESaveType
{
    Manual,
    Automatic
}

public class SaveLoadManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
