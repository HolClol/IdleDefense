using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float Delay;

    protected Transform Target;
    protected EnemyMain MainEnemyController;
    protected Rigidbody2D Rb;
    protected bool Hurt;
    protected float DamageKnockback;
    protected float CurrentCooldown = 0;
    protected float DelayPlay;

    protected virtual void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        MainEnemyController = GetComponent<EnemyMain>();
        DelayPlay = Delay;
    }

    protected virtual void Update()
    {
        if (DelayPlay <= 0)
        {
            if (!Target)
                GetTarget();
            else
                RotateToTarget();
        }
        else
        {
            DelayPlay -= Time.deltaTime;
        }
        
    }

    protected void GetTarget()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            Target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void FixedUpdate()
    {
        if (!MainEnemyController.Dead)
            MoveProgress(Time.fixedDeltaTime);
        else
            Rb.velocity = Vector3.zero;
        
    }

    protected void MoveProgress(float deltaTime)
    {
        if (!Hurt)
        {
            MoveMethod(deltaTime);
        }
        else
        {
            if (MainEnemyController.EnemyType.EnemyID != 1)
                Rb.velocity = transform.up * -(MainEnemyController.EnemyMovespeed * DamageKnockback);
            else
                Rb.velocity = new Vector2(0, 0);
        }
    }

    protected virtual void MoveMethod(float deltaTime)
    {
        Move(MainEnemyController.EnemyMovespeed);
    }

    protected void Move(float speed)
    {
        Rb.velocity = transform.up * speed;
        //transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, speed * Time.deltaTime);
    }

    protected void RotateToTarget()
    {
        Vector2 Target_direction = Target.position - transform.position;
        float angle = Mathf.Atan2(Target_direction.y, Target_direction.x) * Mathf.Rad2Deg - 90f;

        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, (MainEnemyController.EnemyRotateSpeed * 0.005f));
    }

    public void SetHurt(bool value)
    {
        Hurt = value;
    }

    public void SetDamageKnockback(float value)
    {
        DamageKnockback = value;
    }
}
