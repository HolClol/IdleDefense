using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AbilityStat", menuName = "ScriptableObjects/Abilities/AbilityStat", order = 1)]
public class AbilitiesSO : ScriptableObject
{
    [System.Serializable] public class BaseStat
    { 
        public int Damage;
        public float Cooldown;
        public float Knockback;
        public float DamageScaling = 0f;
        [Range(0f, 1f)] public float CritRate = 0f;
        public float CritDamage = 0f;
        public IntVariable DamageType;
    }
    [System.Serializable] public class NormalEnhanceUpgrade
    {
        public BaseStat LevelUp;
    }
    [System.Serializable] public class AbilitiesStatClass
    {
        public GameObject[] ObjectsPrefab;
        public int ID;
        public int EliteID;
        public BaseStat Stats;
    }

    public AbilitiesStatClass AbilityData;
}
