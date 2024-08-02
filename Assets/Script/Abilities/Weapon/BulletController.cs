using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletController : MonoBehaviour
{
    [Range(10f,100f)]
    public float ProjectileSpeed = 20f;
    [Range(1f,5f)]
    public float ProjectileLifetime = 3f;
    public int Damage = 20;
    public int Piercing = 0;
    public UnityEvent<GameObject, int[], float[]> ResponseDamage;

    [SerializeField] GameObject TrailChar;
    private Rigidbody2D Rb;
    private GameObject MainChar;
    private TrailRenderer _trailRenderer;

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
            ResponseDamage.Invoke(trigger.gameObject, new int[] { Damage, 0 }, new float[] { 0.25f, 0f });
            if (Piercing > 0) {
                Piercing -= 1;
            }
            else {
                gameObject.SetActive(false);
            }
        }      
        
    }

    public void UpdateStat(int dmg, int pierce, float speed) {
        ProjectileSpeed = 15f + speed;
        Damage = dmg;
        Piercing = pierce;
    }
}

