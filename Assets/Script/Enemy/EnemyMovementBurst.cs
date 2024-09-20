using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementBurst : EnemyMovement
{
    [SerializeField] float BurstCooldown = 4f;
    [SerializeField] float BurstMultiplier = 10f;
    [SerializeField] int BurstRepeat = 10;

    protected override void MoveMethod(float deltaTime) {
        if (CurrentCooldown <= 0) {
            StartCoroutine(BurstMovement(BurstMultiplier, BurstRepeat, deltaTime));
        }
        else {
            Move(MainEnemyController.EnemyMovespeed);
            CurrentCooldown -= deltaTime; 
        }
    }

    private IEnumerator BurstMovement(float multi, int repeat, float deltaTime) {
        for (int i = 0; i < repeat; i++) {
            Move(MainEnemyController.EnemyMovespeed *multi);
            yield return new WaitForSeconds(0);
        }

        CurrentCooldown = BurstCooldown;
    }
}
