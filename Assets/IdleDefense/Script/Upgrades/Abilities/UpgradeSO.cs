using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/Upgrades/SingleUpgrade", order = 1)]
public class UpgradeSO : ScriptableObject
{
    public UpgradeTypeEnum UpgradeType;
    public WeaponTypeEnum WeaponType;
    public DamageTypeEnum DamageType;
    public string UpgradeName;
    public string[] UpgradeDescription;
    public int MaxLevel;
    public int ID;
    public ElitePathOptions[] ElitePath;

}
