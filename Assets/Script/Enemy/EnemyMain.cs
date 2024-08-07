using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMain : MonoBehaviour
{
    [Header("Enemy Settings")]
    public string EnemyName;
    public EnemyIDAssign EnemyType;
    public EnemyIDAssign EnemyMovement;
    public int MaxHealth = 100;
    public int Health;
    public int Damage = 1;
    public int Experience = 10;
    public float EnemyMovespeed;
    public float EnemyRotateSpeed;
    public float SkillCooldown;

    [HideInInspector] public bool Dead;
    [HideInInspector] public Color m_SpriteColor;

    protected EnemyMovement m_enemyMovement;
    protected EnemyDeathHandler m_enemyDeath;
    protected SpriteRenderer m_spriteRenderer;
    protected Dictionary<int, float> damageCooldowns = new Dictionary<int, float>();
    protected int DamageDealt;
    protected float CurrentCooldown;
    
    

    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    protected virtual void Start() 
    {
        m_enemyDeath = GetComponent<EnemyDeathHandler>();
        m_enemyMovement = GetComponent<EnemyMovement>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteColor = m_spriteRenderer.color;
        Health = MaxHealth;
        EnemyName = gameObject.name;
    }

    protected virtual IEnumerator HurtPlay() {
        Health -= DamageDealt;
        m_spriteRenderer.color = Color.red;
        Color tempColorLerped = m_SpriteColor;
        m_enemyMovement.SetHurt(true);
        float lerp = 0f;

        while (lerp < 1f) {
            tempColorLerped = Color.LerpUnclamped(Color.red, m_SpriteColor, lerp);
            m_spriteRenderer.color = tempColorLerped;
            lerp += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }

        m_spriteRenderer.color = m_SpriteColor;
        m_enemyMovement.SetHurt(false);
    }

    protected IEnumerator DamageCooldownPlay(int sourceID, float cooldown)
    {
        damageCooldowns[sourceID] = Time.time + cooldown;
        yield return new WaitForSeconds(cooldown);
        damageCooldowns.Remove(sourceID);
    }

    

    // Generally the array consist of follow: int[] {dmg, id}; float[] {knockback, debouncetime}
    protected void ReceiveDamage(int[] intinfo, float[] floatinfo) {
        if (DamageDealt != intinfo[0]) {
            DamageDealt = intinfo[0];
            float DamageKnockback = ((float)DamageDealt/50) + floatinfo[0];
            if (DamageKnockback > 30f) {
                DamageKnockback = 30f;
            }
            m_enemyMovement.SetDamageKnockback(DamageKnockback);
        }

        switch (EnemyType.EnemyID)
        {
            case 1:
                DamageDealt -= (int)((float)DamageDealt * 0.2f);
                break;
        }

        if (Health <= DamageDealt && !Dead) {
            m_enemyDeath.DeathCondition();
        }
        else {
            StartCoroutine(HurtPlay());
            if (floatinfo[1] > 0)
                StartCoroutine(DamageCooldownPlay(intinfo[1], floatinfo[1]));
        }
    }


    // Deals damage to target if the debounce value is off
    public bool GetDebounce(int[] intinfo, float[] floatinfo)
    {
        if (!damageCooldowns.ContainsKey(intinfo[1]) || Time.time >= damageCooldowns[intinfo[1]])
        {
            ReceiveDamage(intinfo, floatinfo);
            return false;
        }
            
        else return true;
       
    }
    
}
