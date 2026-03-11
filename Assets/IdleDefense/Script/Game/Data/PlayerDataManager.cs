using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour, IDataPersistence
{
    public int Coins;
    public int UserRank;
    public int UserPoints;
    public bool MainObject;

    private void Start()
    {
        if (!MainObject)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(GameData data)
    {
        Coins = data.Coins;
        UserRank = data.UserRank;
        UserPoints = data.UserPoints;
    }

    public void SaveData(ref GameData data)
    {
        data.Coins = Coins;
        data.UserRank = UserRank;
        data.UserPoints = UserPoints;  
    }

    public void AddCurrency(int[] intstat)
    {
        Coins = intstat[0];
        UserPoints = intstat[1];
    }
}
