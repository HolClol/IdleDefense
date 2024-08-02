using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    [Header("Set Up")]
    [SerializeField] UIPlayerManager PlayerUI;
    [SerializeField] PlayerController PlayerStat;
    [SerializeField] Camera PlayerCamera;
    public UnityEvent UpgradePopUp;
    // Start is called before the first frame update
    [Header("Stats")]
    [SerializeField] int MaxHealth = 100;
    [SerializeField] int EXPToLevelUp = 100;
    [SerializeField] int Level = 1;
    public int ExperienceValue, HealthValue, ShieldValue;

    private int loop = 0;
    private float CameraDistance = 12;
    private bool Leveling;

    void Start()
    {
        Application.targetFrameRate = 60;
        HealthValue = MaxHealth;
        ExperienceValue = 0;
        ShieldValue = 0;
        PlayerUI.DisplayUpdate(1, new int[] { ExperienceValue, EXPToLevelUp });
    }

    // int[] {ID, updatevalue}
    public void UpdateStat(int[] array) {
        switch (array[0]) {
            case 0: // Health
                HealthValue -= array[1];
                PlayerUI.DisplayUpdate(0, new int[] { HealthValue, MaxHealth });
            break;
            case 1: // Experience
                ExperienceValue += array[1];
                if (ExperienceValue >= EXPToLevelUp) {
                    LevelUp();
                }
                PlayerUI.DisplayUpdate(1, new int[] { ExperienceValue, EXPToLevelUp });
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
                PlayerUI.DisplayUpdate(0, new int[] { HealthValue, MaxHealth });
            break;
        }
    }

    private void LevelUp()
    {
        while (ExperienceValue >= EXPToLevelUp)
        {
            ExperienceValue -= EXPToLevelUp;
            EXPToLevelUp = (int)((float)EXPToLevelUp + (float)(EXPToLevelUp * 0.25f));
            loop += 1;
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
            PlayerStat.UpdateDamage(1);
            UpgradePopUp.Invoke();
            yield return new WaitForSeconds(1f);
        }
        loop = 0;
        Leveling = false;
        
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

    public void CameraRun(float[] value)
    {
        StartCoroutine(ChangeCameraDistance(value[0], value[1]));
    }
    public IEnumerator ChangeCameraDistance(float distance, float delay) 
    {
        int loop;
        bool increase = false;
        yield return new WaitForSeconds(delay);

        if (distance > CameraDistance)
        {
            loop = (int)(distance - CameraDistance);
            increase = true;
        }
            
        else
            loop = (int)(CameraDistance - distance);
        for (int i = 0; i < loop*10; i++)
        {
            if (increase)
                PlayerCamera.orthographicSize += 0.1f;
            else
                PlayerCamera.orthographicSize -= 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        CameraDistance = distance;
    }
}
