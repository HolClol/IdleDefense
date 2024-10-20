using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazorbladeController : ProjectileController
{
    [SerializeField] Animator _animator;

    [SerializeField] private GameObject _TempTarget;
    private Rigidbody2D Rb;
    private List<GameObject> Targets = new List<GameObject> {};
    private GameObject MainPlayer;

    private Vector3 ScaleValue = new Vector3(0f, 0f, 0f);
    private Vector3 TargetPos = new Vector3(0f, 0f, 0f);
    private Vector3 OriginalScale;

    private float Duration;
    private float DamageInterval;
    private float Speed;
    private int TargetNumber;
    private int HotBlade = 0;
    private bool Fired = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        OriginalScale = gameObject.transform.localScale;
        MainPlayer = GameObject.Find("Player");
        Rb = GetComponent<Rigidbody2D>();
    }

    protected override void FixedUpdate() {
        if (!Fired)
            return;

        Move(Speed);
        if (Targets.Count > 0)
        {
            CheckTarget();
            RotateToTarget(TargetPos);
        }
    }

    void LateUpdate() {
        if (Fired) {
            gameObject.transform.localScale = OriginalScale + ScaleValue;
        }
    }

    private void CheckTarget()
    {
        if (Vector3.Distance(transform.position, TargetPos) < 0.5f || _TempTarget == null || !Targets.Contains(_TempTarget))
        {
            TargetNumber = Random.Range(0, Targets.Count);
            _TempTarget = Targets[TargetNumber];
            if (Targets[TargetNumber] != null)
            {
                TargetPos = Targets[TargetNumber].transform.position;
            }

        }
    }

    private void Move(float speed) {
        Rb.velocity = transform.up * speed;
    }

    private void RotateToTarget(Vector3 targetPos) 
    {
        Vector2 Target_direction = targetPos - transform.position;
        float angle = Mathf.Atan2(Target_direction.y , Target_direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, 0.25f);
    }

    private IEnumerator BlastOff() {
        TargetNumber = 0;
        TargetPos = new Vector3(0, 0, 0);
        Fired = true;
        yield return new WaitForSeconds(Duration);
        Fired = false;

        //Manual recall for hotblades final upgrade
        if (HotBlade == 1)
        {
            float time = 0;
            do
            {
                RotateToTarget(MainPlayer.transform.position);
                Move(Speed);
                time += Time.deltaTime;
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            } while (Vector2.Distance(transform.position, MainPlayer.transform.position) >= 1f && time <= 3f);
        }
        
        gameObject.SetActive(false);
    }

    public override void StartUp() {
        base.StartUp();
        StartCoroutine(BlastOff());
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue) {
        Damage = intvalue[0];
        DamageType = intvalue[1];
        HotBlade = intvalue[2];
        Knockback = floatvalue[0];
        Duration = floatvalue[1];
        DamageInterval = floatvalue[2];
        Speed = floatvalue[4];
        CritRate = floatvalue[5];
        CritDamage = floatvalue[6];

        ScaleValue = new Vector3(1, 1, 1) * floatvalue[3];
    }

    public void GetPooledTargets(List<GameObject> EnemyPool) {
        Targets = EnemyPool;
    }

    void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { Knockback, DamageInterval, CritRate, CritDamage });
        }  
    }

    void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { Knockback, DamageInterval, CritRate, CritDamage });
        }  
    }
}
