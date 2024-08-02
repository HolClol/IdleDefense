using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheSpikerMovement : EnemyMovement
{
    private bool Stop = false;
    private int ZigzagAngle = 1;
    private float ZigzagMovement = 5f;

    protected override void Update()
    {
        
    }

    protected override void MoveMethod(float deltaTime)
    {
        if (ZigzagMovement < 0 && !Stop)
        {
            StartCoroutine(BurstMovement(0.8f, 20, deltaTime));
        }
        else if (!Stop)
        {
            ZigzagMovement -= deltaTime;
            Rb.velocity = transform.right * ZigzagAngle * MainEnemyController.EnemyMovespeed;
        }
    }

    private IEnumerator BurstMovement(float multi, int repeat, float deltaTime)
    {
        Stop = true;
        for (int i = 0; i < repeat; i++)
        {
            Move(-MainEnemyController.EnemyMovespeed * multi);
            yield return new WaitForSeconds(deltaTime);
        }

        ZigzagAngle = ZigzagAngle * -1;
        ZigzagMovement = 10f;
        Stop = false;
    }
}
