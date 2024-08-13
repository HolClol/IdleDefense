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

    protected override void ReceiveDamage(int[] intinfo, float[] floatinfo)
    {
        base.ReceiveDamage(intinfo, floatinfo);
        StaleList.Add(intinfo[1]);
        if (StaleList.Count > 10)
            StaleList.RemoveAt(StaleList.Count - 1);

    }

}
