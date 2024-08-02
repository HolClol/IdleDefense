using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPrefab", menuName = "ScriptableObjects/Enemy/EnemyPrefabScriptableObject", order = 1)]
public class EnemyPrefabScriptableObject : ScriptableObject 
{
    [System.Serializable] public class LevelPrefabs
    {
        public List<GameObject> EnemyPrefab;
        public float SpawnRate; //Set to 0 if want to not decrease, 1 to increase normally
        public float Timer; //Set to 0 if want to not decrease, 1 to increase normally
    }
    public List<LevelPrefabs> LevelPrefab;
    public GameObject[] BossPrefab;
}



