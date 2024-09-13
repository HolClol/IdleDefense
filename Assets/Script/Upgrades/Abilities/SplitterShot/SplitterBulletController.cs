using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterBulletController : ProjectileController
{  
    [SerializeField] GameObject TrailChar;
    private Rigidbody2D Rb;
    private Animator _animator;
    private TrailRenderer _trailRenderer;

    private float ProjectileSpeed = 20f;
    private float ProjectileLifetime = 0.4f;
    private bool Tagged = false;

    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    protected override void Start()
    {
        //transform.localScale += new Vector3(0, (ProjectileSpeed*0.8f)/100f, 0);
        Rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _trailRenderer = TrailChar.GetComponent<TrailRenderer>();
        Damage = 20;

        StartUp();
    }

    private IEnumerator DestroyProjectile() {
        Tagged = false;
        yield return new WaitForSeconds(ProjectileLifetime*0.4f);

        if (!Tagged) {
            _animator.Play("Disappear");
            Tagged = true;
        }

        yield return new WaitForSeconds(ProjectileLifetime*0.6f);
        gameObject.SetActive(false);
        _trailRenderer.Clear();
    }

    public override void StartUp() {
        base.StartUp();
        StartCoroutine(DestroyProjectile());
    }

    protected override void FixedUpdate()
    {   
        if (gameObject.activeInHierarchy) {
            if (!Tagged) {
                Rb.velocity = transform.up * ProjectileSpeed;
            }
            else {
                Rb.velocity = new Vector2(0, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger) 
    {
        if (trigger.gameObject.CompareTag("Enemy")) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { 4f, 0f, CritRate, CritDamage });
            if (MainProjectile) {
                MainScript.TargetStruckSignal(this.gameObject);
            }
            Tagged = true;
            _animator.Play("Disappear");
        }      
        
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue) {
        Damage = intvalue[0];
        Knockback = floatvalue[0];
        ProjectileLifetime = floatvalue[1];
        DamageType = intvalue[1];
        CritRate = floatvalue[2];
        CritDamage = floatvalue[3];
    }
}
