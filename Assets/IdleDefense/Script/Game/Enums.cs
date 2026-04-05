using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumDataType
{
    Health,
    Shield,
    Experience,
    MaxHealth,
    Coins,
    Points,
    EnemyEliminated, // Functionally should run with one of the two above, but its nice being flexible
    Rage,
}

public enum UpgradeTypeEnum 
{ 
    Stat, 
    Abilities, 
    Artifacts, 
    Weapon 
}

public enum WeaponTypeEnum
{
    None, 
    Drone, 
    Missile, 
    Projectile, 
    Beam
}

public enum DamageTypeEnum
{
    None, 
    Pierce, 
    Slash, 
    Energy, 
    Blast
}
