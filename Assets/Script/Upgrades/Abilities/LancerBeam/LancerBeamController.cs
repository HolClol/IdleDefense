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

    private bool TargetSpotted;
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
            StartCoroutine(FireBeam());
        }
        else
        {
            TimeBeforeFire -= Time.deltaTime;
        }

        TargetSpotted = AutoTargetNearestEnemy();
    }

    private IEnumerator FireBeam()
    {
        for (int i = 0; i < BeamCount; i++)
        {
            GameObject ClonedBeam = GetPooledObject();
        }
        yield return new WaitForSeconds(0f);

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

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < ObjectsList.Count; i++)
        {
            if (!ObjectsList[i].activeInHierarchy)
            {
                ObjectsList[i].SetActive(true);
                ObjectsScriptList[i].UpdateStat(
                    new int[] { AbilitiesStat.Damage, AbilitiesStat.DamageType.Value, Piercing },
                    new float[] { AbilitiesStat.Knockback, BeamLifetime, DamageInterval, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
                ObjectsScriptList[i].StartUp();
                return ObjectsList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[0], transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] { AbilitiesStat.Damage, AbilitiesStat.DamageType.Value, Piercing },
            new float[] { AbilitiesStat.Knockback, BeamLifetime, DamageInterval, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        if (ObjectNew.GetComponent<ProjectileController>() is BeamController derivedInstance)
        {
            derivedInstance.GetPooledTargets(playerController.EnemyInZone);
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
                BeamCount += 1;
                break;
            case 4:
                // No debuff yet lol
                break;
            case 5:
                Piercing = 4;
                break;
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
