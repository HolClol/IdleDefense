using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSpikerDeath : EnemyDeathHandler
{
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
        BossMain.CameraChange.Invoke(new float[] { 15, 10 });
        BossMain.UpdateBossUI[1].Invoke(new int[] { 0, BossMain.MaxHealth });
        Destroy(gameObject, 5);
        BossMain.Dead = true;

        GameObject ClonedParticle = Instantiate(EnemyVFXPrefab.EffectPrefab[2], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
        GameObject ClonedParticle2 = Instantiate(EnemyVFXPrefab.EffectPrefab[3], transform.position, transform.rotation, GameObject.Find("_Projectiles").transform);
    }
}
