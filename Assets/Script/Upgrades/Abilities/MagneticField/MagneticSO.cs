using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagneticField", menuName = "ScriptableObjects/Abilities/MagneticField", order = 1)]
public class MagneticSO : AbilitiesSO
{
    [Header("Weapon Set Up")]
    public int NumbOfOrbs = 4;
    public int Piercing = 0;
    public float Duration = 6f;
    public float OrbMoveSpeed = 4f;
    public float OrbRecover = 2f;
    public float SizeBuff = 1f;
}
