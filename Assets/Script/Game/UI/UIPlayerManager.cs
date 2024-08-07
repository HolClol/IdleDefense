using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerManager : MonoBehaviour
{
    [SerializeField] Image m_HealthBar;
    [SerializeField] Image m_EXPBar;
    //[SerializeField] Image m_ShieldBar;
    //[SerializeField] TMP_Text m_HealthText;
    
    public void DisplayUpdate(int ID, int[] stat)
    {
        switch(ID) {
            case 0: //Update Health
                StartCoroutine(UpdateHealthBar(stat[0], stat[1]));
            break;
            case 1: //Update EXP
                UpdateEXPBar(stat[0], stat[1]);
            break;
        }
    }

    private IEnumerator UpdateHealthBar(int health, int maxHealth) {
        bool TakeDamage = false;
        float HealthPercent = (float)health/(float)maxHealth;
        float TransitionLoop;
        float CurrentHealth = m_HealthBar.fillAmount;
        
        if (CurrentHealth > HealthPercent) { //The health is decreased
            TransitionLoop = Mathf.Round((CurrentHealth - HealthPercent)*100.0f);
            TakeDamage = true;
        }
        else {
            TransitionLoop = Mathf.Round((HealthPercent - CurrentHealth)*100.0f);
        }

        for (int i = 0; i < (int)TransitionLoop; i++) {
            if (TakeDamage) 
                m_HealthBar.fillAmount -= 0.001f;
            else
                m_HealthBar.fillAmount += 0.001f;

            yield return new WaitForSeconds(0.005f/TransitionLoop);
        }
        
    }

    private void UpdateEXPBar(int exp, int EXPReq) {
        float EXPPercent = (float)exp/(float)EXPReq;
        m_EXPBar.fillAmount = EXPPercent;
    }
}
