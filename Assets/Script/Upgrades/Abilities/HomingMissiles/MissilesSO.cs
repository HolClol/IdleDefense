using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomingMissiles", menuName = "ScriptableObjects/Abilities/HomingMissiles", order = 1)]
public class MissilesSO : AbilitiesSO
{
    [System.Serializable] public class EnhanceUpgrade : NormalEnhanceUpgrade
    {
        public int MissileNumbers = 3;
        public float InternalExplode = 1f;
        public Vector3 AdditionalScale = new Vector3(0f, 0f, 0f);
    }

    [Header("Weapon Set Up")]
    public int MissileNumbers = 3;
    public float InternalExplode = 1f;

    public Vector3 AdditionalScale = new Vector3(0f, 0f, 0f);

    [Header("Weapon Upgrade Path")]
    [Tooltip("All the stats inside are additional(Except cooldown), so just set 2 or -2 ")]
    public EnhanceUpgrade[] NormalUpgrade;
    public EnhanceUpgrade[] ElitePath1Upgrade;
    public EnhanceUpgrade[] ElitePath2Upgrade;
    public UpgradeVariables[] BaseUpgrade;
    public UpgradeVariables[] Elite1Upgrade;
    public UpgradeVariables[] Elite2Upgrade;
}
