using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissilesController : AbilitiesController
{
    [SerializeField] int MissileNumbers = 3;
    [SerializeField] float InternalExplode = 1f;

    private Vector3 AdditionalScale = new Vector3(0f, 0f, 0f);
    private MissilesSO BonusAbilityData;
    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        // Copy the data from the ScriptableObject
        if (AbilitySO.GetType().Equals(typeof(MissilesSO)))
        {
            BonusAbilityData = (MissilesSO)AbilitySO;
            MissileNumbers = BonusAbilityData.MissileNumbers;
            InternalExplode = BonusAbilityData.InternalExplode;
            AdditionalScale = BonusAbilityData.AdditionalScale;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        TimeBeforeFire = AbilitiesStat.Stats.Cooldown;
        BaseDamage = AbilitiesStat.Stats.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f && playerController.EnemyInZone.Count > 0) {
            StartCoroutine(FireMissiles());
            TimeBeforeFire = AbilitiesStat.Stats.Cooldown;
        }
        else {
            TimeBeforeFire -= Time.deltaTime;
        }
    }

    private IEnumerator FireMissiles() {
        playerController.EnemyInZone.RemoveAll(GameObject => GameObject == null);
        for (int i = 0; i < MissileNumbers; i++) {
            if (playerController.EnemyInZone.Count > 0)
            {
                Transform SpawnLocation = playerController.EnemyInZone[Random.Range(0, playerController.EnemyInZone.Count - 1)].transform;
                GameObject ClonedBullet = GetPooledObject();
                ClonedBullet.transform.position = SpawnLocation.position;
                yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            }
            else
            {
                break;
            }
            
        }
        
    }

    private GameObject GetPooledObject() {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                ObjectsList[i].SetActive(true);
                ObjectsScriptList[i].UpdateStat(
                    new int[] {AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value }, 
                    new float[] {InternalExplode, AbilitiesStat.Stats.Knockback, AdditionalScale.x, AdditionalScale.y, AdditionalScale.z, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                ObjectsScriptList[i].StartUp();
                return ObjectsList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[0], MainChar.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] {AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value }, 
            new float[] {InternalExplode, AbilitiesStat.Stats.Knockback, AdditionalScale.x, AdditionalScale.y, AdditionalScale.z, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

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
        MissilesSO.EnhanceUpgrade[] UpgradeTable = BonusAbilityData.NormalUpgrade;
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
        if (SpecialStats.MissileNumbers != 0)
            MissileNumbers += SpecialStats.MissileNumbers;
        if (SpecialStats.InternalExplode != 0)
            InternalExplode += SpecialStats.InternalExplode;
        if (!SpecialStats.AdditionalScale.Equals(new Vector3(0f, 0f, 0f)))
            AdditionalScale += SpecialStats.AdditionalScale;

    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        /*switch (WeaponUpgradeLevel) {
            case 1:
                AbilitiesStat.Stats.Cooldown -= 1f;
                AbilitiesStat.Stats.DamageScaling += 0.2f;
                break;
            case 2:
                InternalExplode -= 0.15f;
            break;
            case 3:
                MissileNumbers += 2;
            break;
            case 4:
                AbilitiesStat.Stats.DamageScaling += 0.2f;
                AdditionalScale += new Vector3(2, 2, 2);
            break;
            case 5:
                AbilitiesStat.Stats.Cooldown -= 1.5f;
                MissileNumbers += 3;
            break;
        }*/
        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Stats.Damage = BaseDamage;
            AbilitiesStat.Stats.Damage += (int)((float)(dmg) * AbilitiesStat.Stats.DamageScaling);
        }
        
    }

}
