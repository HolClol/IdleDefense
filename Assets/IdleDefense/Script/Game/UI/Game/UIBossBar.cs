using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBossBar : MonoBehaviour
{
    [SerializeField] Image m_HealthBar;
    [SerializeField] TMP_Text m_Text;
    
    private int TransitionLoop;
    private int maxHP;
    private bool Tweening;
    
    public void HealthChange(int currentHP)
    {
        if (currentHP > 0)
        {
            m_HealthBar.fillAmount = (float)currentHP / (float)maxHP;
        }
        else
        {
            Invoke("Disable", 1); // Terrible method, change in the future
        }
            
    }

    public void SetMaxHP(int maxhp)
    {
        maxHP = maxhp;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
    

    private IEnumerator UpdateHealthBar(int health, int maxhealth)
    {
        bool TakeDamage = false;
        float HealthPercent = (float)health / (float)maxhealth;
        float TransitionLoop;
        float CurrentHealth = m_HealthBar.fillAmount;

        if (CurrentHealth > HealthPercent)
        { //The health is decreased
            TransitionLoop = Mathf.Round((CurrentHealth - HealthPercent) * 100.0f);
            TakeDamage = true;
        }
        else
        {
            TransitionLoop = Mathf.Round((HealthPercent - CurrentHealth) * 100.0f);
        }

        for (int i = 0; i < (int)TransitionLoop*10; i++)
        {
            if (TakeDamage)
                m_HealthBar.fillAmount -= 0.001f;
            else
                m_HealthBar.fillAmount += 0.001f;

            yield return new WaitForSeconds(0.05f / TransitionLoop);
        }
    }
}
