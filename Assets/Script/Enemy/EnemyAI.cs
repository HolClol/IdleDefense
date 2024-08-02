using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
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
    public float Delay;
    public EnemyEffectScriptableObject EnemyVFXPrefab;
    public EnemyMovement _enemyMovement;
    public UnityEvent<int[]> UpdateStat;

    protected Rigidbody2D Rb;
    protected SpriteRenderer m_SpriteRenderer;
    protected Dictionary<int, float> damageCooldowns = new Dictionary<int, float>();
    protected Color m_SpriteColor;
    protected int DamageDealt;
    protected float CurrentCooldown, DelayPlay;
    protected bool Dead;
    protected bool Moltened = false;

    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    protected virtual void Start() 
    {
        Rb = GetComponent<Rigidbody2D>();
        _enemyMovement = GetComponent<EnemyMovement>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteColor = m_SpriteRenderer.color;
        Health = MaxHealth;
        DelayPlay = Delay;
        EnemyName = gameObject.name;
    }

    protected virtual IEnumerator HurtPlay() {
        Health -= DamageDealt;
        m_SpriteRenderer.color = Color.red;
        Color tempColorLerped = m_SpriteColor;
        _enemyMovement.SetHurt(true);
        float lerp = 0f;

        while (lerp < 1f) {
            tempColorLerped = Color.LerpUnclamped(Color.red, m_SpriteColor, lerp);
            m_SpriteRenderer.color = tempColorLerped;
            lerp += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }

        m_SpriteRenderer.color = m_SpriteColor;
        _enemyMovement.SetHurt(false);
    }

    protected IEnumerator DamageCooldownPlay(int sourceID, float cooldown)
    {
        damageCooldowns[sourceID] = Time.time + cooldown;
        yield return new WaitForSeconds(cooldown);
        damageCooldowns.Remove(sourceID);
    }

    protected virtual void DeathCondition()
    {
        if (EnemyType.EnemyID == 2)
        {

            Rb.velocity = new Vector2(0, 0);
            MaxHealth = MaxHealth * 4;
            Health += MaxHealth;
            Moltened = true;
            m_SpriteRenderer.color = Color.grey;
            m_SpriteColor = Color.grey;
            Rb.mass = 200;
            gameObject.name = "Molten";

            UpdateStat.Invoke(new int[] { 1, Experience });
            _enemyMovement.enabled = false;
            Destroy(gameObject, 5);
        }

        else
        {
            if (!Moltened)
            {
                UpdateStat.Invoke(new int[] { 1, Experience });
                GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[0], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
            }

            Destroy(gameObject);
            Dead = true;
        }
        
    }

    // Generally the array consist of follow: int[] {dmg, id}; float[] {knockback, debouncetime}
    protected void ReceiveDamage(int[] intinfo, float[] floatinfo) {
        if (DamageDealt != intinfo[0]) {
            DamageDealt = intinfo[0];
            float DamageKnockback = ((float)DamageDealt/50) + floatinfo[0];
            if (DamageKnockback > 30f) {
                DamageKnockback = 30f;
            }
            _enemyMovement.SetDamageKnockback(DamageKnockback);
        }

        switch (EnemyType.EnemyID)
        {
            case 1:
                DamageDealt -= (int)((float)DamageDealt * 0.2f);
                break;
        }

        if (Health <= DamageDealt && !Dead) {
            DeathCondition();
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

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[1], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
            UpdateStat.Invoke(new int[] { 0, Damage });
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D trigger) {
        if (!trigger.isTrigger) {
            if (trigger.gameObject.CompareTag("Player")) {
                GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[1], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
                UpdateStat.Invoke(new int[] { 0, Damage });
                Destroy(gameObject);
            }
        }
        
    }
    
}
