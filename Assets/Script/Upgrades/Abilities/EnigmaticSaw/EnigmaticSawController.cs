using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaticSawController : AbilitiesController
{
    [SerializeField] int RazorbladeNumbers = 1;
    [SerializeField] float AdditionalScale = 0f;
    [SerializeField] float Duration = 5f;
    [SerializeField] float DamageInterval = 0.6f;
    [SerializeField] float Speed = 20f;

    private EnigmaticSO BonusAbilityData;
    private bool TitanBlade, HotBlade, MaxHotBlade;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        // Copy the data from the ScriptableObject
        if (AbilitySO.GetType().Equals(typeof(EnigmaticSO)))
        {
            BonusAbilityData = (EnigmaticSO)AbilitySO;
            RazorbladeNumbers = BonusAbilityData.RazorbladeNumbers;
            AdditionalScale = BonusAbilityData.AdditionalScale;
            Duration = BonusAbilityData.Duration;
            DamageInterval = BonusAbilityData.DamageInterval;
            Speed = BonusAbilityData.Speed;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        TimeBeforeFire = AbilitiesStat.Stats.Cooldown / 3;
        BaseDamage = AbilitiesStat.Stats.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f && playerController.EnemyInZone.Count > 0) {
            FireRazorblade();
            TimeBeforeFire = AbilitiesStat.Stats.Cooldown + Duration;
        }
        else if (TimeBeforeFire > 0) {
            TimeBeforeFire -= Time.deltaTime;
        }
    }

    private void FireRazorblade() {

        for (int i = 0; i < RazorbladeNumbers; i++)
        {
            GameObject ClonedSaw;
            if (!TitanBlade && !HotBlade)
                ClonedSaw = GetPooledObject(0);
            else if (TitanBlade && !HotBlade)
                ClonedSaw = GetPooledObject(i+1);
            else if (!TitanBlade && HotBlade)
                ClonedSaw = GetPooledObject(3);
        }
    }

    private GameObject GetPooledObject(int prefabindex) {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                if (ObjectsScriptList[i].Index == prefabindex)
                {
                    ObjectsList[i].SetActive(true);
                    ObjectsList[i].transform.position = transform.position;
                    ObjectsScriptList[i].UpdateStat(
                        new int[] { AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value, (HotBlade && MaxHotBlade) ? 1 : 0 },
                        new float[] { AbilitiesStat.Stats.Knockback, Duration, DamageInterval, AdditionalScale, Speed, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                    ObjectsScriptList[i].StartUp();
                    return ObjectsList[i];
                }     
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], MainChar.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] {AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value, (HotBlade && MaxHotBlade) ? 1 : 0 }, 
            new float[] {AbilitiesStat.Stats.Knockback, Duration, DamageInterval, AdditionalScale, Speed, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;
        ObjectNew.GetComponent<ProjectileController>().Index = prefabindex;
        ObjectNew.GetComponent<ProjectileController>().StartUp();

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        if (ObjectNew.GetComponent<ProjectileController>() is RazorbladeController derivedInstance)
        {
            derivedInstance.GetPooledTargets(playerController.EnemyInZone);
        }
        return ObjectNew;
    }

    protected override void IncreaseStats(int upgradelevel)
    {
        int index = upgradelevel - 1;
        if (AbilitiesStat.EliteID != 0)
            index -= 5;
        if (index < 0) return;
        /*EnigmaticSO.EnhanceUpgrade[] UpgradeTable = BonusAbilityData.NormalUpgrade;
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
        if (SpecialStats.RazorbladeNumbers != 0)
            RazorbladeNumbers += SpecialStats.RazorbladeNumbers;
        if (SpecialStats.AdditionalScale != 0)
            AdditionalScale += SpecialStats.AdditionalScale;
        if (SpecialStats.Duration != 0)
            Duration += SpecialStats.Duration;
        if (SpecialStats.DamageInterval != 0)
            DamageInterval += SpecialStats.DamageInterval;
        if (SpecialStats.Speed != 0)
           Speed += SpecialStats.Speed;*/

    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        switch (WeaponUpgradeLevel) {
            case 1:
                BaseDamage += 4;
            break;
            case 6:
                if (AbilitiesStat.EliteID == 1)
                    HotBlade = true;
                else if (AbilitiesStat.EliteID == 2)
                {
                    TitanBlade = true;
                    BaseDamage = (BaseDamage * 4) / 2;
                }
                    
                break;
            case 9:
                if (AbilitiesStat.EliteID == 1)
                    MaxHotBlade = true;
                break;
        }
        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Stats.Damage = BaseDamage;
            AbilitiesStat.Stats.Damage += (int)((float)(dmg) * AbilitiesStat.Stats.DamageScaling);
        }
        
    }
}
