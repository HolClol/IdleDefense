using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public PlayerManager PlayerManager;
    public EnemySpawnController EnemyManager;
    public DamageCalculateManager DamageManager;
    
    public UnityEvent<int[]> EndScreenEvent;

    private void Awake()
    {
        Instance = this;
    }
    
    public void Start()
    {
        SetUp();
        
        EnemyManager.INIT();
        PlayerManager.INIT();

    }

    private void SetUp()
    {
        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(500, 50);
        DOTween.defaultAutoPlay = AutoPlay.None;
        Application.targetFrameRate = 60;
        SceneManager.LoadSceneAsync("GameMenu", LoadSceneMode.Additive);
    }
    
    public void PlayerUpdateStat(EnumDataType enumType, float value)
    {
        switch (enumType)
        {
            case EnumDataType.Health:
                PlayerManager.healthValue += (int)value;
                break;
            case EnumDataType.MaxHealth:
                PlayerManager.MaxHealth += (int)value;
                PlayerUpdateStat(EnumDataType.Health, (int)value);
                break;
            case EnumDataType.Experience:
                PlayerManager.experienceValue += (int)value;
                break;
            case EnumDataType.Shield:
                break;
            case EnumDataType.Rage:
                PlayerManager.rageValue += (int)value;
                break;
            case EnumDataType.Coins:
                PlayerManager.coinsEarned += (int)value;
                break;
            case EnumDataType.Points:
                PlayerManager.pointsEarned += (int)value;
                break;
            case EnumDataType.EnemyEliminated:
                PlayerManager.enemiesEliminated += (int)value;
                break;
            
        }
    }

    public void SendDamage(GameObject enemy, int[] intstat, float[] floatstat)
    {
        DamageManager.DamageCalculate(enemy, intstat, floatstat);
    }

    public void PlayerReceiveUpgrade(int[] stat)
    {
        DamageManager.ReceiveUpgrade(stat);
    }

    public void GameWin(int[] stat)
    {
        EndScreenEvent.Invoke(stat);
    }

    public void GameLose(int[] stat)
    {
        EndScreenEvent.Invoke(stat);
    }
}
