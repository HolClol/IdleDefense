using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EnemySpawnController : MonoBehaviour
{
    [Header("Enemy Spawner Handler")]
    [SerializeField] float SpawnRate = 1f;
    [SerializeField] EnemyPrefabScriptableObject enemyPrefabs;
    [SerializeField] GameObject _enemySpawnPoint;
    [SerializeField] GameObject _bossSpawnPoint;
    [SerializeField] bool CanSpawn;
    [SerializeField] float DifficultySpikeTimer = 30; //This is serialized in case a stage has a different timer
    [SerializeField] int DifficultyLevel = 1;
    public UnityEvent BossCalling;

    private List<Transform> enemySpawnPos = new List<Transform>();
    private List<GameObject> actualEnemyPrefabs = new List<GameObject>();
    private Transform _enemySpawn;
    private GameObject _Boss;
    private int HealthMulti = 0;

    // Stat buff for enemies \\ 
    private float SpeedIncrease = 0;

    // Game Difficulty Manager \\
    private float Spawntimer;
    private float Timer;

    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    private void Start()
    {
        _enemySpawn = GameObject.Find("_Enemy").transform;
        foreach (Transform child in _enemySpawnPoint.transform) {
            enemySpawnPos.Add(child);
        }
        actualEnemyPrefabs = enemyPrefabs.LevelPrefab[0].EnemyPrefab;
        Timer = DifficultySpikeTimer;
    }

    // ======================================================
    // Update is called once per frame
    // ======================================================
    private void Update()
    {
        if (CanSpawn && Spawntimer <= 0) {
            StartCoroutine(SpawnEnemy());
        }
        else 
            Spawntimer -= Time.deltaTime;

        if (Timer <= 0) {
            DifficultyIncrease();
        }
        else 
            Timer -= Time.deltaTime;
    }

    private void DifficultyIncrease() { //NOTE: CHANGE THESE DIFFICULTY NUMBERS ONCE MORE SCENES ARE LOADED INTO THE GAME
        Scene currentScreen = SceneManager.GetActiveScene();
        int buildIndex = currentScreen.buildIndex;
        bool DecreaseSpawnTimer = true;

        // Every stage will have a different type of monsters and difficulty spike
        if (DifficultyLevel < enemyPrefabs.LevelPrefab.Count)
        {
            actualEnemyPrefabs = enemyPrefabs.LevelPrefab[DifficultyLevel].EnemyPrefab;

            if (enemyPrefabs.LevelPrefab[DifficultyLevel].SpawnRate < 0f)
                DecreaseSpawnTimer = false;
            else if (enemyPrefabs.LevelPrefab[DifficultyLevel].SpawnRate > 0f)
                SpawnRate = enemyPrefabs.LevelPrefab[DifficultyLevel].SpawnRate;

            if (enemyPrefabs.LevelPrefab[DifficultyLevel].Timer > 0f)
                DifficultySpikeTimer = enemyPrefabs.LevelPrefab[DifficultyLevel].Timer;

        }
        else if (DifficultyLevel == enemyPrefabs.LevelPrefab.Count) //Final stage
        {
            SpawnRate = 0.5f; //Reset to default spawn time
            BossCalling.Invoke();
            StartCoroutine(SpawnBoss(0));
        }

        if (DecreaseSpawnTimer)
            SpawnRate -= SpawnRate * 0.05f;
        SpawnRate = Mathf.Round(SpawnRate * 100f) / 100f;

        if (DifficultySpikeTimer < 90)
        { //Cap difficulty timer
            DifficultySpikeTimer += 15;
            if (DifficultySpikeTimer > 90)
            {
                DifficultySpikeTimer = 90;
            }
        }

        Timer = DifficultySpikeTimer;
        DifficultyLevel += 1;
        if (DifficultyLevel % 2 == 0)
        {
            IncreaseMulti(0);
        }
        else if (DifficultyLevel % 5 == 0)
        {
            SpawnRate -= SpawnRate * 0.35f;
            SpawnRate = Mathf.Round(SpawnRate * 100f) / 100f;
            IncreaseMulti(0);
        }

    }

    private IEnumerator SpawnEnemy()
    {
        WaitForSeconds wait = new WaitForSeconds(SpawnRate);
        Spawntimer = SpawnRate;
        yield return wait;

        int randEnemy = Random.Range(0, actualEnemyPrefabs.Count);
        int randPos = Random.Range(0, enemySpawnPos.Count);
        if (CanSpawn) {
            GameObject enemyToSpawn = actualEnemyPrefabs[randEnemy];
            Transform enemyToSpawnPos = enemySpawnPos[randPos];
            

            GameObject Enemy = Instantiate(enemyToSpawn, enemyToSpawnPos.position, Quaternion.identity, _enemySpawn);
            Enemy.GetComponent<EnemyAI>().MaxHealth += (Enemy.GetComponent<EnemyAI>().MaxHealth * HealthMulti)/2;
            Enemy.GetComponent<EnemyAI>().Experience += (Enemy.GetComponent<EnemyAI>().Experience * HealthMulti);
            // Enemy.GetComponent<EnemyBehaviourScript>().EnemyMovespeed += SpeedIncrease;
        }
    }

    private IEnumerator SpawnBoss(int index)
    {
        GameObject bossToSpawn = enemyPrefabs.BossPrefab[index];
        Transform bossToSpawnPos = _bossSpawnPoint.transform;

        yield return new WaitForSeconds(6f);
        GameObject Boss = Instantiate(bossToSpawn, bossToSpawnPos.position, Quaternion.identity, _enemySpawn);
        _Boss = Boss;
    }

    public void IncreaseMulti(int ID) {
        switch (ID) {
            case 0:
                HealthMulti += 1;
            break;
            case 1:
                SpeedIncrease += 0.5f;
            break;
        }
    }
}
