using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementZigzag : EnemyMovement
{
    [SerializeField] float ZigzagMovement = 3f;
    [SerializeField] float BurstMultiplier = 5f;
    private int ZigzagAngle = 1;

    protected override void MoveMethod(float deltaTime) {
        if (CurrentCooldown <= 0) {
            ZigzagAngle = ZigzagAngle * -1;
            StartCoroutine(BurstMovement(BurstMultiplier, 5, deltaTime));
        }
        else { 
            CurrentCooldown -= deltaTime;
            Rb.velocity = transform.right * ZigzagAngle * MainEnemyController.EnemyMovespeed;
        }
    }

    private IEnumerator BurstMovement(float multi, int repeat, float deltaTime) {
        for (int i = 0; i < repeat; i++) {
            Move(MainEnemyController.EnemyMovespeed * multi);
            yield return new WaitForSeconds(deltaTime);
        }

        CurrentCooldown = ZigzagMovement;
    }
}
