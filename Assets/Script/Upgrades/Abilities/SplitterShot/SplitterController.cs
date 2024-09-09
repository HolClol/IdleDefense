using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SplitterController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject FiringPoint;
    [SerializeField] GameObject _lineRenderer;

    private float BulletLifetime = 0.65f;
    private int BulletNumb = 5;
    private int Radius = 7;
    private int Bounce = 0;
    private int Repeat = 1;

    private Vector3 TargetPos;
    private Transform Target;
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
        DamageScaling = 0.6f;
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f && TargetSpotted && Target != null) {
            if (IsFacingTarget(Target.transform))
            {
                StartCoroutine(FireBullet());
                TimeBeforeFire = AbilitiesStat.Cooldown;
            }
            
        }
        else {
            TimeBeforeFire -= Time.deltaTime;
        }
 
        TargetSpotted = AutoTargetNearestEnemy();
        float angle = Mathf.Atan2(TargetPos.y - FiringPoint.transform.position.y, TargetPos.x - FiringPoint.transform.position.x) * Mathf.Rad2Deg;
        FiringPoint.transform.localRotation = Quaternion.Lerp(FiringPoint.transform.localRotation, Quaternion.Euler(0, 0, angle), 2.5f * Time.deltaTime);
        
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
                ClonedBullet.transform.rotation = FiringPoint.transform.rotation * Quaternion.Euler(0,0,-90) * Quaternion.Euler(RandomPos);
            }
            yield return new WaitForSeconds(0.25f);
        }
        
        
    }

    private bool AutoTargetNearestEnemy() {
        bool inrange = false;
        if (playerController.EnemyInZone.Count > 0) {
            foreach (GameObject selectFirstTarget in playerController.EnemyInZone)
            {
                if (selectFirstTarget != null)
                {
                    float firstdistance = Vector2.Distance(transform.position, selectFirstTarget.transform.position);
                    if (firstdistance < Radius)
                    {
                        TargetPos = selectFirstTarget.transform.position;
                        Target = selectFirstTarget.transform;

                        inrange = true;
                        break;
                    }
                }
                
            }

            foreach (GameObject enemyTarget in playerController.EnemyInZone) {
                if (enemyTarget != null) {
                    float distance = Vector2.Distance(transform.position, enemyTarget.transform.position);
                    float lastdistance = Vector2.Distance(transform.position, TargetPos);

                    if (enemyTarget.name != "Molten") 
                    {
                        if (distance < Radius && distance < lastdistance)
                        {
                            TargetPos = enemyTarget.transform.position;
                            Target = enemyTarget.transform;

                            inrange = true;
                        }
                    }   
                    
                }
                
            }
        }
        return inrange;
        
    }

    private bool IsFacingTarget(Transform target)
    {
        Vector2 forward = FiringPoint.transform.right;
        Vector2 directionToTarget = (target.position - FiringPoint.transform.position).normalized;

        float angle = Vector2.Angle(forward, directionToTarget);

        return angle < 5f;
    }

    private GameObject GetPooledObject(int prefabindex) {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                int Main = ObjectsScriptList[i].MainProjectile ? 0 : 1;
                if (Main == prefabindex) {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(
                        new int[] {AbilitiesStat.Damage, AbilitiesStat.DamageType.Value}, 
                        new float[] {AbilitiesStat.Knockback, BulletLifetime, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
                    ObjectsScriptList[i].StartUp();
                    return ObjectsList[i];
                }
                
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] {AbilitiesStat.Damage, AbilitiesStat.DamageType.Value}, 
            new float[] {AbilitiesStat.Knockback, BulletLifetime, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
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
                BulletLifetime = 1.2f;
                Radius += 3;
                clonedlineRenderer.GetComponent<SplitterLineRange>().IncreaseSize(8);
            break;
            case 2:
                DamageScaling = 0.25f;
                AbilitiesStat.Knockback = 6f;
            break;
            case 3:
                Bounce = 2;
            break;
            case 4:
                BulletNumb = 8;
            break;
            case 5:
                Bounce = 3;
                DamageScaling = 0.5f;
                AbilitiesStat.Cooldown -= 1f;
                break;
        }
        BaseDamage += 5;
        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + dmg/2;
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * DamageScaling);
        }
        
    }

}
