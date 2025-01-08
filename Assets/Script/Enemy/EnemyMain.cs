using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMain : MonoBehaviour
{
    [Header("Enemy Settings")]
    [HideInInspector] public string EnemyName;
    public EnemyIDAssign EnemyType;
    public EnemyIDAssign EnemyMovement;
    public Sprite EnemySprite;
    public int MaxHealth = 100;
    public int Health;
    public int Damage = 1;
    public int Experience = 10;
    public int EnemyID;
    public float EnemyMovespeed;
    public float EnemyRotateSpeed;
    public float SkillCooldown;

    [HideInInspector] public int DamageDealt;
    [HideInInspector] public bool Dead;
    [HideInInspector] public Color m_SpriteColor;

    protected EnemyMovement m_enemyMovement;
    protected EnemyDeathHandler m_enemyDeath;
    protected SpriteRenderer m_spriteRenderer;
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

    protected virtual void TakeDamage(int id)
    {
        Health -= DamageDealt;
    }

    // Generally the array consist of follow: int[] {dmg, id}; float[] {knockback, debouncetime}
    public virtual int ReceiveDamage(int[] intinfo, float[] floatinfo) {
        DamageDealt = intinfo[0];
        float DamageKnockback = ((float)DamageDealt/50) + floatinfo[0];
        if (DamageKnockback > 30f) {
            DamageKnockback = 30f;
        }
        m_enemyMovement.SetDamageKnockback(DamageKnockback);

        if (Health <= DamageDealt && !Dead) {
            m_enemyDeath.DeathCondition();
        }
        else {
            TakeDamage(intinfo[1]);
            StartCoroutine(HurtPlay()); 
        }
        return DamageDealt;
    }
    
}
