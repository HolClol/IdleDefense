using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerManager : MonoBehaviour
{
    [Header("Set Up")]
    public UnityEvent UpgradePopUp;
    public UnityEvent<int[]> UpdateDamage;
    public UnityEvent<int[]> UpdateUI;
    public UnityEvent SuddenPause;
    public UnityEvent RageActivate;
    
    [Header("Innate Base Stats")]
    public int MaxHealth = 100;
    public int MaxRage = 100;
    
    [Header("Stats")]
    [SerializeField] int EXPToLevelUp = 250;
    [SerializeField] int Level = 1;
    #region SET UP VALUES
    public int ExperienceValue, HealthValue, ShieldValue, RageValue, EnemiesEliminated, PointsEarned, CoinsEarned;
    public int healthValue
    {
        get { return HealthValue; }
        set
        {
            if (HealthValue != value)
            {
                if (value > HealthValue)
                {
                    HealthValue = MaxHealth;
                }
                else
                {
                    HealthValue = value;
                }
                
                UpdateUI.Invoke(new int[] { 0, HealthValue, MaxHealth });
                if (HealthValue <= 0)
                {
                    GameManager.Instance.GameLose(new int[] { 0, EnemiesEliminated, CoinsEarned, PointsEarned });
                }   
            }
        }
    }

    public int experienceValue
    {
        get { return ExperienceValue; }
        set
        {
            if (ExperienceValue != value)
            {
                ExperienceValue = value;
                if (ExperienceValue >= EXPToLevelUp) 
                {
                    LevelUp();
                }
                UpdateUI.Invoke(new int[] { 1, ExperienceValue, EXPToLevelUp });
            }
        }
    }

    public int shieldValue
    {
        get { return ShieldValue; }
        set
        {
            if (ShieldValue != value)
            {
                ShieldValue = value;
            }
        }
    }

    public int rageValue
    {
        get { return RageValue; }
        set { 
            if (RageValue != value)
            {
                RageValue = value;
                if (RageValue >= MaxRage)
                {
                    RageActivate.Invoke();
                    RageValue = 0;
                }
            }
        }
    }

    public int enemiesEliminated
    {
        get { return EnemiesEliminated; }
        set
        {
            if (EnemiesEliminated != value)
            {
                EnemiesEliminated = value;
            }
        }
    }

    public int pointsEarned
    {
        get { return PointsEarned; }
        set
        {
            if (PointsEarned != value)
            {
                PointsEarned = value;
            }
        }
    }

    public int coinsEarned
    {
        get { return CoinsEarned; }
        set
        {
            if (CoinsEarned != value)
            {
                CoinsEarned = value;
            }
        }
    }

    #endregion
    
    private int loop = 0;
    private bool Leveling;
    
    public void INIT()
    {
        HealthValue = MaxHealth;
        ExperienceValue = 0;
        ShieldValue = 0;
    }

    private void LevelUp()
    {
        int templevel = Level;
        while (ExperienceValue >= EXPToLevelUp)
        {
            ExperienceValue -= EXPToLevelUp;
            templevel += 1;
            loop += 1;

            float currentexp = (float)EXPToLevelUp;
            float math = currentexp + 2 * Mathf.Pow(templevel,0.9f);
            EXPToLevelUp = (int)math;
        }
        if (!Leveling)
            StartCoroutine(LoopUpdate());
        
    }

    private IEnumerator LoopUpdate()
    {
        Leveling = true;

        for (int i = 0; i < loop; i++)
        {
            Level += 1;
            
            GameManager.Instance.PlayerUpdateStat(new PlayerDataType(EnumDataType.Health, 50));
            UpdateDamage.Invoke(new int[] { 2 });
            UpgradePopUp.Invoke();
            yield return new WaitForSeconds(1f);
        }
        loop = 0;
        Leveling = false;
        
    }

    private void OnApplicationPause(bool pause)
    {
        SuddenPause.Invoke();
    }

    public void VictoryScreen()
    {
        GameManager.Instance.GameWin(new int[] { 1, EnemiesEliminated, CoinsEarned, PointsEarned });
    }

    public int GetStat(int ID) {
        switch (ID) {
            case 0: //Max Health
                return MaxHealth;
            case 1: //EXP required
                return EXPToLevelUp;
            default:
                return 1;
        }
    }
 
}
