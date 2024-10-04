using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LancerBeam", menuName = "ScriptableObjects/Abilities/LancerBeam", order = 1)]
public class LancerBeamSO : AbilitiesSO
{
    [System.Serializable] public class EnhanceUpgrade
    {
        public BaseStat LevelUp;
        public float BeamLifetime = 4f;
        public float DamageInterval = 0.5f;
        public float SizeIncrease = 0f;
        public int Radius = 12;
        public int BeamCount = 1;
        public int Piercing = 0;
        public int FractionBeam = 3;
    }

    [Header("Weapon Set Up")]
    public float BeamLifetime = 4f;
    public float DamageInterval = 0.5f;
    public float SizeIncrease = 0f;
    public int Radius = 12;
    public int BeamCount = 1;
    public int Piercing = 0;
    public int FractionBeam = 3;

    [Header("Weapon Upgrade Path")]
    [Tooltip("All the stats inside are additional(Except cooldown), so just set 2 or -2 ")]
    public EnhanceUpgrade[] NormalUpgrade;
    public EnhanceUpgrade[] ElitePath1Upgrade;
    public EnhanceUpgrade[] ElitePath2Upgrade;
}
