using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityStat", menuName = "ScriptableObjects/Abilities/AbilityStat", order = 1)]
public class AbilitiesSO : ScriptableObject
{
    [System.Serializable] public class AbilitiesStatClass
    {
        public GameObject[] ObjectsPrefab;
        public int Damage;
        public int ID;
        public int EliteID;
        public float Cooldown;
        public float Knockback;
        public float DamageScaling = 0f;
        [Range(0f, 1f)] public float CritRate = 0.05f;
        public float CritDamage = 1f;
        public IntVariable DamageType;
    }

    public AbilitiesStatClass AbilityData;
}
