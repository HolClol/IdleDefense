using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawnController : MonoBehaviour
{
    [Header("Stage Info")]
    [SerializeField] CurrentStage StageSO;
    [Header("Enemy Spawner Handler")]
    [SerializeField] GameObject _enemySpawnPoint;
    [SerializeField] GameObject _bossSpawnPoint;
    [SerializeField] float DifficultySpikeTimer = 30; //This is serialized in case a stage has a different timer
    [SerializeField] int DifficultyLevel = 1; 
    [SerializeField] bool CanSpawn;
    [SerializeField] float SpawnRate = 1f;
    public UnityEvent BossCalling;

    private StageInfo.EnemyPrefab enemyPrefabs;
    private List<Transform> enemySpawnPos = new List<Transform>();
    private List<EnemyEntity> actualEnemyPrefabs = new List<EnemyEntity>();
    private Dictionary<int, int> enemyCapMechanic = new Dictionary<int, int>();
    private Transform _enemySpawn;
    private GameObject _Boss;
    private float BonusMulti = 0;

    // Stat buff for enemies \\ 

    // Game Difficulty Manager \\
    private float Spawntimer;
    private float Timer;
    private int MultipleSpawn = 1;

    // ======================================================
    // This exist for playtesting
    // ======================================================
    private void Start()
    {
        enemyPrefabs = StageSO.Stage.StageEnemies;
        _enemySpawn = GameObject.Find("_Enemy").transform;

        foreach (Transform child in _enemySpawnPoint.transform)
            enemySpawnPos.Add(child);

        actualEnemyPrefabs = enemyPrefabs.LevelPrefab[0].EnemySpawn;
        Timer = DifficultySpikeTimer;
    }

    // ======================================================
    // Update is called once per frame
    // ======================================================
    private void Update()
    {
        if (CanSpawn && Spawntimer <= 0)
            StartCoroutine(SpawnEnemy());
        else
            Spawntimer -= Time.deltaTime;

        if (Timer <= 0)
            DifficultyIncrease();
        else
            Timer -= Time.deltaTime;
    }

    private void DifficultyIncrease() { 
        bool DecreaseSpawnTimer = true;

        // Every stage will have a different type of monsters and difficulty spike
        if (DifficultyLevel < enemyPrefabs.LevelPrefab.Count)
        {
            actualEnemyPrefabs = enemyPrefabs.LevelPrefab[DifficultyLevel].EnemySpawn;

            // Put spawn rate to -1 if the spawn rate should not be decreased
            if (enemyPrefabs.LevelPrefab[DifficultyLevel].SpawnRate < 0f)
                DecreaseSpawnTimer = false;
            // Put spawn rate to higher than 0 to set the spawn rate 
            else if (enemyPrefabs.LevelPrefab[DifficultyLevel].SpawnRate > 0f)
                SpawnRate = enemyPrefabs.LevelPrefab[DifficultyLevel].SpawnRate;

            // Put timer higher than 0 to set the timer
            if (enemyPrefabs.LevelPrefab[DifficultyLevel].Timer > 0f)
                DifficultySpikeTimer = enemyPrefabs.LevelPrefab[DifficultyLevel].Timer;

        }
        else if (DifficultyLevel == enemyPrefabs.LevelPrefab.Count) //Final stage
        {
            SpawnRate = 0.8f; //Reset to default spawn time
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
                DifficultySpikeTimer = 90;
        }

        Timer = DifficultySpikeTimer;
        DifficultyLevel += 1;

        if (DifficultyLevel % 2 == 0)
        {
            IncreaseMulti(0);
            MultipleSpawn = Mathf.Min(MultipleSpawn + 1, 5); //Cap is 5 enemies per spawn
        }
        else if (DifficultyLevel % 5 == 0)
        {
            SpawnRate -= SpawnRate * 0.4f;
            SpawnRate = Mathf.Round(SpawnRate * 100f) / 100f;
            IncreaseMulti(0);
        }

        enemyCapMechanic.Clear();
    }

    private IEnumerator SpawnEnemy()
    {
        if (!CanSpawn) 
            yield break;

        WaitForSeconds wait = new WaitForSeconds(SpawnRate);
        Spawntimer = SpawnRate;
        yield return wait;
         
        for (int i = 0; i < Random.Range(1,MultipleSpawn); i++)
        {
            int randEnemy = Random.Range(0, actualEnemyPrefabs.Count);
            int randPos = Random.Range(0, enemySpawnPos.Count);

            int spawnchance = actualEnemyPrefabs[randEnemy].SpawnChance;

            GameObject enemyToSpawn = actualEnemyPrefabs[randEnemy].EnemyPrefab;
            Transform enemyToSpawnPos = enemySpawnPos[randPos];

            if (spawnchance > Random.Range(0,100))
            {
                int id = enemyToSpawn.GetComponent<EnemyMain>().EnemyID;
                if (enemyCapMechanic.ContainsKey(id) && enemyCapMechanic[id] >= actualEnemyPrefabs[randEnemy].SpawnCap && actualEnemyPrefabs[randEnemy].SpawnCap > 0)
                    yield break;
                
                if (enemyCapMechanic.ContainsKey(id))
                    enemyCapMechanic[id] += 1;
                else
                    enemyCapMechanic[id] = 0;

                GameObject Enemy = Instantiate(enemyToSpawn, enemyToSpawnPos.position, Quaternion.identity, _enemySpawn);
                Enemy.GetComponent<EnemyMain>().MaxHealth += (int)(Enemy.GetComponent<EnemyMain>().MaxHealth * BonusMulti);
                Enemy.GetComponent<EnemyMain>().Experience += (int)(Enemy.GetComponent<EnemyMain>().Experience * BonusMulti);
                
                // Enemy.GetComponent<EnemyBehaviourScript>().EnemyMovespeed += SpeedIncrease;

                yield return new WaitForSeconds(0.05f);
            }
            
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

    public void DecreaseEnemyCap(int[] info)
    {
        if (enemyCapMechanic.ContainsKey(info[0]))
            enemyCapMechanic[info[0]] -= 1;
            
    }


    public void IncreaseMulti(int ID) {
        switch (ID) {
            case 0:
                BonusMulti += 1;
            break;
        }
    }

}
