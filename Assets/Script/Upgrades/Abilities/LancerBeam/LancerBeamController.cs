using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancerBeamController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject _lineRenderer;

    [SerializeField] float BeamLifetime = 4f;
    [SerializeField] float DamageInterval = 0.5f;
    [SerializeField] float SizeIncrease = 0f;
    [SerializeField] int Radius = 12;
    [SerializeField] int BeamCount = 1;
    [SerializeField] int Piercing = 1;
    [SerializeField] int FractionBeam = 3;

    private bool TargetSpotted, FractionUnlocked, BurnAfterUnlocked;
    private GameObject clonedlineRenderer;
    private LancerBeamSO BonusAbilityData;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        // Copy the data from the ScriptableObject
        if (AbilitySO.GetType().Equals(typeof(LancerBeamSO)))
        {
            BonusAbilityData = (LancerBeamSO)AbilitySO;
            BeamLifetime = BonusAbilityData.BeamLifetime;
            DamageInterval = BonusAbilityData.DamageInterval;
            SizeIncrease = BonusAbilityData.SizeIncrease;
            Radius = BonusAbilityData.Radius;
            BeamCount = BonusAbilityData.BeamCount;
            Piercing = BonusAbilityData.Piercing;
            FractionBeam = BonusAbilityData.FractionBeam;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        TimeBeforeFire = AbilitiesStat.Stats.Cooldown / 2;
        BaseDamage = AbilitiesStat.Stats.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
        clonedlineRenderer = Instantiate(_lineRenderer, transform.position, Quaternion.identity);
        clonedlineRenderer.GetComponent<SplitterLineRange>().IncreaseSize(Radius * 10);
    }

    void FixedUpdate()
    {
        if (TimeBeforeFire <= 0f && TargetSpotted)
        {
            TimeBeforeFire = AbilitiesStat.Stats.Cooldown + BeamLifetime;
            FireBeam();
        }
        else
        {
            TimeBeforeFire -= Time.deltaTime;
        }

        TargetSpotted = AutoTargetNearestEnemy();
    }

    private void FireBeam()
    {
        for (int i = 0; i < BeamCount; i++)
        {
            GameObject ClonedBeam = GetPooledObject(0, new Vector3(0,0,0));
        }

    }

    private List<GameObject> GetFractionBeam()
    {
        List<GameObject> FractionBeamList = new List<GameObject> { };
        for (int y = 0; y < FractionBeam; y++)
        {
            GameObject ClonedFractionBeam = GetPooledObject(1, new Vector3(0,0,0));
            FractionBeamList.Add(ClonedFractionBeam);
        }
        return FractionBeamList;
    }

    private bool AutoTargetNearestEnemy()
    {
        bool inrange = false;
        if (playerController.EnemyInZone.Count <= 0)
            return false;

        foreach (var enemyTarget in playerController.EnemyInZone) 
        {
            if (enemyTarget != null)
            {
                float distance = Vector2.Distance(transform.position, enemyTarget.transform.position);

                if (distance < Radius && enemyTarget.name != "Molten")
                    return true;
            }
        }

        
        return inrange;

    }

    private GameObject GetPooledObject(int prefabindex, Vector3 pos)
    {
        for (int i = 0; i < ObjectsList.Count; i++)
        {
            if (!ObjectsList[i].activeInHierarchy)
            {
                int Main = ObjectsScriptList[i].MainProjectile ? 0 : 1;
                if (Main == prefabindex || (prefabindex == 2 && ObjectsScriptList[i] is BurningBeamController))
                {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(
                        new int[] { AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value, Piercing, FractionUnlocked ? 1 : 0, BurnAfterUnlocked ? 1 : 0 },
                        new float[] { AbilitiesStat.Stats.Knockback, BeamLifetime, DamageInterval, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage, SizeIncrease });
                    if (Main == 0 && FractionUnlocked && ObjectsScriptList[i] is BeamController mainBeam)
                        mainBeam.FractionBeam = GetFractionBeam();
                    else if (Main == 0 && BurnAfterUnlocked && ObjectsScriptList[i] is BurningBeamController burnBeam)
                        burnBeam.SetPosition(pos);
                    ObjectsScriptList[i].StartUp();
                    return ObjectsList[i];
                }
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] { AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value, Piercing, FractionUnlocked ? 1 : 0, BurnAfterUnlocked ? 1:0 },
            new float[] { AbilitiesStat.Stats.Knockback, BeamLifetime, DamageInterval, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage, SizeIncrease });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        if (ObjectNew.GetComponent<ProjectileController>() is BeamController derivedInstance)
        {
            derivedInstance.GetPooledTargets(playerController.EnemyInZone);
            if (derivedInstance.MainProjectile == true && FractionUnlocked)
                derivedInstance.FractionBeam = GetFractionBeam();
        }
        else if (ObjectNew.GetComponent<ProjectileController>() is BurningBeamController elitederivedInstance)
        {
            elitederivedInstance.SetPosition(pos);
        }
        return ObjectNew;
    }

    private IEnumerator FireAfterBurner(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);
        GetPooledObject(2, pos);
    }

    protected override void IncreaseStats(int upgradelevel)
    {
        int index = upgradelevel - 1;
        if (AbilitiesStat.EliteID != 0)
            index -= 5;
        if (index < 0) return;
        LancerBeamSO.EnhanceUpgrade[] UpgradeTable = BonusAbilityData.NormalUpgrade;
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
        if (SpecialStats.BeamLifetime != 0)
            BeamLifetime += SpecialStats.BeamLifetime;
        if (SpecialStats.DamageInterval != 0)
            DamageInterval += SpecialStats.DamageInterval;
        if (SpecialStats.SizeIncrease != 0)
            SizeIncrease += SpecialStats.SizeIncrease;
        if (SpecialStats.Radius != 0)
            Radius += SpecialStats.Radius;
        if (SpecialStats.BeamCount != 0)
            BeamCount += SpecialStats.BeamCount;
        if (SpecialStats.Piercing != 0)
            Piercing += SpecialStats.Piercing;
        if (SpecialStats.FractionBeam != 0)
            FractionBeam += SpecialStats.FractionBeam;

    }

    public override void TargetStruckSignal(GameObject[] TaggedObject)
    {
        if (TaggedObject == null) return;
        StartCoroutine(FireAfterBurner(TaggedObject[0].transform.position, 0.25f));
    }


    public override void CheckUpgrade(int upgradelevel)
    {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        switch (WeaponUpgradeLevel)
        {
            /*
            case 4:
                // No debuff yet lol
                break;*/
            #region ELITE UPGRADE
            case 6:
                if (AbilitiesStat.EliteID == 1)
                    BurnAfterUnlocked = true;
                else if (AbilitiesStat.EliteID == 2)
                    FractionUnlocked = true;
                break;
            /*
            case 9:
                if (AbilitiesStat.EliteID == 1)
                    AbilitiesStat.Stats.Cooldown -= AbilitiesStat.Stats.Cooldown * 0.3f;
                *//*else if (AbilitiesStat.EliteID == 2)
                    Apply debuff;*//*
                break;*/
                #endregion
        }

        UpdateDamage(playerController.PlayerStats.BaseDamage);
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
