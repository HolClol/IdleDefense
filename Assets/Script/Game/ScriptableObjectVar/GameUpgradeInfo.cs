using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeInfo", menuName = "ScriptableObjects/Upgrades/UpgradeInfo")]
public class GameUpgradeInfo : ScriptableObject
{
    [System.Serializable]
    public class AbilitiesInfo
    {
        public int ID;
        public int UpgradeLevel;
        public int Damage;
        
        public AbilitiesInfo(int id, int level, int damage)
        {
            UpgradeLevel = level;
            Damage = damage;
            ID = id;

        }
    }
    public int TotalDamage;
    public List<AbilitiesInfo> AbilitiesStat = new List<AbilitiesInfo>();

    public void Reset()
    {
        AbilitiesStat.Clear();
        TotalDamage = 0;
    }
}
