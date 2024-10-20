using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningBeamController : ProjectileController
{ 
    [SerializeField] private Transform FirePoint;
    [SerializeField] private LineRenderer LaserPointer;

    private Vector3 TargetPos = new Vector3(0f, 0f, 0f);
    private LayerMask enemyLayer;

    private float LaserDistance = 12f;
    private float Duration = 0.5f;
    private float DamageInterval = 2f;
    private float StartWidth, EndWidth;
    private int Piercing = 0;
    private bool Fired = false;
    private Tween fadeTween;

    protected override void Awake()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        StartWidth = LaserPointer.startWidth;
        EndWidth = LaserPointer.endWidth;
        fadeTween = DOTween.ToAlpha(() => LaserPointer.material.color, x => LaserPointer.material.color = x, 0f, 0.5f)
            .From(1f)
            .SetDelay(0f)
            .SetAutoKill(false)
            .Pause()
            .OnComplete(() => { gameObject.SetActive(false); });
    }

    protected override void FixedUpdate()
    {
        if (!Fired)
            return;

        if (TargetPos != null)
        {
            RotateToTarget(TargetPos);
            ShootLaser();
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
                    {
                        SendDamage(_hit[i].collider.gameObject,
                        new int[] { Damage, ID, DamageType, 0 },
                        new float[] { Knockback, DamageInterval, CritRate, CritDamage });
                    }

            }

        }

    }

    private void Draw2DRay(Vector2 StartPos, Vector2 EndPos)
    {
        LaserPointer.SetPosition(0, StartPos);
        LaserPointer.SetPosition(1, EndPos);
    }

    private void RotateToTarget(Vector3 targetPos)
    {
        Vector2 Target_direction = targetPos - transform.position;
        float angle = Mathf.Atan2(Target_direction.y, Target_direction.x) * Mathf.Rad2Deg;

        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = q;
    }

    private IEnumerator ActivateBeam()
    {
        fadeTween.Rewind();
        Fired = true;
        yield return new WaitForSeconds(Duration);
        fadeTween.Restart();
        
    }

    public void OnDisable()
    {
        Fired = false;
        Draw2DRay(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        TargetPos = new Vector3(0, 0, 0);
    }

    public override void StartUp()
    {
        base.StartUp();
        StartCoroutine(ActivateBeam());
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue)
    {
        Damage = (int)((float)intvalue[0] * 0.7f);
        DamageType = intvalue[1];
        Piercing = intvalue[2];
        Knockback = floatvalue[0];
        CritRate = floatvalue[3];
        CritDamage = floatvalue[4];

        if (LaserPointer.startWidth != StartWidth + floatvalue[5])
            LaserPointer.widthCurve = AnimationCurve.Linear(0f, StartWidth + floatvalue[5], 1f, EndWidth + floatvalue[5]);
    }

    public void SetPosition(Vector3 pos)
    {
        TargetPos = pos;
    }

}
