using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject FiringPoint;
    [SerializeField] GameObject _lineRenderer;

    private float BulletLifetime = 0.4f;
    private int BulletNumb = 3;
    private int Radius = 3;
    private int Bounce = 0;
    private int Repeat = 1;

    private Vector3 TargetPos;
    private Quaternion TargetRotate;
    private bool TargetSpotted;
    private GameObject clonedlineRenderer;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        TimeBeforeFire = AbilitiesStat.Cooldown;
        BaseDamage = AbilitiesStat.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
        clonedlineRenderer = Instantiate(_lineRenderer, transform.position, Quaternion.identity);
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f && TargetSpotted) {
            StartCoroutine(FireBullet());
            TimeBeforeFire = AbilitiesStat.Cooldown;
        }
        else {
            TimeBeforeFire -= Time.deltaTime;
        }

        TargetSpotted = AutoTargetNearestEnemy();
    }

    private IEnumerator FireBullet() {
        for (int fired = 0; fired < Repeat; fired++) {
            for (int i = 0; i < BulletNumb; i++) {
                float Angle = (i*8);
                if (Angle > 60) {
                    Angle = 60;
                }
                GameObject ClonedBullet = GetPooledObject(0);
                Vector3 RandomPos = new Vector3(0, 0, Random.Range(-Angle, Angle));
                ClonedBullet.transform.position = transform.position;
                ClonedBullet.transform.rotation = FiringPoint.transform.rotation * Quaternion.Euler(RandomPos);
            }
            yield return new WaitForSeconds(0.25f);
        }
        
        
    }

    private bool AutoTargetNearestEnemy() {
        bool inrange = false;
        if (playerController.EnemyInZone.Count > 0) {
            foreach (GameObject enemyTarget in playerController.EnemyInZone) {
                if (enemyTarget != null) {
                    float distance = Vector2.Distance(transform.position, enemyTarget.transform.position);
                    // Target nearest enemy
                    if (distance < Radius & enemyTarget.name != "Molten") {
                        TargetPos = enemyTarget.transform.position;
                        TargetRotate = enemyTarget.transform.rotation;
                        // Rotate to the target
                        float angle = Mathf.Atan2(TargetPos.y - FiringPoint.transform.position.y, TargetPos.x - FiringPoint.transform.position.x) * Mathf.Rad2Deg;
                        FiringPoint.transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, angle), 0.25f * Time.deltaTime);
                        inrange = true;
                    }   
                    
                }
                
            }
        }
        return inrange;
        
    }

    private GameObject GetPooledObject(int prefabindex) {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                int Main = ObjectsScriptList[i].MainProjectile ? 0 : 1;
                if (Main == prefabindex) {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(new int[] {AbilitiesStat.Damage}, new float[] {AbilitiesStat.Knockback, BulletLifetime});
                    ObjectsScriptList[i].StartUp();
                    return ObjectsList[i];
                }
                
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(new int[] {AbilitiesStat.Damage}, new float[] {AbilitiesStat.Knockback, BulletLifetime});
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        return ObjectNew;
    }

    public override void TargetStruckSignal(GameObject TaggedObject) {
        for (int i = 0; i < Bounce; i++) {
            float Angle = (i*8);
            if (Angle > 45) {
                Angle = 45;
            }
            GameObject ClonedBullet = GetPooledObject(1);
            Vector3 RandomPos = new Vector3(0, 0, Random.Range(-Angle, Angle));
            ClonedBullet.transform.position = TaggedObject.transform.position;
            ClonedBullet.transform.rotation = TaggedObject.transform.rotation * Quaternion.Euler(RandomPos);
        }
    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;

        switch (WeaponUpgradeLevel) {
            case 0:
                FiringPoint.SetActive(true);
                break;
            case 1:
                BulletLifetime = 0.6f;
                Radius += 1;
                clonedlineRenderer.GetComponent<SplitterLineRange>().IncreaseSize(8);
            break;
            case 2:
                DamageScaling = 0.2f;
                AbilitiesStat.Knockback = 6f;
            break;
            case 3:
                Bounce = 2;
            break;
            case 4:
                BulletNumb = 5;
            break;
            case 5:
                Repeat = 2;
            break;
            case 6:
                Bounce = 3;
                DamageScaling = 0.4f;
                AbilitiesStat.Cooldown -= 1f;
            break;
        }

        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + dmg/2;
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * DamageScaling);
        }
        
    }

}
