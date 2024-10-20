using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitterShotgun", menuName = "ScriptableObjects/Abilities/SplitterShotgun", order = 1)]
public class SplitterSO : AbilitiesSO
{
    [System.Serializable] public class EnhanceUpgrade
    {
        public BaseStat LevelUp;
        public float BulletLifetime = 0.65f;
        public int BulletNumb = 5;
        public int Radius = 7;
        public int Bounce = 0;
    }

    [Header("Weapon Set Up")]
    public float BulletLifetime = 0.65f;
    public int BulletNumb = 5;
    public int Radius = 7;
    public int Bounce = 0;

    [Header("Weapon Upgrade Path")]
    [Tooltip("All the stats inside are additional(Except cooldown), so just set 2 or -2 ")]
    public EnhanceUpgrade[] NormalUpgrade;
    public EnhanceUpgrade[] ElitePath1Upgrade;
    public EnhanceUpgrade[] ElitePath2Upgrade;
}
