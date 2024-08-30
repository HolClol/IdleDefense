using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public UpgradesScriptableObject UpgradesData;
    public UnityEvent<int[]> UpdateStat;

    private PlayerGameStat PlayerStat;
    private GameObject MainPlayer;
    private List<int> CurrentIDUpgradeTable = new List<int>();
    private List<int> ObtainedUpgrade = new List<int>();
    private int RandomUpgradeIndex;
    private int Level;
    private int LevelSelect;
    private int ElitePath;
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
        int index = CurrentIDUpgradeTable.Count;
        Level = PlayerStat.GetUpgradeLevel(UpgradesData.UpgradeInfoTable[random].ID);

        while (Level >= maxlevel)
        {  
            if (GuaranteeUpgrade && CurrentIDUpgradeTable.Count > 2)
                random = ObtainedUpgrade[Random.Range(0, ObtainedUpgrade.Count - 1)];
            else
                random = RerollOption(random);

            maxlevel = UpgradesData.UpgradeInfoTable[random].MaxLevel;
            Level = PlayerStat.GetUpgradeLevel(UpgradesData.UpgradeInfoTable[random].ID);
        }
        
        CurrentIDUpgradeTable.Add(random);

        if (ObtainedUpgrade.Contains(random) && GuaranteeUpgrade)
            GuaranteeUpgrade = false;

        LevelSelect = Level;
        if (random == 0 && Level < maxlevel)
            LevelSelect = Random.Range(1, maxlevel - 1);
        if (UpgradesData.UpgradeInfoTable[random].ElitePath.Length <= 0 || Level < maxlevel)
            return random;

        // Entering the Elite path domain
        Elite = true;
        ElitePath = Random.Range(0, UpgradesData.UpgradeInfoTable[random].ElitePath.Length - 1);
        int elitelevel = maxlevel + UpgradesData.UpgradeInfoTable[random].ElitePath[ElitePath].MaxLevel;
        while (Level >= elitelevel || Level >= maxlevel) // If elite maxed then reset
        {
            // Repeat the process again
            random = RerollOption(random);
            maxlevel = UpgradesData.UpgradeInfoTable[random].MaxLevel;
            Level = PlayerStat.GetUpgradeLevel(UpgradesData.UpgradeInfoTable[random].ID);
        }
        LevelSelect = Level;
        
        return random;
    }

    private int RerollOption(int number) {
        while (CurrentIDUpgradeTable.Contains(number))
        {
            int random = number;
            int coinflip = 0;

            do
            {
                coinflip = Random.Range(-random, UpgradesData.UpgradeInfoTable.Count - (random + 1));
            } while (coinflip == 0);
            random = random + coinflip;
            number = random;
        }
        return number;
    }

    // Send the upgrade int informations to UI 
    public int[] UpgradeIntInfo() {
        RandomUpgradeIndex = UpgradeOptionRandom();
        int MaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].MaxLevel;
        int[] InfoInt = new int[] {RandomUpgradeIndex, Level, MaxLevel};

        if (Elite)
        {
            int OldMaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].MaxLevel;
            MaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[ElitePath].MaxLevel;
            InfoInt = new int[] { RandomUpgradeIndex, Level - OldMaxLevel, MaxLevel };
        }
        
        return InfoInt;
    }

    // Send the upgrade string informations to UI 
    public string[] UpgradeStringInfo() {
        int index = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeDescription.Length-1;
        if (index >= LevelSelect) {
            index = LevelSelect;
        }
        string[] InfoString = new string[] {UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeName, UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeDescription[index]};

        if (Elite)
        {
            int OldMaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[ElitePath].MaxLevel;
            index = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[ElitePath].UpgradeDescription.Length - 1;
            if (index >= LevelSelect)
            {
                index = LevelSelect;
            }
            InfoString = new string[] { UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[ElitePath].UpgradeName, UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].ElitePath[ElitePath].UpgradeDescription[index] };
        }

        return InfoString;
    }

    // Receive back the selected upgrade from UI
    public void UpgradeOptionSelected(int ID) {
        CurrentIDUpgradeTable.Clear();
        int RealId = UpgradesData.UpgradeInfoTable[ID].ID;
        Level = PlayerStat.UpgradeAbility(RealId);

        string ScriptName = "";
        switch (RealId)
        { //Most upgrades does not need a special case and is calculated through the projectile handler itself
            case 0: //Weapon Upgrade
                MainPlayer.transform.Find("AbilitiesHolder").GetComponent<WeaponController>().CheckUpgrade(LevelSelect-1);
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
