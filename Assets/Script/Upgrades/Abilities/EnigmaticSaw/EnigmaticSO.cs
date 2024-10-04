using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnigmaticSaw", menuName = "ScriptableObjects/Abilities/EnigmaticSaw", order = 1)]
public class EnigmaticSO : AbilitiesSO
{
    [System.Serializable] public class EnhanceUpgrade
    {
        public BaseStat LevelUp;
        public int RazorbladeNumbers = 1;
        public float AdditionalScale = 0f;
        public float Duration = 5f;
        public float DamageInterval = 0.6f;
        public float Speed = 20f;
    }

    [Header("Weapon Set Up")]
    public int RazorbladeNumbers = 1;
    public float AdditionalScale = 0f;
    public float Duration = 5f;
    public float DamageInterval = 0.6f;
    public float Speed = 20f;

    [Header("Weapon Upgrade Path")]
    [Tooltip("All the stats inside are additional(Except cooldown), so just set 2 or -2 ")]
    public EnhanceUpgrade[] NormalUpgrade;
    public EnhanceUpgrade[] ElitePath1Upgrade;
    public EnhanceUpgrade[] ElitePath2Upgrade;
}
