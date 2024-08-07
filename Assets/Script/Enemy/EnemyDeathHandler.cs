using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDeathHandler : MonoBehaviour
{
    public EnemyEffectScriptableObject EnemyVFXPrefab;
    public UnityEvent<int[]> UpdateStat;

    protected EnemyMain m_enemyState;
    protected Rigidbody2D m_Rb;
    protected SpriteRenderer m_spriteRenderer;
    protected EnemyMovement m_enemyMovement;

    protected bool Moltened = false;

    protected virtual void Start()
    {
        m_enemyState = GetComponent<EnemyMain>();
        m_enemyMovement = GetComponent<EnemyMovement>();
        m_Rb = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void DeathCondition()
    {
        if (m_enemyState.EnemyType.EnemyID == 2)
        {

            m_Rb.velocity = new Vector2(0, 0);
            m_enemyState.MaxHealth = m_enemyState.MaxHealth * 4;
            m_enemyState.Health += m_enemyState.MaxHealth;
            Moltened = true;
            m_spriteRenderer.color = Color.grey;
            m_enemyState.m_SpriteColor = Color.grey;
            m_Rb.mass = 200;
            gameObject.name = "Molten";

            UpdateStat.Invoke(new int[] { 1, m_enemyState.Experience });
            m_enemyMovement.enabled = false;
            Destroy(gameObject, 5);
        }

        else
        {
            if (!Moltened)
            {
                UpdateStat.Invoke(new int[] { 1, m_enemyState.Experience });
                GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[0], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
            }

            Destroy(gameObject);
            m_enemyState.Dead = true;
        }

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[1], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
            UpdateStat.Invoke(new int[] { 0, m_enemyState.Damage });
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D trigger)
    {
        if (!trigger.isTrigger)
        {
            if (trigger.gameObject.CompareTag("Player"))
            {
                GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[1], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
                UpdateStat.Invoke(new int[] { 0, m_enemyState.Damage });
                Destroy(gameObject);
            }
        }
    }

}
