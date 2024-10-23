using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanBladeController : ProjectileController
{
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _TempTarget;
    private Rigidbody2D Rb;

    private Vector3 ScaleValue = new Vector3(0f, 0f, 0f);
    private Vector3 TargetPos = new Vector3(0f, 0f, 0f);
    private Vector3 OriginalScale;

    private float Speed = 10f;
    private float Duration;
    private float DamageInterval;
    private bool Fired = false;
    // Start is called before the first frame update
    protected override void Awake()
    {
        OriginalScale = gameObject.transform.localScale;
        _TempTarget = GameObject.Find("Player");
        Rb = GetComponent<Rigidbody2D>();
        StartUp();
    }

    protected override void FixedUpdate()
    {
        if (!Fired)
            return;

        Rb.velocity = transform.up * 3f;
        transform.RotateAround(TargetPos, Vector3.forward, Speed/2f);
        //transform.Rotate(Vector3.forward, -1f);

    }

    void LateUpdate()
    {
        if (Fired)
        {
            gameObject.transform.localScale = OriginalScale + ScaleValue;
        }
    }

    private IEnumerator BlastOff()
    {
        if (MainProjectile)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else 
            transform.rotation = Quaternion.Euler(0, 0, 180f);

        TargetPos = _TempTarget.transform.position;
        Fired = true;
        yield return new WaitForSeconds(Duration);
        Fired = false;
        gameObject.SetActive(false);
    }

    protected override void OnEnable()
    {
        base.StartUp();
        StartCoroutine(BlastOff());
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue)
    {
        Damage = intvalue[0];
        DamageType = intvalue[1];
        Knockback = floatvalue[0];
        Duration = floatvalue[1];
        DamageInterval = floatvalue[2];
        Speed = floatvalue[4];
        CritRate = floatvalue[5];
        CritDamage = floatvalue[6];

        ScaleValue = new Vector3(1, 1, 1) * floatvalue[3];
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { Knockback, DamageInterval, CritRate, CritDamage });
        }
    }

    void OnTriggerStay2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Enemy"))
        {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { Knockback, DamageInterval, CritRate, CritDamage });
        }
    }
}
