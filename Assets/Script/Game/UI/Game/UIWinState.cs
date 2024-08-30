using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class UIWinState : MonoBehaviour
{
    
    [System.Serializable] public class TextInfo
    {
        public TMP_Text GameState;
        public TMP_Text EnemyCount;
        public TMP_Text CoinCount;
        public TMP_Text PointCount;
        public TMP_Text StageInfo;
        public TMP_Text CoinMulti;
        public TMP_Text PointMulti;
    }
    public TextInfo TextLib;
    public CurrentStage CurrentStage;
    public FloatVariable GameSpeed;
    public UnityEvent<int[]> CallScene;
    public UnityEvent<int[]> SendCurrency;

    private int WinState = 0; //0 is lose, 1 is win
    private int Coins, Points;
    
    // Display stats and your game state
    // Follow this format {0 = winstate, 1 = enemy count, 2 = coin count, 3 = point count}
    public void WinStateDisplay(int[] stat)
    {
        Time.timeScale = 0;
        GameSpeed.Value = 0;
        WinState = stat[0];
        if (WinState == 0)
            TextLib.GameState.text = "DEFEAT";
        else if (WinState == 1)
            TextLib.GameState.text = "VICTORY";

        Coins = stat[2];
        Points = stat[3];
        float CoinMulti = CurrentStage.Stage.StageReward.CoinsMultiplier;
        float PointMulti = CurrentStage.Stage.StageReward.UserRankExpMultiplier;

        TextLib.EnemyCount.text = "Enemies Defeated: " + stat[1].ToString();
        StartCoroutine(ShowRewards(false, 1f, 1f, 1f));

        if (WinState == 1)
        { 
            StartCoroutine(ShowRewards(true, 2f, CoinMulti, PointMulti));
        }
            


    }

    private IEnumerator ShowRewards(bool victory, float timer, float coinmulti, float pointmulti)
    {
        yield return new WaitForSecondsRealtime(timer);
        if (victory)
        {
            TextLib.StageInfo.gameObject.SetActive(true);
            TextLib.CoinMulti.gameObject.SetActive(true);
            TextLib.PointMulti.gameObject.SetActive(true);

            TextLib.StageInfo.text = "Stage Played: " + CurrentStage.Stage.StageName;
            TextLib.CoinMulti.text = "Coin Bonus: " + coinmulti.ToString() + "x";
            TextLib.PointMulti.text = "Point Bonus: " + pointmulti.ToString() + "x";
        }

        Coins += (int)((float)Coins * (coinmulti - 1f));
        Points += (int)((float)Points * (pointmulti - 1f));

        TextLib.CoinCount.text = "Coins Earned: " + Coins.ToString();
        TextLib.PointCount.text = "Points Earned: " + Points.ToString();
        
    }

    //Replay the level or continue 
    public void ReplayOrContinue()
    {
        SendCurrency.Invoke(new int[] { Coins, Points });
        CallScene.Invoke(new int[] {0});
    }

    public void Return()
    {
        SendCurrency.Invoke(new int[] { Coins, Points });
        CallScene.Invoke(new int[] {2});
    }
}
