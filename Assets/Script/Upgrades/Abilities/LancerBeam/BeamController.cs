using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BeamController : ProjectileController
{
    [SerializeField] private GameObject _TempTarget;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private LineRenderer LaserPointer;
    private List<GameObject> Targets = new List<GameObject> { };

    private Vector3 TargetPos = new Vector3(0f, 0f, 0f);
    private LayerMask enemyLayer;

    private float LaserDistance = 12f;
    private float Duration = 4f;
    private float DamageInterval = 0.5f;
    private int TargetNumber;
    private int Piercing = 0;
    private bool Fired = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        StartUp();
    }

    protected override void FixedUpdate()
    { 
        if (!Fired)
            return;
        if (Targets.Count <= 0)
            return;

        FindTarget();

        if (_TempTarget != null)
        {
            TargetPos = _TempTarget.transform.position;
            RotateToTarget(TargetPos);

            if (IsFacingTarget(TargetPos) && Vector3.Distance(transform.position, TargetPos) <= LaserDistance + 1)
                ShootLaser();   
            else
                StartCoroutine(ResetBeam());
        }   

    }

    private void FindTarget()
    {
        if (_TempTarget == null || !Targets.Contains(_TempTarget) || Vector3.Distance(transform.position, TargetPos) > LaserDistance)
        { 
            TargetNumber = Random.Range(0, Targets.Count);
            if (Targets[TargetNumber] != null)
            {
                _TempTarget = Targets[TargetNumber];
                TargetPos = Targets[TargetNumber].transform.position;
            }
        }
    }

    private void ShootLaser()
    {
        if (!Physics2D.Raycast(transform.position, transform.right))
            return;

        RaycastHit2D[] _hit = Physics2D.RaycastAll(FirePoint.position, transform.right, LaserDistance, enemyLayer);
        if (_hit.Length == 0) 
            return; 

        // Find how many targets are within the raycast and maximum amount it can pierce
        int index = _hit.Length - 1;
        if (index > Piercing)
            index = Piercing;

        if (_hit[index].collider != null) // Main Target
        {
            Draw2DRay(FirePoint.position, _hit[index].point);
            for (int i = 0; i < index + 1; i++)
            {
                if (_hit[i].collider != null) // Check other targets within the raycast
                    if (_hit[i].collider.gameObject.CompareTag("Enemy"))
                        SendDamage(_hit[i].collider.gameObject,
                        new int[] { Damage, ID, DamageType, 0 },
                        new float[] { Knockback, DamageInterval, CritRate, CritDamage });
            }

        }

    }

    private void Draw2DRay(Vector2 StartPos, Vector2 EndPos)
    {
        LaserPointer.SetPosition(0, StartPos);
        LaserPointer.SetPosition(1, EndPos);
    }

    private bool IsFacingTarget(Vector3 target)
    {
        Vector2 forward = FirePoint.transform.right;
        Vector2 directionToTarget = (target - FirePoint.transform.position).normalized;

        float angle = Vector2.Angle(forward, directionToTarget);

        return angle < 5f;
    }

    private void RotateToTarget(Vector3 targetPos)
    {
        Vector2 Target_direction = targetPos - transform.position;
        float angle = Mathf.Atan2(Target_direction.y, Target_direction.x) * Mathf.Rad2Deg;

        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, 1f);
    }

    private IEnumerator ResetBeam()
    {
        Draw2DRay(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        Fired = false;
        yield return new WaitForSeconds(0.5f);
        Fired = true;
    }

    private IEnumerator ActivateBeam()
    {
        Fired = true;
        TargetNumber = 0;
        _TempTarget = null;
        yield return new WaitForSeconds(Duration);
        Fired = false;
        Draw2DRay(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        gameObject.SetActive(false);
    }

    public override void StartUp()
    {
        base.StartUp();
        StartCoroutine(ActivateBeam());
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue)
    {
        Damage = intvalue[0];
        DamageType = intvalue[1];
        Piercing = intvalue[2];
        Knockback = floatvalue[0];
        Duration = floatvalue[1];
        DamageInterval = floatvalue[2];
        CritRate = floatvalue[3];
        CritDamage = floatvalue[4];
    }

    public void GetPooledTargets(List<GameObject> EnemyPool)
    {
        Targets = EnemyPool;
    }
}
