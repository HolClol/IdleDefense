using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public UpgradesScriptableObject UpgradesData;
    public UnityEvent<int[]> UpdateStat;
    public UnityEvent<int[]> UpdateLevelUI;

    private PlayerGameStat PlayerStat;
    private GameObject MainPlayer;
    private List<int> CurrentIDUpgradeTable = new List<int>();
    private List<int> ObtainedUpgrade = new List<int>();
    private int RandomUpgradeIndex, Level, LevelSelect, EliteIndex;
    private bool GuaranteeUpgrade = false;
    private bool Elite = false;
    
    private void Start()
    {
        MainPlayer = GameObject.Find("Player").gameObject;
        PlayerStat = MainPlayer.GetComponent<PlayerGameStat>();
    }

    //Select a random upgrade in the table (This should run in the first info collect signal)
    private int UpgradeOptionRandom() {
        int random = Random.Range(0, UpgradesData.UpgradeInfoTable.Count);
        int maxlevel = UpgradesData.UpgradeInfoTable[random].MaxLevel;
        int realmaxlevel = Totalmaxlevel(random);
        //int index = CurrentIDUpgradeTable.Count;

        Level = PlayerStat.GetUpgradeLevel(UpgradesData.UpgradeInfoTable[random].ID);
        Elite = false;
        EliteIndex = 0;

        while (Level >= realmaxlevel || CurrentIDUpgradeTable.Contains(random))
        {  
            if (GuaranteeUpgrade && CurrentIDUpgradeTable.Count == 3)
                random = ObtainedUpgrade[Random.Range(0, ObtainedUpgrade.Count)];
            else
                random = Random.Range(0, UpgradesData.UpgradeInfoTable.Count);

            maxlevel = UpgradesData.UpgradeInfoTable[random].MaxLevel;
            realmaxlevel = Totalmaxlevel(random);
            Level = PlayerStat.GetUpgradeLevel(UpgradesData.UpgradeInfoTable[random].ID);
        }
        
        CurrentIDUpgradeTable.Add(random);

        if (ObtainedUpgrade.Contains(random) && GuaranteeUpgrade)
            GuaranteeUpgrade = false;

        if (random == 0 && Level < maxlevel)
            LevelSelect = Random.Range(1, maxlevel);
        if (UpgradesData.UpgradeInfoTable[random].ElitePath.Length <= 0 || Level < maxlevel)
            return random;

        // Entering the Elite path domain
        LevelSelect = Level;
        Elite = true;
        if (Level == maxlevel)
            EliteIndex = Random.Range(0, UpgradesData.UpgradeInfoTable[random].ElitePath.Length);
        else if (Level > maxlevel)
            EliteIndex = PlayerStat.GetEliteID(UpgradesData.UpgradeInfoTable[random].ID);
        
        return random;
    }
    private int Totalmaxlevel(int number)
    {
        if (UpgradesData.UpgradeInfoTable[number].ElitePath.Length > 0)
            return UpgradesData.UpgradeInfoTable[number].MaxLevel + UpgradesData.UpgradeInfoTable[number].ElitePath[0].MaxLevel;
        else
            return UpgradesData.UpgradeInfoTable[number].MaxLevel;
    }

    private int LevelCheckScan(int id)
    {
        if (UpgradesData.UpgradeInfoTable[id].UpgradeType == UpgradeSO.UpgradeTypeEnum.Weapon)
            return LevelSelect;
        else
            return Level;
    }

    // Send the upgrade int informations to UI 
    public int[] UpgradeIntInfo() {
        RandomUpgradeIndex = UpgradeOptionRandom();
        int MaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].MaxLevel;
        int[] InfoInt = new int[] {RandomUpgradeIndex, Level, MaxLevel, EliteIndex};

        if (Elite)
        {
            int EliteID = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[EliteIndex].AlternateID;
            int OldMaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].MaxLevel;
            MaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[EliteIndex].MaxLevel;
            
            InfoInt = new int[] { RandomUpgradeIndex, Level - OldMaxLevel, MaxLevel, EliteID};
        }
        
        return InfoInt;
    }

    // Send the upgrade string informations to UI 
    public string[] UpgradeStringInfo() {
        int index = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeDescription.Length-1;
        int level = LevelCheckScan(RandomUpgradeIndex);
        if (index >= level) {
            index = level;
        }
        string[] InfoString = new string[] {UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeName, UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeDescription[index]};

        if (Elite)
        {
            index = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[EliteIndex].UpgradeDescription.Length - 1;
            int OldMaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].MaxLevel;
            if (index + OldMaxLevel >= level)
            {
                index = level - OldMaxLevel;
            }
            InfoString = new string[] { UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[EliteIndex].EliteUpgradeName, UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[EliteIndex].UpgradeDescription[index] };
        }

        return InfoString;
    }

    // Receive back the selected upgrade from UI
    public void UpgradeOptionSelected(int ID, int EliteID) {
        CurrentIDUpgradeTable.Clear();
        int RealId = UpgradesData.UpgradeInfoTable[ID].ID;
        bool UnlockEliteIndex = PlayerStat.EliteUnlock(RealId, EliteID);
        Level = PlayerStat.UpgradeAbility(RealId);
        UpdateLevelUI.Invoke(new int[] { 1, RealId });     

        string ScriptName = "";
        switch (RealId)
        { //Most upgrades does not need a special case and is calculated through the projectile handler itself
            case 0: //Weapon Upgrade
                if (UnlockEliteIndex)
                    MainPlayer.transform.Find("AbilitiesHolder").GetComponent<WeaponController>().UnlockELite(EliteID);
                MainPlayer.transform.Find("AbilitiesHolder").GetComponent<WeaponController>().CheckUpgrade(LevelSelect);
                break;
            case 1:
                ScriptName = "HomingMissilesController";
                break;
            case 2:
                ScriptName = "FieryEruptionController";
                break;
            case 3:
                ScriptName = "MagneticFieldController";
                break;
            case 4:
                ScriptName = "SplitterController";
                break;
            case 5:
                ScriptName = "EnigmaticSawController";
                break;
            case 6:
                ScriptName = "LancerBeamController";
                break;
            case 20:
                UpdateStat.Invoke(new int[] { 3, 20 });
                break;
            case 21:
                PlayerStat.UpdateDamage(new int[] { 5 });
                break;
        }

        AbilitiesController ScriptComponent;
        if (ScriptName != "")
        {
            ScriptComponent = MainPlayer.transform.Find("AbilitiesHolder").GetComponent(ScriptName) as AbilitiesController;
            if (UnlockEliteIndex)
                ScriptComponent.EliteUnlock(EliteID);
            switch (Level)
            {
                case 0:
                    ScriptComponent.enabled = true;
                    break;
                default:
                    ScriptComponent.CheckUpgrade(Level - 1); //Real upgrade value is smaller by 1
                    break;
            }
        }
        // Remove the selected upgrade from board if its level is maxed
        if (UpgradesData.UpgradeInfoTable[ID].MaxLevel <= Level && ObtainedUpgrade.Contains(ID))
                ObtainedUpgrade.Remove(ID);
        // Save selected upgrades to guarantee one in the next level up
        else if (UpgradesData.UpgradeInfoTable[ID].UpgradeType != UpgradeSO.UpgradeTypeEnum.Stat && !ObtainedUpgrade.Contains(ID))
            ObtainedUpgrade.Add(ID);

        int guarantee = Random.Range(0, 10);
        if (guarantee > 3 && ObtainedUpgrade.Count > 0)
            GuaranteeUpgrade = true;

    }
}
