using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieryEruptionController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject CrosshairTarget;

    private GameObject MainEruption;
    private ProjectileController MainEruptionScript;
    private EruptionSO BonusAbilityData;

    [SerializeField] int AdditionalEruptions = 0;
    [SerializeField] float GroundDuration = 0;

    private Vector3 DecreaseScale = new Vector3(0.6f, 0.6f, 0f);
    private Vector3 IncreaseScale = new Vector3(0f, 0f, 0f);

    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        // Copy the data from the ScriptableObject
        if (AbilitySO.GetType().Equals(typeof(EruptionSO)))
        {
            BonusAbilityData = (EruptionSO)AbilitySO;
            AdditionalEruptions = BonusAbilityData.AdditionalEruptions;
            GroundDuration = BonusAbilityData.GroundDuration;
            DecreaseScale = BonusAbilityData.DecreaseScale;
            IncreaseScale = BonusAbilityData.IncreaseScale;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        TimeBeforeFire = AbilitiesStat.Stats.Cooldown /4;
        BaseDamage = AbilitiesStat.Stats.Damage;
        CheckUpgrade(WeaponUpgradeLevel);

        MainEruption = Instantiate(AbilitiesStat.ObjectsPrefab[0], CrosshairTarget.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);
        MainEruptionScript = MainEruption.GetComponent<ProjectileController>();
        MainEruption.SetActive(false);
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f) {
            FireEruption();
            TimeBeforeFire = AbilitiesStat.Stats.Cooldown;
        }
        else if (TimeBeforeFire > 0)
        {
            TimeBeforeFire -= Time.deltaTime;
        }
    }

    private void FireEruption() {
        MainEruption.transform.position = CrosshairTarget.transform.position;
        MainEruption.SetActive(true);
        MainEruptionScript.StartUp();
        MainEruptionScript.UpdateStat(new int[] {AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value }, new float[] { AbilitiesStat.Stats.Knockback, 2f, GroundDuration, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        StartCoroutine(FireMiniEruption());
    }

    private IEnumerator FireMiniEruption() {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < AdditionalEruptions; i++) {
            GameObject MiniEruption = GetPooledObject(0);
        }
        
    }

    private GameObject GetPooledObject(int prefabindex) {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                ObjectsList[i].transform.position = MainEruption.transform.position + new Vector3(Random.Range(-3,3), Random.Range(-3,3), 0);
                ObjectsList[i].SetActive(true);
                ObjectsScriptList[i].UpdateStat(new int[] { AbilitiesStat.Stats.Damage / 2, AbilitiesStat.Stats.DamageType.Value }, new float[] { AbilitiesStat.Stats.Knockback, 2f, GroundDuration, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                ObjectsScriptList[i].StartUp();       
                return ObjectsList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], MainEruption.transform.position + new Vector3(Random.Range(-3,3), Random.Range(-3,3), 0), Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.transform.localScale = ObjectNew.transform.localScale - DecreaseScale;
        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(new int[] {AbilitiesStat.Stats.Damage /2, AbilitiesStat.Stats.DamageType.Value }, new float[] {AbilitiesStat.Stats.Knockback, 2f, GroundDuration, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;
        ObjectNew.GetComponent<ProjectileController>().Index = prefabindex;
        ObjectNew.GetComponent<ProjectileController>().StartUp();

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        return ObjectNew;
    }

    protected override void IncreaseStats(int upgradelevel)
    {
        int index = upgradelevel - 1;
        if (AbilitiesStat.EliteID != 0)
            index -= 5;
        if (index < 0) return;
        EruptionSO.EnhanceUpgrade[] UpgradeTable = BonusAbilityData.NormalUpgrade;
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
        if (SpecialStats.AdditionalEruptions != 0)
            AdditionalEruptions += SpecialStats.AdditionalEruptions;
        if (SpecialStats.GroundDuration != 0)
            GroundDuration += SpecialStats.GroundDuration;
        if (!SpecialStats.DecreaseScale.Equals(new Vector3(0, 0, 0)))
            DecreaseScale += SpecialStats.DecreaseScale;
        if (!SpecialStats.IncreaseScale.Equals(new Vector3(0, 0, 0)))
            IncreaseScale += SpecialStats.IncreaseScale;

    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        switch (WeaponUpgradeLevel) {
            case 1:
                /*AbilitiesStat.Stats.DamageScaling += 0.2f;
                IncreaseScale = IncreaseScale + new Vector3(1f, 1f, 1f);*/
                MainEruption.transform.localScale = MainEruption.transform.localScale + IncreaseScale;
                for (int i = 0; i < AdditionalEruptions; i++)
                {
                    GameObject MiniEruption = GetPooledObject(0);
                    MiniEruption.transform.localScale = MainEruption.transform.localScale - DecreaseScale;
                }
                break;
            /*case 2:
                AdditionalEruptions += 1;
            break;*/
            case 3:
                /*AbilitiesStat.Stats.Cooldown -= 1f;
                AbilitiesStat.Stats.DamageScaling += 0.2f;*/
                for (int i = 0; i < AdditionalEruptions; i++) {
                    GameObject MiniEruption = GetPooledObject(0);
                    MiniEruption.transform.localScale = MiniEruption.transform.localScale + IncreaseScale;
                } 
            break;
            /*case 4:
                AdditionalEruptions += 1;
                GroundDuration = 2f;
            break;*/
            /*case 5:
                AdditionalEruptions = 4;
                GroundDuration = 5f;
                //Burning ground buff
            break;*/
        }
        BaseDamage += 5;
        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Stats.Damage = BaseDamage;
            AbilitiesStat.Stats.Damage += (int)((float)(dmg) * AbilitiesStat.Stats.DamageScaling);
        }
        
    }
}
