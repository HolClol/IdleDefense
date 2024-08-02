using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazorbladeController : ProjectileController
{
    [SerializeField] Animator _animator;

    [SerializeField] private GameObject _TempTarget;
    private Rigidbody2D Rb;
    private List<GameObject> Targets = new List<GameObject> {};

    private Vector3 ScaleValue = new Vector3(0f, 0f, 0f);
    private Vector3 TargetPos = new Vector3(0f, 0f, 0f);
    private Vector3 OriginalScale;

    private float Duration;
    private float DamageInterval;
    private int TargetNumber;
    private bool Fired = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        OriginalScale = gameObject.transform.localScale;
        Rb = GetComponent<Rigidbody2D>();
        StartUp();
    }

    protected override void FixedUpdate() {
        if (Fired) {
            if (Targets.Count > 0) {
                if (Vector3.Distance(transform.position, TargetPos) < 0.5f || _TempTarget == null || !Targets.Contains(_TempTarget))
                {
                    TargetNumber = Random.Range(0, Targets.Count);
                    _TempTarget = Targets[TargetNumber];
                    if (Targets[TargetNumber] != null)
                    {
                        TargetPos = Targets[TargetNumber].transform.position;
                    }
                    
                }

                RotateToTarget(TargetPos);

            }
            Move(20f);
        }
    }

    void LateUpdate() {
        if (Fired) {
            gameObject.transform.localScale = OriginalScale + ScaleValue;
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
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, 0.2f);
    }

    private IEnumerator BlastOff() {
        TargetNumber = 0;
        TargetPos = new Vector3(0, 0, 0);
        Fired = true;
        yield return new WaitForSeconds(Duration);
        Fired = false;
        gameObject.SetActive(false);
    }

    public override void StartUp() {
        StartCoroutine(BlastOff());
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue) {
        Damage = intvalue[0];
        Knockback = floatvalue[0];
        Duration = floatvalue[1];
        DamageInterval = floatvalue[2];

        ScaleValue = new Vector3(1, 1, 1) * floatvalue[3];
    }

    public void GetPooledTargets(List<GameObject> EnemyPool) {
        Targets = EnemyPool;
    }

    void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            ResponseDamage.Invoke(trigger.gameObject, new int[] { Damage, ID }, new float[] { Knockback, DamageInterval });
        }  
    }

    void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            ResponseDamage.Invoke(trigger.gameObject, new int[] { Damage, ID }, new float[] { Knockback, DamageInterval });
        }  
    }
}
