using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileController : MonoBehaviour
{
    [HideInInspector] public AbilitiesController MainScript;
    public bool MainProjectile;
    public UnityEvent<GameObject, int[], float[]> ResponseDamage;

    protected Dictionary<GameObject, float> damageCooldowns = new Dictionary<GameObject, float>();
    protected float Knockback;
    protected int Damage;
    protected int DamageType;
    protected float CritRate = 0.05f;
    protected float CritDamage = 1f;
    public int ID;
    public int Index;
    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    protected virtual void Awake()
    {

    }

    public virtual void StartUp() 
    {
        damageCooldowns.Clear();
    }

    // ======================================================
    // Update is called once per frame
    // ======================================================
    protected virtual void FixedUpdate()
    {   

    }

    private IEnumerator DamageCooldownPlay(GameObject EnemySource, float cooldown)
    {
        damageCooldowns[EnemySource] = cooldown;
        yield return new WaitForSeconds(cooldown);
        damageCooldowns.Remove(EnemySource);
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
        if (!damageCooldowns.ContainsKey(target))
        {
            int CritHit = CritCalculate(intstat[0], floatstat[2], floatstat[3]);
            int Crit = CritHit != intstat[0] ? 1 : 0;
            intstat[0] = CritHit;
            intstat[3] = Crit;
            ResponseDamage.Invoke(target, intstat, floatstat);
            StartCoroutine(DamageCooldownPlay(target, floatstat[1]));
        }
            
    }

    public virtual void UpdateStat(int[] intvalue, float[] floatvalue)
    {

    }

}
