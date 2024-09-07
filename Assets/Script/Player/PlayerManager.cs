using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Set Up")]
    public UnityEvent UpgradePopUp;
    public UnityEvent<int[]> UpdateDamage;
    public UnityEvent<int[]> UpdateUI;
    public UnityEvent<int[]> EndScreenEvent;
    public UnityEvent SuddenPause;

    [Header("Stats")]
    private int MaxHealth = 100;
    [SerializeField] int EXPToLevelUp = 250;
    [SerializeField] int Level = 1;
    public int ExperienceValue, HealthValue, ShieldValue, EnemiesEliminated, PointsEarned, CoinsEarned;

    private int loop = 0;
    private bool Leveling;
    void Start()
    {
        Application.targetFrameRate = 60;
        SceneManager.LoadSceneAsync("GameMenu", LoadSceneMode.Additive);
        HealthValue = MaxHealth;
        ExperienceValue = 0;
        ShieldValue = 0;
    }

    // int[] {ID, updatevalue}
    public void UpdateStat(int[] array) {
        switch (array[0]) {
            case 0: // Health
                HealthValue -= array[1];
                UpdateUI.Invoke(new int[] { 0, HealthValue, MaxHealth });
                if (HealthValue <= 0)
                {
                    EndScreenEvent.Invoke(new int[] { 0, EnemiesEliminated, CoinsEarned, PointsEarned });
                }   
            break;
            case 1: // Experience
                ExperienceValue += array[1];
                if (ExperienceValue >= EXPToLevelUp) 
                {
                    LevelUp();
                }
                UpdateUI.Invoke(new int[] { 1, ExperienceValue, EXPToLevelUp });
            break;
            case 2: // Shield
            break;
            case 3: //Set Max Health + Heal
                MaxHealth += array[1];
                UpdateStat(new int[] { 4, 20 });
            break;
            case 4: //Heal
                if (array[1] + HealthValue >= MaxHealth) {
                    HealthValue = MaxHealth;
                }
                else {
                    HealthValue += array[1];
                }
                UpdateUI.Invoke(new int[] { 0, HealthValue, MaxHealth });
            break;
            case 8: //Add kill count and points
                CoinsEarned += array[1];
                PointsEarned += array[2];
                EnemiesEliminated += 1;
            break;
        }
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
            float math = currentexp + (currentexp * 0.1f) + (currentexp * (templevel / currentexp) * 50);
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

            UpdateStat(new int[] { 4, 50 });
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
        EndScreenEvent.Invoke(new int[] { 1, EnemiesEliminated, CoinsEarned, PointsEarned });
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
