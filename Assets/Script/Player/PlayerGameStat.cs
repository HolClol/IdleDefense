using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGameStat : MonoBehaviour
{
    public UnityEvent<int[]> SendLevel;

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
                PlayerController.PlayerUpgradeStat newUpgrade = new PlayerController.PlayerUpgradeStat(id, level);
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
    public void GetUpgradeLevel(int[] id)
    {
        switch (id[0]) //Type of call
        {
            case 0: //Asking for upgrade level call
                int result = 0;
                for (int i = 0; i < MainController.PlayerStats.Upgrades.Count; i++)
                {
                    if (id[1] == MainController.PlayerStats.Upgrades[i].UpgradeID)
                    {
                        result = MainController.PlayerStats.Upgrades[i].UpgradeLevel; //Looped
                        break;
                    }
                }
                SendLevel.Invoke(new int[] { result });
                break;

            case 1: //Sending the upgraded option
                bool existed = false;
                for (int i = 0; i < MainController.PlayerStats.Upgrades.Count; i++)
                {
                    if (id[1] == MainController.PlayerStats.Upgrades[i].UpgradeID)
                    {
                        existed = true;
                        AdjustUpgrade(1, i, 0);
                        SendLevel.Invoke(new int[] { MainController.PlayerStats.Upgrades[i].UpgradeLevel });
                    }
                }
                if (!existed)
                {
                    AdjustUpgrade(0, id[1], 1);
                }
                break;
        }

    }
}
