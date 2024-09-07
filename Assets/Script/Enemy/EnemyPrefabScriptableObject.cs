using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyEntity
{
    public GameObject EnemyPrefab;
    [Range(1, 20)] public int SpawnChance = 10;
    public int SpawnCap;
}

[CreateAssetMenu(fileName = "EnemyPrefab", menuName = "ScriptableObjects/Enemy/EnemyPrefabScriptableObject", order = 1)]
public class EnemyPrefabScriptableObject : ScriptableObject 
{
    [System.Serializable] public class LevelPrefabs
    {
        public List<EnemyEntity> EnemySpawn;
        public float SpawnRate; //Set to 0 if want to not decrease, 1 to increase normally
        public float Timer; //Set to 0 if want to not decrease, 1 to increase normally
        public int SpawnMax;
        public int SpawnMin;
    }

    public List<LevelPrefabs> LevelPrefab;
    public List<GameObject> BossPrefab;
    public int StartingHPBonus;
    public int StartingEXPBonus;
}



