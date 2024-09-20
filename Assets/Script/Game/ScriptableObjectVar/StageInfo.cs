using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class EnemyEntity
{
    public GameObject EnemyPrefab;
    [Range(1, 100)] public int SpawnChance = 50;
    public int SpawnCap;
    public float BonusHealthMulti = 0;
    public float BonusExpMulti = 0;
}

[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObjects/Game/StageInfo")]
public class StageInfo : ScriptableObject
{
    [System.Serializable] public struct RewardsMultiplier
    {
        public float Coins;
        public float UserRankExp;
        public float CoinsMultiplier;
        public float UserRankExpMultiplier;
    }

    [System.Serializable] public class LevelPrefabs
    {
        public List<EnemyEntity> EnemySpawn;
        public float SpawnRate; //Set to 0 if want to not decrease, 1 to increase normally
        public float Timer; //Set to 0 if want to not decrease, 1 to increase normally
        public int SpawnMax;
        public int SpawnMin;
    }
    [System.Serializable] public class EnemyPrefab
    {
        public List<LevelPrefabs> LevelPrefab;
        public List<GameObject> BossPrefab;
        public float StartingHPBonus;
        public float StartingEXPBonus;
    }

    public string StageName;
    public string StageScene;
    public Image StageImage;
    public EnemyPrefab StageEnemies;
    public RewardsMultiplier StageReward;
    public int Energy; //Not planned yet
}
