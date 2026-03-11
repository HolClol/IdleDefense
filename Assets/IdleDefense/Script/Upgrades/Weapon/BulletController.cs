using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletController : MonoBehaviour
{ 
    public int DamageType = 1; 

    [SerializeField] GameObject TrailChar;
    private Rigidbody2D Rb;
    private GameObject MainChar;
    private TrailRenderer _trailRenderer;
    private int Damage = 20;
    private int Piercing = 0;
    private int Bounce = 0;
    [Range(0f, 1f)] private float CritRate = 0.1f;
    private float CritDamage = 1f;
    private float ProjectileSpeed = 30f;
    [Range(1f, 5f)]
    private float ProjectileLifetime = 1.5f;

    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    private void Start()
    {
        //transform.localScale += new Vector3(0, (ProjectileSpeed*0.8f)/100f, 0);
        Rb = GetComponent<Rigidbody2D>();
        MainChar = GameObject.Find("Player");
        _trailRenderer = TrailChar.GetComponent<TrailRenderer>();

        StartUp();
    }

    private IEnumerator DestroyProjectile() {
        yield return new WaitForSeconds(ProjectileLifetime);
        gameObject.SetActive(false);
    }

    public void StartUp() {
        _trailRenderer.Clear();
        StartCoroutine(DestroyProjectile());
    }

    // ======================================================
    // Update is called once per frame
    // ======================================================
    private void FixedUpdate()
    {   
        if (gameObject.activeInHierarchy) {
            Rb.velocity = transform.up * ProjectileSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger) 
    {
        if (trigger.gameObject.CompareTag("Enemy")) {
            SendDamage(trigger.gameObject, new int[] { Damage, 0, DamageType, 0 }, new float[] { 0.25f, 0f, CritRate, CritDamage });
            if (Piercing > 0) {
                Piercing -= 1;
            }
            else {
                gameObject.SetActive(false);
            }
        }
        else if (trigger.gameObject.GetComponent<EdgeCollider2D>() && Bounce > 0)
        {
            var firstContact = trigger.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position);
            Vector2 normal = (transform.position - new Vector3(firstContact.x, firstContact.y)).normalized;
            Vector2 newVelocity = Vector2.Reflect(transform.up.normalized, normal);

            transform.up = newVelocity;
            Bounce -= 1;

        }
        
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
        DamageCalculateManager.Instance.DamageCalculate(target, intstat, floatstat);
    }

    public void UpdateStat(int[] intstat, float[] floatstat) {
        Damage = intstat[0];
        Piercing = intstat[1];
        Bounce = intstat[2];
        ProjectileSpeed = 15f + floatstat[0];
        CritRate = floatstat[1];
        CritDamage = floatstat[2];
        ProjectileLifetime = floatstat[3];
        
    }
}

