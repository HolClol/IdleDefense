using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] GameObject MainPlayer;
    public UpgradesScriptableObject UpgradesData;
    public UnityEvent<int[]> UpdateStat;

    private PlayerController playerController;
    private int RandomUpgradeIndex;
    private int Level;
    private List<int> CurrentIDUpgradeTable = new List<int>();

    private void Start() {
        playerController = MainPlayer.GetComponent<PlayerController>();
    }

    // Check whether the player has the upgrade or not
    private int CheckPlayerUpgrade(int ID) {
        int result = 0;
        for (int i = 0; i < playerController.PlayerStats.Upgrades.Count; i++) {
            if (ID == playerController.PlayerStats.Upgrades[i].UpgradeID) {
                result = playerController.PlayerStats.Upgrades[i].UpgradeLevel;
                return result; //Looped
            }
        }
        return result; //No upgrade
    }

    //Select a random upgrade in the table (This should run in the first info collect signal)
    private int UpgradeOptionRandom() {
        int random = Random.Range(0, UpgradesData.UpgradeInfoTable.Count);
        int maxlevel = UpgradesData.UpgradeInfoTable[random].Upgrade.MaxLevel;
        int index = CurrentIDUpgradeTable.Count;
        Level = CheckPlayerUpgrade(UpgradesData.UpgradeInfoTable[random].Upgrade.ID);

        for (int i = 0; i < CurrentIDUpgradeTable.Count; i++) {
            // Check if the upgrade is duped or maxed
            while (random == CurrentIDUpgradeTable[i] || Level >= maxlevel) { 
                random = RerollOption(random);
                
                maxlevel = UpgradesData.UpgradeInfoTable[random].Upgrade.MaxLevel;
                Level = CheckPlayerUpgrade(UpgradesData.UpgradeInfoTable[random].Upgrade.ID);
            }
        }

        if (Level >= maxlevel)
        {
            random = RerollOption(random);

            maxlevel = UpgradesData.UpgradeInfoTable[random].Upgrade.MaxLevel;
            Level = CheckPlayerUpgrade(UpgradesData.UpgradeInfoTable[random].Upgrade.ID);
        }

        CurrentIDUpgradeTable.Add(random);
        return random;
    }

    private int RerollOption(int number) {
        int backupthird = 0;
        int random = number;
        int coinflip = 0; 
        do { 
            coinflip = Random.Range(-random, UpgradesData.UpgradeInfoTable.Count - (random + 1)); 
        } while (coinflip == 0);
        random = random + coinflip;
        
        if (CurrentIDUpgradeTable.Count >= 2) {
            backupthird = CurrentIDUpgradeTable[0];
            if (random == backupthird) {
                coinflip = 0; 
                do { 
                    coinflip = Random.Range(-random, UpgradesData.UpgradeInfoTable.Count - (random + 1)); 
                } while (coinflip == 0);
                random = random + coinflip;
            }
        }
        return random;
    }

    // Send the upgrade int informations to UI 
    public int[] UpgradeIntInfo() {
        RandomUpgradeIndex = UpgradeOptionRandom();
        int[] InfoInt = new int[] {RandomUpgradeIndex, Level};
        return InfoInt;
    }

    // Send the upgrade string informations to UI 
    public string[] UpgradeStringInfo() {
        int index = UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].Upgrade.UpgradeDescription.Length-1;
        if (index >= Level) {
            index = Level;
        }
        string[] InfoString = new string[] {UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].Upgrade.UpgradeName, UpgradesData.UpgradeInfoTable[RandomUpgradeIndex].Upgrade.UpgradeDescription[index]};
        return InfoString;
    }

    // Receive back the selected upgrade from UI
    public void UpgradeOptionSelected(int ID) {
        CurrentIDUpgradeTable.Clear();
        bool existed = false;
        int upgradelevel = 0; 
        int RealId = UpgradesData.UpgradeInfoTable[ID].Upgrade.ID;
        for (int i = 0; i < playerController.PlayerStats.Upgrades.Count; i++) {
            if (RealId == playerController.PlayerStats.Upgrades[i].UpgradeID) {
                existed = true;
                playerController.AdjustUpgrade(1, i, 0);
                upgradelevel = playerController.PlayerStats.Upgrades[i].UpgradeLevel;
            }
        }
        if (!existed) {
            playerController.AdjustUpgrade(0, RealId, 1);
        }

        string ScriptName = "";
        switch (RealId) { //Most upgrades does not need a special case and is calculated through the projectile handler itself
            case 0: //Weapon Upgrade
                MainPlayer.transform.Find("PlayerChar").GetComponent<WeaponController>().CheckUpgrade(upgradelevel);
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
                UpdateStat.Invoke(new int[] {3, 20});
            break;
            case 21:
                playerController.UpdateDamage(5);
            break;
        }

        AbilitiesController ScriptComponent;
        if (ScriptName != "") {
            ScriptComponent = MainPlayer.transform.Find("PlayerChar").GetComponent(ScriptName) as AbilitiesController;
            switch (upgradelevel) {
                case 0:
                    ScriptComponent.enabled = true;
                break;
                default:
                    ScriptComponent.CheckUpgrade(upgradelevel-1); //Real upgrade value is smaller by 1
                break;
            }
        }
        

    }
}
