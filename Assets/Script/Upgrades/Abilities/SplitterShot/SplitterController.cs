using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject FiringPoint;
    [SerializeField] GameObject ChargedFiringPoint;
    [SerializeField] GameObject _lineRenderer;

    [SerializeField] float BulletLifetime = 0.65f;
    [SerializeField] int BulletNumb = 5;
    [SerializeField] int Radius = 6;
    [SerializeField] int Bounce = 0;

    private ChargadeBlastController ChargedBlastScript;
    private Vector3 TargetPos;
    private Transform Target;
    private GameObject clonedlineRenderer;
    private SplitterSO BonusAbilityData;  
    private bool TargetSpotted, AutoFire, ChargedBlast, Reloading;
    private int Ammo = 6;
    private float CurrentTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        // Copy the data from the ScriptableObject
        if (AbilitySO.GetType().Equals(typeof(SplitterSO)))
        {
            BonusAbilityData = (SplitterSO)AbilitySO;
            BulletLifetime = BonusAbilityData.BulletLifetime;
            BulletNumb = BonusAbilityData.BulletNumb;
            Radius = BonusAbilityData.Radius;
            Bounce = BonusAbilityData.Bounce;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        AutoFire = true;
        ChargedBlastScript = ChargedFiringPoint.GetComponent<ChargadeBlastController>();
        TimeBeforeFire = AbilitiesStat.Stats.Cooldown;
        BaseDamage = AbilitiesStat.Stats.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
        clonedlineRenderer = Instantiate(_lineRenderer, transform.position, Quaternion.identity);
        clonedlineRenderer.GetComponent<SplitterLineRange>().IncreaseSize(Radius*10);
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f && TargetSpotted && Target != null) {
            if (IsFacingTarget(Target.transform) && Ammo > 0)
            {
                StartCoroutine(FireBullet());
                TimeBeforeFire = AbilitiesStat.Stats.Cooldown;
                if (AutoFire)
                {
                    Ammo -= 1;
                    Reloading = true;
                    CurrentTime = Time.time;
                }        
            }
            if (Ammo <= 0) 
                TimeBeforeFire = 2f;
        }
        else if (TimeBeforeFire > 0)
        {
            TimeBeforeFire -= Time.deltaTime;

            if (AutoFire && Ammo != 6)
                if (Time.time - CurrentTime >= 2f && Reloading)
                {
                    Reloading = false;
                    Ammo = 6;
                }
                          
        }
 
        TargetSpotted = AutoTargetNearestEnemy();
        float angle = Mathf.Atan2(TargetPos.y - FiringPoint.transform.position.y, TargetPos.x - FiringPoint.transform.position.x) * Mathf.Rad2Deg;
        FiringPoint.transform.localRotation = Quaternion.Lerp(FiringPoint.transform.localRotation, Quaternion.Euler(0, 0, angle), 2.5f * Time.deltaTime);
        
    }

    private IEnumerator FireBullet() {
        for (int i = 0; i < BulletNumb; i++) {
            float Angle = (i*8);
            if (Angle > 60) {
                Angle = 60;
            }
            GameObject ClonedBullet = GetPooledObject(0, null);
            Vector3 RandomPos = new Vector3(0, 0, Random.Range(-Angle, Angle));
            ClonedBullet.transform.position = transform.position;
            ClonedBullet.transform.rotation = FiringPoint.transform.rotation * Quaternion.Euler(0,0,-90) * Quaternion.Euler(RandomPos);
        }
        yield return new WaitForSeconds(0.25f);
 
    }

    private bool AutoTargetNearestEnemy() {
        bool inrange = false;
        if (playerController.EnemyInZone.Count <= 0) 
            return false;

        foreach (GameObject selectFirstTarget in playerController.EnemyInZone)
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

        foreach (GameObject enemyTarget in playerController.EnemyInZone)
        {
            float distance = Vector2.Distance(transform.position, enemyTarget.transform.position);
            float lastdistance = Vector2.Distance(transform.position, TargetPos);

            if (enemyTarget.name != "Molten")
                continue;

            if (distance < Radius && distance < lastdistance)
            {
                TargetPos = enemyTarget.transform.position;
                Target = enemyTarget.transform;

                inrange = true;
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

    private GameObject GetPooledObject(int prefabindex, GameObject main) {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                int Main = ObjectsScriptList[i].MainProjectile ? 0 : 1;
                if (ObjectsScriptList[i].Index == prefabindex)
                {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(
                        new int[] { AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value },
                        new float[] { AbilitiesStat.Stats.Knockback, BulletLifetime, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                    if (Main == 1 && ObjectsScriptList[i] is SplitterBulletController bouncingBullet)
                        bouncingBullet.SetMain(main);
                    ObjectsScriptList[i].StartUp();
                    return ObjectsList[i];
                }        
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] {AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value}, 
            new float[] {AbilitiesStat.Stats.Knockback, BulletLifetime, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;
        ObjectNew.GetComponent<ProjectileController>().Index = prefabindex;
        ObjectNew.GetComponent<ProjectileController>().StartUp();

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        if (ObjectNew.GetComponent<ProjectileController>() is SplitterBulletController bounceBullet && !ObjectNew.GetComponent<SplitterBulletController>().MainProjectile)
            bounceBullet.SetMain(main);
        return ObjectNew;
    }

    protected override void IncreaseStats(int upgradelevel)
    {
        int index = upgradelevel - 1;
        if (AbilitiesStat.EliteID != 0)
            index -= 5;
        if (index < 0) return;
        /*SplitterSO.EnhanceUpgrade[] UpgradeTable = BonusAbilityData.NormalUpgrade;
        if (AbilitiesStat.EliteID == 1)
            UpgradeTable = BonusAbilityData.ElitePath1Upgrade;
        else if (AbilitiesStat.EliteID == 2)
            UpgradeTable = BonusAbilityData.ElitePath2Upgrade;

        var BaseStats = UpgradeTable[index].LevelUp;
        var SpecialStats = UpgradeTable[index];

        // Check base stats
        if (BaseStats.Damage != 0)
            AbilitiesStat.Stats.Damage += BaseStats.Damage;
        if (BaseStats.Cooldown != 0)
            AbilitiesStat.Stats.Cooldown -= AbilitiesStat.Stats.Cooldown * (BaseStats.Cooldown / 100);
        if (BaseStats.Knockback != 0)
            AbilitiesStat.Stats.Knockback += BaseStats.Knockback;
        if (BaseStats.DamageScaling != 0)
            AbilitiesStat.Stats.DamageScaling += BaseStats.DamageScaling;
        if (BaseStats.CritRate != 0)
            AbilitiesStat.Stats.CritRate += BaseStats.CritRate;
        if (BaseStats.CritDamage != 0)
            AbilitiesStat.Stats.CritDamage += BaseStats.CritDamage;

        // Check special stats
        if (SpecialStats.BulletLifetime != 0)
            BulletLifetime += SpecialStats.BulletLifetime;
        if (SpecialStats.BulletNumb != 0)
            BulletNumb += SpecialStats.BulletNumb;
        if (SpecialStats.Radius != 0)
            Radius += SpecialStats.Radius;
        if (SpecialStats.Bounce != 0)
            Bounce += SpecialStats.Bounce;*/
    }

    public override void TargetStruckSignal(GameObject[] TaggedObject) {
        for (int i = 0; i < Bounce; i++) {
            float Angle = (i*8);
            if (Angle > 45) {
                Angle = 45;
            }
            GameObject ClonedBullet = GetPooledObject(1, TaggedObject[0]);
            Vector3 RandomPos = new Vector3(0, 0, Random.Range(-Angle, Angle));
            ClonedBullet.transform.position = TaggedObject[0].transform.position;
            ClonedBullet.transform.rotation = TaggedObject[1].transform.rotation * Quaternion.Euler(RandomPos);
        }
    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        switch (WeaponUpgradeLevel) {
            case 0:
                FiringPoint.SetActive(true);
                break;
            case 1:
                /*BulletLifetime = 1.2f;
                Radius += 3;*/
                clonedlineRenderer.GetComponent<SplitterLineRange>().IncreaseSize(Radius);
                break;
            /*case 2:
                AbilitiesStat.Stats.DamageScaling = 0.25f;
                AbilitiesStat.Stats.Knockback = 6f;
            break;
            case 3:
                Bounce = 2;
            break;
            case 4:
                BulletNumb = 8;
            break;
            case 5:
                Bounce = 3;
                AbilitiesStat.Stats.DamageScaling = 0.5f;
                AbilitiesStat.Stats.Cooldown -= 1f;
                break;*/
            case 6:
                if (AbilitiesStat.EliteID == 1)
                {
                    ChargedBlast = true;
                    ChargedFiringPoint.SetActive(true);
                }
                     
                else if (AbilitiesStat.EliteID == 2)
                {
                    AutoFire = true;
                    clonedlineRenderer.GetComponent<SplitterLineRange>().IncreaseSize(-2);
                }
                break;
        }
        BaseDamage += 2;
        UpdateDamage(playerController.PlayerStats.BaseDamage);
        if (ChargedBlast)
            ChargedBlastScript.CheckUpgrade(WeaponUpgradeLevel);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Stats.Damage = BaseDamage;
            AbilitiesStat.Stats.Damage += (int)((float)(dmg) * AbilitiesStat.Stats.DamageScaling);
        }
        
    }

}
