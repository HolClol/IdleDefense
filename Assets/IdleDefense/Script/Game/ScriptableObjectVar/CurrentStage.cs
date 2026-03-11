using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentStage", menuName = "ScriptableObjects/VariablesType/Stage")]
public class CurrentStage : ScriptableObject
{
    public StageInfo Stage;

    public void ResetStage()
    {
        Stage = null;
    }
}
