using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGameStat : MonoBehaviour
{
    private PlayerController MainController;
    private void Start()
    {
        MainController = GetComponent<PlayerController>();
    }
    public void AdjustUpgrade(int type, int id, int level)
    {
        switch (type)
        {
            case 0:
                PlayerController.PlayerUpgradeStat newUpgrade = new PlayerController.PlayerUpgradeStat(id, level, 0);
                MainController.PlayerStats.Upgrades.Add(newUpgrade);
                break;
            case 1:
                MainController.PlayerStats.Upgrades[id].UpgradeLevel += 1;
                break;
        }

    }

    public void UpdateDamage(int[] damage)
    {
        MainController.PlayerStats.BaseDamage += damage[0];
        MainController.PlayerChar.SendMessage("UpdateDamage", MainController.PlayerStats.BaseDamage);
    }

    // Check players upgrade
    public int GetUpgradeLevel(int id)
    {
        int result = 0;
        for (int i = 0; i < MainController.PlayerStats.Upgrades.Count; i++)
        {
            if (id == MainController.PlayerStats.Upgrades[i].UpgradeID)
            {
                result = MainController.PlayerStats.Upgrades[i].UpgradeLevel; //Looped
                return result;
            }
        }
        return result;
    }

    public int UpgradeAbility(int id)
    {
        bool existed = false;
        int level = 0;
        for (int i = 0; i < MainController.PlayerStats.Upgrades.Count; i++)
        {
            if (id == MainController.PlayerStats.Upgrades[i].UpgradeID)
            {
                existed = true;
                AdjustUpgrade(1, i, 0);
                level = MainController.PlayerStats.Upgrades[i].UpgradeLevel;
                break;
            }
        }
        if (!existed)
        {
            AdjustUpgrade(0, id, 1);
        }
        return level;
    }

    public void EliteUnlock(int id)
    {
        for (int i = 0; i < MainController.PlayerStats.Upgrades.Count; i++)
        {
            if (id == MainController.PlayerStats.Upgrades[i].UpgradeID)
            {
                
            }
        }
    }

}
