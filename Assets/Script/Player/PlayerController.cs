using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerController : MonoBehaviour
{
    [System.Serializable] public class PlayerUpgradeStat {
        public int UpgradeID;
        public int UpgradeLevel;

        public PlayerUpgradeStat(int id, int level) {
            UpgradeID = id;
            UpgradeLevel = level;
        }
    }
    [System.Serializable] public class PlayerInGameStat {
        public int BaseDamage = 20;
        [Range(10, 40)]
        public float RotateSpeed = 20;
        public List<PlayerUpgradeStat> Upgrades;
    }

    [Header("Player Set Up")]
    public GameObject CrosshairTarget;
    public GameObject PlayerChar;
    public PlayerInGameStat PlayerStats;

    [Header("Player Manager")]
    public bool TouchPriority;
    public bool LockedIn;

    public List<GameObject> EnemyInZone;

    private Vector2 TouchPos;
    private Vector2 TargetPos;
    private bool MouseInteracting;

    // ======================================================
    // Start is called before the first frame update
    // ======================================================
    void Start()
    {
        EnhancedTouchSupport.Enable();
    }

    // ======================================================
    // Update is called once per frame
    // ======================================================
    void Update()
    {
        if (Touch.activeTouches.Count > 0) {
            Touch touch = Touch.activeTouches[0];

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
                TouchPos = Camera.main.ScreenToWorldPoint(touch.screenPosition);
                if (!TouchPriority) {
                    TouchPriority = true;
                    LockedIn = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended) {
                if (TouchPriority) {
                    TouchPriority = false;
                    LockedIn = false;
                }
            }
        }

        //Lock on to the nearest target
        if (!TouchPriority) {
            if (EnemyInZone.Count > 0) {
                LockedIn = true;
            }
            else if (EnemyInZone.Count <= 0) {
                LockedIn = false;
            }
        }

        if (Time.timeScale > 0) {
            if (LockedIn && TouchPriority) { //Mouse Target
                TargetPos = TouchPos;

                // Rotate to the target
                float angle = Mathf.Atan2(TargetPos.y - transform.position.y, TargetPos.x - transform.position.x) * Mathf.Rad2Deg;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, angle), (PlayerStats.RotateSpeed) * Time.deltaTime);
                CrosshairTarget.transform.position = TargetPos;
            }
            else if (LockedIn && !TouchPriority) { //Nearest Enemy Target Auto
                EnemyInZone.RemoveAll(GameObject => GameObject == null);
                TargetPos = EnemyInZone[0].transform.position;
                AutoTargetNearestEnemy();

                // Rotate to the target
                float angle = Mathf.Atan2(CrosshairTarget.transform.position.y - transform.position.y, CrosshairTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
                transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, angle), (PlayerStats.RotateSpeed * 0.5f) * Time.deltaTime);
                CrosshairTarget.transform.position = Vector3.Lerp(CrosshairTarget.transform.position, TargetPos, (PlayerStats.RotateSpeed * 0.25f) * Time.deltaTime);
            }

        }

    }

    private void AutoTargetNearestEnemy() {
        EnemyInZone.RemoveAll(GameObject => GameObject == null);
        foreach (GameObject enemyTarget in EnemyInZone) {
            if (enemyTarget != null) {
                float distance = Vector2.Distance(transform.position, enemyTarget.transform.position);
                float lastdistance = Vector2.Distance(transform.position, TargetPos);
                // Target nearest enemy
                if (distance < lastdistance & enemyTarget.name != "Molten") {
                    TargetPos = enemyTarget.transform.position;
                }

            }

        }
    }

}
