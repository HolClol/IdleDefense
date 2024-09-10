using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FieryEruption", menuName = "ScriptableObjects/Abilities/FieryEruption", order = 1)]
public class EruptionSO : AbilitiesSO
{
    [Header("Weapon Set Up")]
    public int AdditionalEruptions = 0;
    public float GroundDuration = 0;

    public Vector3 DecreaseScale = new Vector3(0.6f, 0.6f, 0f);
    public Vector3 IncreaseScale = new Vector3(0f, 0f, 0f);
}
