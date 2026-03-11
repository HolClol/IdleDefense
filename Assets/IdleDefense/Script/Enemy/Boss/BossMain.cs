using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossMain : EnemyMain
{
    [Header("Events")]
    public UnityEvent<float[]> CameraChange;
    public UnityEvent<int[]>[] UpdateBossUI;

    protected List<int> StaleList = new List<int>();

    protected override void TakeDamage(int id)
    {
        int damagestale = 0;
        foreach (var staleid in StaleList)
        {
            if (staleid == id) { damagestale++; }
        }
        float damagereduction = (float)DamageDealt * ((float)damagestale * 0.04f);
        DamageDealt -= (int)damagereduction; 

        Health -= DamageDealt;
     }

    public override int ReceiveDamage(int[] intinfo, float[] floatinfo)
    {
        DamageDealt = intinfo[0];
        float DamageKnockback = ((float)DamageDealt / 50) + floatinfo[0];
        if (DamageKnockback > 30f)
        {
            DamageKnockback = 30f;
        }
        m_enemyMovement.SetDamageKnockback(DamageKnockback);

        if (Health <= DamageDealt && !Dead)
        {
            m_enemyDeath.DeathCondition();
        }
        else
        {
            TakeDamage(intinfo[1]);
            StartCoroutine(HurtPlay());
            StaleList.Add(intinfo[1]);
            if (StaleList.Count > 10)
                StaleList.RemoveAt(StaleList.Count - 1);
        }
        return DamageDealt;
    }

}
