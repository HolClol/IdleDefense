using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissilesController : AbilitiesController
{
    [SerializeField] int MissileNumbers = 3;
    [SerializeField] float InternalExplode = 1f;

    private Vector3 AdditionalScale = new Vector3(0f, 0f, 0f);
    private MissilesSO BonusAbilityData;
    private bool RocketBarrage, AtomicNuke;
    private int RocketID = 0;
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
        else if (TimeBeforeFire > 0)
        {
            TimeBeforeFire -= Time.deltaTime;
        }
    }

    private IEnumerator FireMissiles() {
        playerController.EnemyInZone.RemoveAll(GameObject => GameObject == null);
        for (int i = 0; i < MissileNumbers; i++) {
            if (playerController.EnemyInZone.Count > 0)
            {
                Transform SpawnLocation = playerController.EnemyInZone[Random.Range(0, playerController.EnemyInZone.Count - 1)].transform;
                GameObject ClonedBullet = GetPooledObject(RocketID);
                ClonedBullet.transform.position = SpawnLocation.position;
                yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            }
            else
            {
                break;
            }
            
        }
        
    }

    private GameObject GetPooledObject(int prefabindex) {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                if (ObjectsScriptList[i].Index == prefabindex)
                {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(
                        new int[] { AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value },
                        new float[] { InternalExplode, AbilitiesStat.Stats.Knockback, AdditionalScale.x, AdditionalScale.y, AdditionalScale.z, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                    return ObjectsList[i];
                }
                
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], MainChar.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] {AbilitiesStat.Stats.Damage, AbilitiesStat.Stats.DamageType.Value }, 
            new float[] {InternalExplode, AbilitiesStat.Stats.Knockback, AdditionalScale.x, AdditionalScale.y, AdditionalScale.z, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;
        ObjectNew.GetComponent<ProjectileController>().Index = prefabindex;

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
                case StatVariables.MissileNumbers:
                    MissileNumbers += (int)stat.Value;
                    break;
                case StatVariables.InternalExplode:
                    InternalExplode += stat.Value;
                    break;
                case StatVariables.Scale:
                    AdditionalScale += new Vector3(stat.Value, stat.Value, 0f);
                    break;
            }
        }

    }

    public override void TargetStruckSignal(GameObject[] TaggedObject)
    {
        // Spawn seperate explosion for the missiles
        GameObject Explode = null;
        if (RocketBarrage)
            Explode = GetPooledObject(2);
        else if (AtomicNuke)
            Explode = GetPooledObject(4);

        if (Explode != null)
        {
            Explode.transform.position = TaggedObject[0].transform.position;
        }
            
    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        switch (WeaponUpgradeLevel) {
            case 6:
                //Clear everything to bring in the new projetiles (Yes this means the currently old fired ones will dissappear)
                ObjectsList.Clear();
                ObjectsScriptList.Clear();
                if (AbilitiesStat.EliteID == 1)
                {
                    RocketBarrage = true;
                    RocketID = 1;
                }
                else if (AbilitiesStat.EliteID == 2)
                {
                    AtomicNuke = true;
                    RocketID = 3;
                }
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
