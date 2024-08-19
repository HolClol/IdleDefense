using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIBossBar : MonoBehaviour
{
    [SerializeField] EnemyPrefabScriptableObject enemyPrefab;
    [SerializeField] Image m_HealthBar;
    [SerializeField] TMP_Text m_Text;

    private Animator m_Animator;
    private int TransitionLoop;
    private bool Tweening;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("BossIntroBar");
    }

    public void UpdateName(int[] value)
    {
        m_Text.text = enemyPrefab.BossPrefab[value[0]].gameObject.GetComponent<EnemyMain>().EnemyName;
    }
    public void HealthChange(int[] value)
    {
        if (value[0] > 0)
        {
            HealthChange(value[0], value[1]);
        }
        else
        {
            m_Animator.Play("BossOutroBar");
            Invoke("Disable", 1);
        }
            
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void HealthChange(int health, int maxhealth)
    {
        m_HealthBar.fillAmount = (float)health / (float)maxhealth; ;
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
