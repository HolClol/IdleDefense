using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EruptionController : ProjectileController
{
    [SerializeField] GameObject ParticleHandler;

    private ParticleSystem _particles;

    private float Delay = 2f;
    private float groundDelay = 0f;
    private bool BurningGround = false;
    
    protected override void Awake()
    {   
        _particles = ParticleHandler.GetComponent<ParticleSystem>();
        StartUp();
    }


    private IEnumerator DisableObject(float timer) {
        yield return new WaitForSeconds(timer);
        _particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        BurningGround = true;
        yield return new WaitForSeconds(groundDelay);
        gameObject.SetActive(false);
    }

    public override void StartUp() {
        base.StartUp();
        ParticleHandler.transform.localScale = transform.localScale * 0.3f;
        if (_particles != null) {
            _particles.Play();
        }
        StartCoroutine(DisableObject(Delay));
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue) {
        Damage = intvalue[0];
        DamageType = intvalue[1];
        Knockback = floatvalue[0];
        Delay = floatvalue[1];
        groundDelay = floatvalue[2];
        CritRate = floatvalue[3];
        CritDamage = floatvalue[4];
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy") && !BurningGround) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { Knockback, 0f, CritRate, CritDamage });
        }  
    }
    
    private void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            if (BurningGround) {
                SendDamage(trigger.gameObject, new int[] { Damage / 2, ID, DamageType, 0 }, new float[] { Knockback / 10, 1f, CritRate, CritDamage });
            }
            else {
                SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { Knockback, 1f, CritRate, CritDamage });
            }
            
        }  
    }
}
