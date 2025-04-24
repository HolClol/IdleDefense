using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargadeBlastController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject FiringPoint;

    [SerializeField] float BulletLifetime = 2f;
    [SerializeField] int BulletNumb = 5;
    [SerializeField] int Bounce = 0;

    private Vector3 TargetPos;
    private Transform Target;
    private SplitterSO BonusAbilityData;
    private bool TargetSpotted;
    private float MaxAngle = 45f;

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
            Bounce = BonusAbilityData.Bounce;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        AbilitiesStat.EliteID = 1; //Set Elite ID immediately as this is an Elite weapon
        TimeBeforeFire = AbilitiesStat.Stats.Cooldown;
        BaseDamage = AbilitiesStat.Stats.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
    }

    void FixedUpdate()
    {
        if (TimeBeforeFire <= 0f && TargetSpotted && Target != null)
        {
            if (IsFacingTarget(Target.transform))
            {
                StartCoroutine(FireBullet());
                TimeBeforeFire = AbilitiesStat.Stats.Cooldown;
            }

        }
        else
        {
            TimeBeforeFire -= Time.deltaTime;
        }

        TargetSpotted = AutoTargetNearestEnemy();
        float angle = Mathf.Atan2(TargetPos.y - FiringPoint.transform.position.y, TargetPos.x - FiringPoint.transform.position.x) * Mathf.Rad2Deg;
        FiringPoint.transform.localRotation = Quaternion.Lerp(FiringPoint.transform.localRotation, Quaternion.Euler(0, 0, angle), 2.5f * Time.deltaTime);

    }

    private IEnumerator FireBullet()
    {
        for (int i = 0; i < BulletNumb; i++)
        {
            GameObject ClonedBullet = GetPooledObject(2, null, 2f);
            ClonedBullet.transform.position = transform.position;
            ClonedBullet.transform.rotation = FiringPoint.transform.rotation * Quaternion.Euler(0, 0, -90);
        }
        yield return new WaitForSeconds(0.3f);


    }

    private bool AutoTargetNearestEnemy()
    {
        bool inrange = false;
        if (playerController.EnemyInZone.Count <= 0)
            return false;

        foreach (GameObject selectFirstTarget in playerController.EnemyInZone)
        {
            TargetPos = selectFirstTarget.transform.position;
            Target = selectFirstTarget.transform;

            inrange = true;
            break;
        }

        foreach (GameObject enemyTarget in playerController.EnemyInZone)
        {
            float distance = Vector2.Distance(transform.position, enemyTarget.transform.position);
            float lastdistance = Vector2.Distance(transform.position, TargetPos);

            if (enemyTarget.name != "Molten")
                continue;

            if (distance < lastdistance)
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

    private GameObject GetPooledObject(int prefabindex, GameObject main, float Time)
    {
        for (int i = 0; i < ObjectsList.Count; i++)
        {
            if (!ObjectsList[i].activeInHierarchy)
            {
                int Main = ObjectsScriptList[i].MainProjectile ? 0 : 1;
                if (Main == prefabindex || prefabindex == 2)
                {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(
                        new int[] { AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value },
                        new float[] { AbilitiesStat.Stats.Knockback, Time, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                    if (Main == 1 && ObjectsScriptList[i] is SplitterBulletController bouncingBullet)
                        bouncingBullet.SetMain(main);
                    ObjectsScriptList[i].StartUp();
                    return ObjectsList[i];
                }

            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] { AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value },
            new float[] { AbilitiesStat.Stats.Knockback, Time, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

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

        base.IncreaseStats(upgradelevel);

        UpgradeVariables[] UpgradeTable = BonusAbilityData.NormalUpgrade;
        if (AbilitiesStat.EliteID == 1)
            UpgradeTable = BonusAbilityData.ElitePath1Upgrade;
        else if (AbilitiesStat.EliteID == 2)
            UpgradeTable = BonusAbilityData.ElitePath2Upgrade;

        var SpecialStats = UpgradeTable[index].UpgradeTable;

        // Check special stats
        foreach (var stat in SpecialStats)
        {
            switch (stat.Stat)
            {
                case StatVariables.BulletLifetime:
                    BulletLifetime += stat.Value;
                    break;
                case StatVariables.BulletNumb:
                    BulletNumb += (int)stat.Value;
                    break;
                case StatVariables.Bounce:
                    Bounce += (int)stat.Value;
                    break;
            }
        }
    }

    public override void TargetStruckSignal(GameObject[] TaggedObject)
    {
        for (int i = 0; i < Bounce; i++)
        {
            float Angle = (i * 8);
            if (Angle > MaxAngle)
            {
                Angle = MaxAngle;
            }
            GameObject ClonedBullet = GetPooledObject(1, TaggedObject[0], BulletLifetime);
            Vector3 RandomPos = new Vector3(0, 0, Random.Range(-Angle, Angle));
            ClonedBullet.transform.position = TaggedObject[0].transform.position;
            ClonedBullet.transform.rotation = TaggedObject[1].transform.rotation * Quaternion.Euler(RandomPos);
        }
    }

    public override void CheckUpgrade(int upgradelevel)
    {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        switch (WeaponUpgradeLevel)
        {
            case 8:
                MaxAngle = 90f;
                break;
            case 10:
                BulletNumb = 3;
                break;

        }
    }

    public override void UpdateDamage(int dmg)
    {
        if (this.enabled)
        {
            AbilitiesStat.Stats.Damage = BaseDamage;
            AbilitiesStat.Stats.Damage += (int)((float)(dmg) * AbilitiesStat.Stats.DamageScaling);
        }

    }
}
