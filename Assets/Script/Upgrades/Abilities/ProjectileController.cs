using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileController : MonoBehaviour
{
    public AbilitiesController MainScript;
    public bool MainProjectile;
    public UnityEvent<GameObject, int[], float[]> ResponseDamage;

    protected float Knockback;
    protected int Damage;
    protected int DamageType;
    protected float CritRate = 0.05f;
    protected float CritDamage = 1f;
    public int ID;
    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    protected virtual void Start()
    {

    }

    public virtual void StartUp() 
    {
        
    }

    // ======================================================
    // Update is called once per frame
    // ======================================================
    protected virtual void FixedUpdate()
    {   

    }

    private int CritCalculate(int dmg, float cc, float cdmg)
    {
        if (Random.Range(0, 100) > 100 - (int)(cc * 100f))
        {
            dmg = dmg + (int)((float)dmg * cdmg);
        }
        return dmg;
    }
    protected virtual void SendDamage(GameObject target, int[] intstat, float[] floatstat)
    {
        int CritHit = CritCalculate(intstat[0], floatstat[2], floatstat[3]);
        int Crit = CritHit != intstat[0] ? 1 : 0;
        intstat[0] = CritHit;
        intstat[3] = Crit;
        ResponseDamage.Invoke(target, intstat, floatstat);
    }

    public virtual void UpdateStat(int[] intvalue, float[] floatvalue)
    {

    }

}
