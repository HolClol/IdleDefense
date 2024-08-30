using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    
    public UpgradesScriptableObject UpgradesData;
    public UnityEvent<int[]> UpdateStat;
    public UnityEvent<int[]> UpdateDamage;
    public UnityEvent<int[]> SendPlayerUpgrade;

    private GameObject MainPlayer;
    private List<int> CurrentIDUpgradeTable = new List<int>();
    private List<int> ObtainedUpgrade = new List<int>();
    private int RandomUpgradeIndex;
    private int Level;
    private int LevelSelect;
    private bool GuaranteeUpgrade = false;
    private bool Elite = false;

    private void Start()
    {
        MainPlayer = GameObject.Find("Player").gameObject;
    }

    //Select a random upgrade in the table (This should run in the first info collect signal)
    private int UpgradeOptionRandom() {
        int random = Random.Range(0, UpgradesData.UpgradeInfoTable.Count);
        int maxlevel = UpgradesData.UpgradeInfoTable[random].MaxLevel;
        int index = CurrentIDUpgradeTable.Count;

        Level = 0;
        LevelSelect = Level;
        SendPlayerUpgrade.Invoke(new int[] {0, UpgradesData.UpgradeInfoTable[random].ID });

        while (CurrentIDUpgradeTable.Contains(random))
        {
            random = RerollOption(random);   
        }

        if (ObtainedUpgrade.Contains(random) && GuaranteeUpgrade)
            GuaranteeUpgrade = false;

        maxlevel = UpgradesData.UpgradeInfoTable[random].MaxLevel;
        SendPlayerUpgrade.Invoke(new int[] { 0, UpgradesData.UpgradeInfoTable[random].ID });
        CurrentIDUpgradeTable.Add(random);

        // Replace a random upgrade if there is a guarantee old upgrade
        if (GuaranteeUpgrade && CurrentIDUpgradeTable.Count > 2)
        {
            int GuaranteeOptionUpgrade = Random.Range(0, CurrentIDUpgradeTable.Count - 1);
            int ExistedUpgradeID = ObtainedUpgrade[Random.Range(0, ObtainedUpgrade.Count - 1)];

            while (CurrentIDUpgradeTable.Contains(ExistedUpgradeID))
            {
                ExistedUpgradeID = ObtainedUpgrade[Random.Range(0, ObtainedUpgrade.Count - 1)];
                CurrentIDUpgradeTable[GuaranteeOptionUpgrade] = ExistedUpgradeID;
            }
            
            GuaranteeUpgrade = false;

            random = CurrentIDUpgradeTable[GuaranteeOptionUpgrade];
            maxlevel = UpgradesData.UpgradeInfoTable[random].MaxLevel;
            SendPlayerUpgrade.Invoke(new int[] { 0, UpgradesData.UpgradeInfoTable[random].ID });
        }

        if (random == 0 && Level < maxlevel)
            LevelSelect = Random.Range(1, maxlevel - 1);

        if (UpgradesData.UpgradeInfoTable[random].ElitePath.Length <= 0)
            return random;

        // Entering the Elite path domain
        Elite = true;
        if (Level == maxlevel)
        {
            int elitepath = Random.Range(0, UpgradesData.UpgradeInfoTable[random].ElitePath.Length - 1);
            
        }
        else if (Level > maxlevel)
        {

        }

        return random;
    }

    private int RerollOption(int number) {
        int random = number;
        int coinflip = 0; 

        do { 
            coinflip = Random.Range(-random, UpgradesData.UpgradeInfoTable.Count - (random + 1)); 
        } while (coinflip == 0);
        random = random + coinflip;
        return random;
    }

    // Check whether the player has the upgrade or not
    public void CheckPlayerUpgrade(int[] ID)
    {
        Level = ID[0];
    }

    // Send the upgrade int informations to UI 
    public int[] UpgradeIntInfo() {
        RandomUpgradeIndex = UpgradeOptionRandom();
        int MaxLevel = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].MaxLevel;
        int[] InfoInt = new int[] {RandomUpgradeIndex, Level, MaxLevel};
        return InfoInt;
    }

    // Send the upgrade string informations to UI 
    public string[] UpgradeStringInfo() {
        int index = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeDescription.Length-1;
        if (index >= LevelSelect) {
            index = LevelSelect;
        }
        string[] InfoString = new string[] {UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeName, UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].UpgradeDescription[index]};
        return InfoString;
    }

    // Receive back the selected upgrade from UI
    public void UpgradeOptionSelected(int ID) {
        CurrentIDUpgradeTable.Clear();
        Level = 0;
        int RealId = UpgradesData.UpgradeInfoTable[ID].ID;
        SendPlayerUpgrade.Invoke(new int[] {1, RealId });

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
                UpdateDamage.Invoke(new int[] { 5 });
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
