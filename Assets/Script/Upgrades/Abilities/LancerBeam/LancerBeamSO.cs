using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LancerBeam", menuName = "ScriptableObjects/Abilities/LancerBeam", order = 1)]
public class LancerBeamSO : AbilitiesSO
{
    [System.Serializable] public class SpecialProperties
    {

    }
    [Header("Weapon Set Up")]
    public float BeamLifetime = 4f;
    public float DamageInterval = 0.5f;
    public int Radius = 12;
    public int BeamCount = 1;
    public int Piercing = 0;
    public int FractionBeam = 3;
}
