using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TheSpikerAI : EnemyAI
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] GameObject _projectile;

    [Header("Events")]
    public UnityEvent<float[]> CameraChange;
    public UnityEvent<int[]>[] UpdateBossUI;

    private float TimeBeforeFire;
    private int FireRepeat = 1;
    private float angle, offsetangle;
    

    protected override void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Health = MaxHealth;
        TimeBeforeFire = SkillCooldown;
        m_SpriteColor = _spriteRenderer.color;
        m_SpriteRenderer = _spriteRenderer;
        CameraChange.Invoke(new float[] { 18, 5 });
        UpdateBossUI[0].Invoke(new int[] { 0 });
        UpdateBossUI[1].Invoke(new int[] { Health, MaxHealth });
    }
    private void Update()
    {

        if (TimeBeforeFire <= 0f) {
            angle = 0f;
            offsetangle = 0f;
            StartCoroutine(FireProjectile());
            TimeBeforeFire = SkillCooldown;
        }
        else
            TimeBeforeFire -= Time.deltaTime;


    }

    protected override IEnumerator HurtPlay()
    {
        Health -= DamageDealt;
        m_SpriteRenderer.color = Color.red;
        Color tempColorLerped = m_SpriteColor;
        _enemyMovement.SetHurt(true);
        float lerp = 0f;
        UpdateBossUI[1].Invoke(new int[] { Health, MaxHealth });

        while (lerp < 1f)
        {
            tempColorLerped = Color.LerpUnclamped(Color.red, m_SpriteColor, lerp);
            m_SpriteRenderer.color = tempColorLerped;
            lerp += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }

        m_SpriteRenderer.color = m_SpriteColor;
        _enemyMovement.SetHurt(false);
    }


    protected override void DeathCondition()
    {
        UpdateStat.Invoke(new int[] { 1, Experience });
        CameraChange.Invoke(new float[] { 12, 5 });
        UpdateBossUI[1].Invoke(new int[] { 0, MaxHealth });
        Destroy(gameObject);
        Dead = true;

        GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[0], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
    }

    private IEnumerator FireProjectile()
    {
        for (int i = 0; i < FireRepeat; i++)
        {
            for (int p = 0; p < 4; p++)
            {
                GameObject clonedProjectile = Instantiate(_projectile, transform.position, Quaternion.identity, GameObject.Find("_Enemy").transform);
                RotateToTarget(clonedProjectile, p, 4);
                clonedProjectile.transform.localEulerAngles = new Vector3(0, 0, -angle);
            }
            yield return new WaitForSeconds(0.6f);
            offsetangle = 45;
        }
        
    }

    private void RotateToTarget(GameObject projectile, int order, int total)
    {
        // Calculate the new position
        float offsetAngle = ((order * (360f + offsetangle))/ total) * Mathf.Deg2Rad; // Offset for each object
        float Target_x = transform.position.x + 1.2f * Mathf.Cos(offsetAngle);
        float Target_y = transform.position.y - 1.2f * Mathf.Sin(offsetAngle);

        projectile.transform.position = new Vector3(Target_x, Target_y, 0); // Update the position
        angle += 90f;
    }

}
