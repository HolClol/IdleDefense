using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int Coins;
    public int UserRank;
    public int UserPoints;

    public GameData()
    {
        this.Coins = 0;
        this.UserRank = 1;
        this.UserPoints = 0;
    }
}
