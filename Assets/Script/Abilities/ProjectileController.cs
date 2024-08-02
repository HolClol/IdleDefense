using System;
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

    protected virtual void SendDamage(GameObject target, int[] intstat, float[] floatstat)
    {
        ResponseDamage.Invoke(target, intstat, floatstat);
    }

    public virtual void UpdateStat(int[] intvalue, float[] floatvalue)
    {

    }

}
