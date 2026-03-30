using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UIBossBar : MonoBehaviour
{
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private TMP_Text m_Text;
    
    private RectTransform _rectTransform;
    private int TransitionLoop;
    private int maxHP;
    private bool Tweening;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    
    public void HealthChange(int currentHP)
    {
        m_HealthBar.fillAmount = (float)currentHP / (float)maxHP;
    }

    public void SetMaxHP(int maxhp)
    {
        _rectTransform.DOAnchorPosY(-700f, 1f);
        maxHP = maxhp;
    }

    public void PlayDeath() 
    {
        _rectTransform.DOAnchorPosY(-1300f, 1f);
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
