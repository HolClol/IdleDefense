using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObjects/Game/StageInfo")]
public class StageInfo : ScriptableObject
{
    [System.Serializable] public struct RewardsMultiplier {
        public float Coins;
        public float UserRankExp;
        public float CoinsMultiplier;
        public float UserRankExpMultiplier;
    }

    public string StageName;
    public string StageScene;
    public Image StageImage;
    public EnemyPrefabScriptableObject StageEnemies;
    public RewardsMultiplier StageReward;
    public int Energy; //Not planned yet
}
