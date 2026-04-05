using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class GameData
{
    public int Coins;
    public int UserRank;
    public int UserPoints;

    public GameData()
    {
        this.Coins = 0;
        this.UserRank = 1;
        this.UserPoints = 0;
    }
}

[System.Serializable] public class PlayerUpgradeStat {
    public int UpgradeID;
    public int UpgradeLevel;
    public int ElitePath; // IT WE KEEP IT

    public PlayerUpgradeStat(int id, int level, int eliteid) {
        UpgradeID = id;
        UpgradeLevel = level;
        ElitePath = eliteid;
    }
}
[System.Serializable] public class PlayerInGameStat {
    public int BaseDamage = 20;
    [Range(10, 40)]
    public float RotateSpeed = 20;
    public List<PlayerUpgradeStat> Upgrades;
}

[System.Serializable] public class ElitePathOptions
{
    public string EliteUpgradeName;
    public string[] UpgradeDescription;
    public int AlternateID;
    public int MaxLevel;
}

[System.Serializable] public class AbilitiesStat
{
    public int UpgradeLevel;
    public int Damage;

    public AbilitiesStat(int level, int damage)
    {
        UpgradeLevel = level;
        Damage = damage;
    }
}

