using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObjects/Upgrades/SingleUpgrade", order = 1)]
public class UpgradeSO : ScriptableObject
{
    public enum UpgradeTypeEnum { Stat, Abilities, Passive, Weapon }
    [System.Serializable] public class UpgradeInfo
    {
        public UpgradeTypeEnum UpgradeType;
        public string UpgradeName;
        public string[] UpgradeDescription;
        public int MaxLevel;
        public int ID;

    }
    public UpgradeInfo Upgrade;
}
