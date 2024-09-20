using DG.Tweening.Core.Easing;
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
    [SerializeField] int Radius = 12;
    [SerializeField] int BeamCount = 1;
    [SerializeField] int Piercing = 1;
    [SerializeField] int FractionBeam = 3;

    private bool TargetSpotted;
    private bool FractionUnlocked = true;
    private GameObject clonedlineRenderer;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        // Copy the data from the ScriptableObject
        if (AbilitySO.GetType().Equals(typeof(LancerBeamSO)))
        {
            LancerBeamSO BonusAbilityData = (LancerBeamSO)AbilitySO;
            BeamLifetime = BonusAbilityData.BeamLifetime;
            DamageInterval = BonusAbilityData.DamageInterval;
            Radius = BonusAbilityData.Radius;
            BeamCount = BonusAbilityData.BeamCount;
            Piercing = BonusAbilityData.Piercing;
            FractionBeam = BonusAbilityData.FractionBeam;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        TimeBeforeFire = AbilitiesStat.Cooldown / 2;
        BaseDamage = AbilitiesStat.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
        clonedlineRenderer = Instantiate(_lineRenderer, transform.position, Quaternion.identity);
        clonedlineRenderer.GetComponent<SplitterLineRange>().IncreaseSize(Radius * 10);
    }

    void FixedUpdate()
    {
        if (TimeBeforeFire <= 0f && TargetSpotted)
        {
            TimeBeforeFire = AbilitiesStat.Cooldown;
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
            GameObject ClonedBeam = GetPooledObject(0);
            

        }

    }

    private List<GameObject> GetFractionBeam()
    {
        List<GameObject> FractionBeamList = new List<GameObject> { };
        for (int y = 0; y < FractionBeam; y++)
        {
            GameObject ClonedFractionBeam = GetPooledObject(1);
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

    private GameObject GetPooledObject(int prefabindex)
    {
        for (int i = 0; i < ObjectsList.Count; i++)
        {
            if (!ObjectsList[i].activeInHierarchy)
            {
                int Main = ObjectsScriptList[i].MainProjectile ? 0 : 1;
                if (Main == prefabindex)
                {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(
                        new int[] { AbilitiesStat.Damage, AbilitiesStat.DamageType.Value, Piercing, FractionUnlocked ? 1 : 0, FractionBeam },
                        new float[] { AbilitiesStat.Knockback, BeamLifetime, DamageInterval, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
                    ObjectsScriptList[i].StartUp();
                    if (Main == 0 && FractionUnlocked && ObjectsScriptList[i] is BeamController mainBeam)
                        mainBeam.FractionBeam = GetFractionBeam();
                    return ObjectsList[i];
                }
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] { AbilitiesStat.Damage, AbilitiesStat.DamageType.Value, Piercing, FractionUnlocked ? 1 : 0, FractionBeam },
            new float[] { AbilitiesStat.Knockback, BeamLifetime, DamageInterval, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        if (ObjectNew.GetComponent<ProjectileController>() is BeamController derivedInstance)
        {
            derivedInstance.GetPooledTargets(playerController.EnemyInZone);
            if (derivedInstance.MainProjectile == true && FractionUnlocked)
                derivedInstance.FractionBeam = GetFractionBeam();
        }  
        return ObjectNew;
    }

    public override void CheckUpgrade(int upgradelevel)
    {
        WeaponUpgradeLevel = upgradelevel;

        switch (WeaponUpgradeLevel)
        {
            case 1:
                AbilitiesStat.DamageScaling += 0.2f;
                break;
            case 2:
                BeamCount += 1;
                break;
            case 3:
                BeamCount += 2;
                break;
            case 4:
                // No debuff yet lol
                break;
            case 5:
                Piercing = 4;
                break;
            #region ELITE UPGRADE
            case 6:
                if (AbilitiesStat.EliteID == 1)
                {
                    //Not yet
                }
                else if (AbilitiesStat.EliteID == 2)
                    FractionUnlocked = true;
                break;
            case 7:
                if (AbilitiesStat.EliteID == 1)
                { }
                else if (AbilitiesStat.EliteID == 2)
                    AbilitiesStat.DamageScaling += 0.25f;
                break;
            case 8:
                if (AbilitiesStat.EliteID == 1)
                { }
                else if (AbilitiesStat.EliteID == 2)
                    FractionBeam += 1;
                break;
            case 9:
                /*if (AbilitiesStat.EliteID == 1)
                    Piercing += 1;
                else if (AbilitiesStat.EliteID == 2)
                    AdditionalBulletSpeed += 5f;*/
                break;
            case 10:
                if (AbilitiesStat.EliteID == 1)
                { }
                else if (AbilitiesStat.EliteID == 2)
                    Piercing += 1;
                break;
                #endregion
        }

        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg)
    {
        if (this.enabled)
        {
            AbilitiesStat.Damage = BaseDamage;
            AbilitiesStat.Damage += (int)((float)(dmg) * AbilitiesStat.DamageScaling);
        }

    }
}
