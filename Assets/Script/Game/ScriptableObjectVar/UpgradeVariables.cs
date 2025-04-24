using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatVariables
{
    Damage,
    Cooldown,
    Knockback,
    DamageScaling,
    CritRate,
    CritDamage,
    DamageType,
    Radius,
    Duration,
    DamageInterval,
    Scale,
    // Homing Missiles \\ 
    MissileNumbers,
    InternalExplode,
    // Lancer Beam \\
    BeamLifetime,
    BeamCount,
    Piercing,
    FractionBeam,
    // Splitter Shotgun \\
    Bounce,
    BulletLifetime,
    BulletNumb,
    // Enigmatic Saw \\
    RazorbladeCount,
    Speed,
    // Magnetic Field \\
    NumbOfOrbs,
    NumbOfMini,
    OrbMoveSpeed,
    OrbRecover,
    // Ground Eruption
    AdditionalEruptions,
    GroundDuration,
    DecreaseScale,
    IncreaseScale,
}

public enum AbilityProfile
{
    Base,
    EngimaticSaw,
    HomingMissiles,
    MagneticField,
    LancerBeam,
    SplitterShotgun,
    FieryEruption,
}

[CreateAssetMenu(fileName = "UpgradeTable", menuName = "ScriptableObjects/Upgrades/UpgradeTable", order = 1)]
public class UpgradeVariables : ScriptableObject
{
    [System.Serializable] public class UpgradeStat
    {
        public StatVariables Stat;
        public float Value;
    }

    public AbilityProfile AbilityProfile;
    public UpgradeStat[] UpgradeTable;
}
