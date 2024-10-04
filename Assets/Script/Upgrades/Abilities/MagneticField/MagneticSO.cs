using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagneticField", menuName = "ScriptableObjects/Abilities/MagneticField", order = 1)]
public class MagneticSO : AbilitiesSO
{
    [System.Serializable] public class EnhanceUpgrade
    {
        public BaseStat LevelUp;
        public int NumbOfOrbs = 4;
        public int NumbOfMini = 3;
        public int Piercing = 0;
        public float Duration = 6f;
        public float OrbMoveSpeed = 4f;
        public float OrbRecover = 2f;
        public float SizeBuff = 1f;
    }

    [Header("Weapon Set Up")]
    public int NumbOfOrbs = 4;
    public int NumbOfMini = 3;
    public int Piercing = 0;
    public float Duration = 6f;
    public float OrbMoveSpeed = 4f;
    public float OrbRecover = 2f;
    public float SizeBuff = 1f;

    [Header("Weapon Upgrade Path")]
    [Tooltip("All the stats inside are additional(Except cooldown), so just set 2 or -2 ")]
    public EnhanceUpgrade[] NormalUpgrade;
    public EnhanceUpgrade[] ElitePath1Upgrade;
    public EnhanceUpgrade[] ElitePath2Upgrade;
}
