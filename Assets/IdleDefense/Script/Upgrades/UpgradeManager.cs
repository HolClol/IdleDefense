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
    private AbilitiesManager _abilitiesHolder;
    private WeaponController _weaponHolder;
    private List<int> CurrentIDUpgradeTable = new List<int>();
    private List<int> ObtainedUpgrade = new List<int>();
    private int RandomUpgradeIndex, Level, LevelSelect, EliteIndex;
    private bool GuaranteeUpgrade;
    private bool Elite;
    
    private void Start()
    {
        MainPlayer = GameObject.Find("Player").gameObject;
        PlayerStat = MainPlayer.GetComponent<PlayerGameStat>();
        _abilitiesHolder = MainPlayer.GetComponentInChildren<AbilitiesManager>();
        _weaponHolder = MainPlayer.GetComponentInChildren<WeaponController>();
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

    // Reset the table from public
    public void ResetTable()
    {
        CurrentIDUpgradeTable.Clear();
    }

    // Receive back the selected upgrade from UI
    public void UpgradeOptionSelected(int ID, int eliteID) {
        ResetTable();
        int realID = UpgradesData.UpgradeInfoTable[ID].ID;
        bool unlockEliteIndex = PlayerStat.EliteUnlock(realID, eliteID);
        bool isAbility = false;
        var abilityHolder = MainPlayer.transform.Find("AbilitiesHolder");
        Level = PlayerStat.UpgradeAbility(realID);
        UpdateLevelUI.Invoke(new int[] { 1, realID });     
        
        #region ID assign NOTE
        /*
            ID = 1 => HomingMissiles
            ID = 2 => FieryEruption
            ID = 3 => MagneticField
            ID = 4 => SplitterShotgun
            ID = 5 => EnigmaticSaw
            ID = 6 => LancerBeam
        */
        #endregion
        
        // 1-19 is assumed to be all abilities
        if (realID > 0 && realID < 20)
        {
            isAbility = true;
        }
        
        switch (realID)
        { //Most upgrades does not need a special case and is calculated through the projectile handler itself
            case 0: //Weapon Upgrade
                if (unlockEliteIndex)
                    _weaponHolder.UnlockELite(eliteID);
                _weaponHolder.CheckUpgrade(LevelSelect);
                break;
            case 20:
                UpdateStat.Invoke(new int[] { 3, 20 });
                break;
            case 21:
                PlayerStat.UpdateDamage(new int[] { 5 });
                break;
        }
        
        if (isAbility)
        {
            if (unlockEliteIndex)
                _abilitiesHolder.UnlockElite(realID, eliteID);
            switch (Level)
            {
                case 0:
                    _abilitiesHolder.ActivateAbility(realID);
                    break;
                default:
                    _abilitiesHolder.UpgradeAbility(realID, Level - 1); //Real upgrade value is smaller by 1
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
