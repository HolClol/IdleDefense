using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TheSpikerDeath : EnemyDeathHandler
{
    public UnityEvent PlayerWin;
    private TheSpikerAI BossMain;
    private Animator _animator;
    protected override void Start()
    {
        base.Start();
        BossMain = GetComponent<TheSpikerAI>();
        _animator = GetComponent<Animator>();
    }
    public override void DeathCondition()
    {
        _animator.Play("Death");
        UpdateStat.Invoke(new int[] { 1, BossMain.Experience });
        UpdateStat.Invoke(new int[] { 8, m_enemyState.MaxHealth/20, m_enemyState.Experience / 1000 });
        BossMain.CameraChange.Invoke(new float[] { 20, 10 });
        BossMain.UpdateBossUI[1].Invoke(new int[] { 0, BossMain.MaxHealth });
        BossMain.Dead = true;
        Invoke("CallWin", 5);

        GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[2], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
        GameObject ClonedParticle2 = Instantiate(EnemyVFXPrefab.EffectPrefab[3], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
    }

    private void CallWin()
    {
        PlayerWin.Invoke();
        Destroy(gameObject);
    }
}
