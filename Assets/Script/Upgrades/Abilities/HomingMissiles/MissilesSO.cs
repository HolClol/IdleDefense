using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomingMissiles", menuName = "ScriptableObjects/Abilities/HomingMissiles", order = 1)]
public class MissilesSO : AbilitiesSO
{
    [Header("Weapon Set Up")]
    public int MissileNumbers = 3;
    public float InternalExplode = 1f;

    public Vector3 AdditionalScale = new Vector3(0f, 0f, 0f);
}
