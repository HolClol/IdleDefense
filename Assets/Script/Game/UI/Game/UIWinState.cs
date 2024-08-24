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

    private int WinState = 0; //0 is lose, 1 is win
    
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

        TextLib.EnemyCount.text = "Enemies Defeated: " + stat[1].ToString();
        TextLib.CoinCount.text = "Coins Earned: " + stat[2].ToString();
        TextLib.PointCount.text = "Points Earned: " + stat[3].ToString();

        TextLib.StageInfo.text = "Stage Played: " + CurrentStage.Stage.StageName;
        TextLib.CoinMulti.text = "Coin Bonus: " + CurrentStage.Stage.StageReward.CoinsMultiplier.ToString() + "x";
        TextLib.PointMulti.text = "Point Bonus: " + CurrentStage.Stage.StageReward.UserRankExpMultiplier.ToString() + "x";
    }

    //Replay the level or continue 
    public void ReplayOrContinue()
    {
        CallScene.Invoke(new int[] {0});
    }

    public void Return()
    {
        CallScene.Invoke(new int[] {2});
    }
}
