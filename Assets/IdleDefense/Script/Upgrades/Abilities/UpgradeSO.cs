using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElitePathOptions
{
    public string EliteUpgradeName;
    public string[] UpgradeDescription;
    public int AlternateID;
    public int MaxLevel;
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/Upgrades/SingleUpgrade", order = 1)]
public class UpgradeSO : ScriptableObject
{
    public enum UpgradeTypeEnum { Stat, Abilities, Artifacts, Weapon }
    public enum WeaponTypeEnum { None, Drone, Missile, Projectile, Beam}
    public enum DamageTypeEnum { None, Pierce, Slash, Energy, Blast }

    public UpgradeTypeEnum UpgradeType;
    public WeaponTypeEnum WeaponType;
    public DamageTypeEnum DamageType;
    public string UpgradeName;
    public string[] UpgradeDescription;
    public int MaxLevel;
    public int ID;
    public ElitePathOptions[] ElitePath;

}
