using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityStat", menuName = "ScriptableObjects/Abilities/AbilityStat", order = 1)]
public class AbilitiesSO : ScriptableObject
{
    [System.Serializable]
    public class AbilitiesStatClass
    {
        public GameObject[] ObjectsPrefab;
        public int Damage;
        public int ID;
        public int EliteID;
        public float Cooldown;
        public float Knockback;
        [Range(0f, 1f)] public float CritRate = 0.05f;
        public float CritDamage = 1f;
        public IntVariable DamageType;

        public AbilitiesStatClass(int dmg, float cd, float kb, int id, int eliteID)
        {
            Damage = dmg;
            Cooldown = cd;
            Knockback = kb;
            ID = id;
            EliteID = eliteID;
        }
    }
    public AbilitiesStatClass AbilitiesStat;

}
