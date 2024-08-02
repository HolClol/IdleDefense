using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UpgradesListData", menuName = "ScriptableObjects/Upgrades/UpgradeList", order = 1)]
public class UpgradesScriptableObject : ScriptableObject 
{
    public List<UpgradeSO> UpgradeInfoTable;
}
