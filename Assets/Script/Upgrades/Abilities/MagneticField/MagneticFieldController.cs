using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MagneticFieldController : AbilitiesController
{
    [SerializeField] int NumbOfOrbs = 4;
    [SerializeField] int NumbOfMini = 3;
    [SerializeField] int Piercing = 0;
    [SerializeField] float Duration = 6f;
    [SerializeField] float OrbMoveSpeed;
    [SerializeField] float OrbRecover = 2f;
    [SerializeField] float SizeBuff = 1f;

    private MagneticSO BonusAbilityData;
    private int TotalOrbs;
    private int TotalSpin = 0;
    private float angle;
    private float dist = 2f;
    private bool active, MaxLevel, LastOrb, Overload, Protection;
    
    // Start is called before the first frame update
    protected override void Start() 
    {
        // Initialize the base start
        base.Start();

        // Copy the data from the ScriptableObject
        if (AbilitySO.GetType().Equals(typeof(MagneticSO)))
        {
            BonusAbilityData = (MagneticSO)AbilitySO;
            NumbOfOrbs = BonusAbilityData.NumbOfOrbs;
            NumbOfMini = BonusAbilityData.NumbOfMini;
            Piercing = BonusAbilityData.Piercing;
            Duration = BonusAbilityData.Duration;
            OrbMoveSpeed = BonusAbilityData.OrbMoveSpeed;
            OrbRecover = BonusAbilityData.OrbRecover;
            SizeBuff = BonusAbilityData.SizeBuff;
            TotalOrbs = NumbOfOrbs;
        }
        else
        {
            Debug.LogError("Ability data not assigned!");
        }

        TimeBeforeFire = AbilitiesStat.Stats.Cooldown /2;
        BaseDamage = AbilitiesStat.Stats.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
    }

    void FixedUpdate()
    {
        if (TimeBeforeFire <= 0f && !LastOrb) {
            active = false;
            FireOrb(active);
            TimeBeforeFire = AbilitiesStat.Stats.Cooldown + Duration;
        }
        else {
            TimeBeforeFire -= Time.deltaTime;
            if (ObjectsList.Count > 0) {
                FireOrb(active);
            }
        }
    }

    private void FireOrb(bool activate) {
        if (!activate) {
            for (int i = 0; i < TotalOrbs; i++) {
                int index = 0;
                if (Protection && i > TotalOrbs - NumbOfMini - 1)
                    index = 1;
                GameObject ClonedOrb = GetPooledObject(index);
            }
            active = true;
            TotalSpin = 0;
            
        }
        else if (activate) {
            for (int i = 0; i < ObjectsList.Count; i++) {  
                if (Protection && i > TotalOrbs - NumbOfMini - 1)
                    RotateToTarget(i, NumbOfMini, -1);
                else 
                    RotateToTarget(i, NumbOfOrbs, 1);

                if (Overload && TotalSpin >= 3)
                {
                    TotalSpin = 0;
                    var script = ObjectsScriptList[i] as MagneticOrb;
                    if (script != null)
                        script.Explode();
                }
            }
        }
    }

    private void RotateToTarget(int order, int total, int abs) 
    {
        if (!ObjectsList[order].activeInHierarchy) return;

        int orbindex = order;
        float space = dist;
        float radians;
        if (abs == -1)
        {
            angle += OrbMoveSpeed * 0.25f;
            orbindex -= NumbOfOrbs;
            space -= 1f;
            radians = -angle * Mathf.Deg2Rad;  
        }
        else
        {
            angle += OrbMoveSpeed * 0.5f;
            radians = angle * Mathf.Deg2Rad; 
        }

        // Calculate the new position
        float offsetAngle = (orbindex * 360f / total) * Mathf.Deg2Rad; // Offset for each object
        float Target_x = MainChar.transform.position.x + space * Mathf.Cos(radians + offsetAngle);
        float Target_y = MainChar.transform.position.y + space * Mathf.Sin(radians + offsetAngle);

        ObjectsList[order].transform.position = new Vector3(Target_x, Target_y, 0); // Update the position

        if (angle % 180 == 0 && TotalSpin < 3 && Overload)
            TotalSpin += 1;
    }

    private GameObject GetPooledObject(int index) {
        if (MaxLevel) {
            LastOrb = true;
        }
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                int Main = ObjectsScriptList[i].MainProjectile ? 0 : 1;
                if (Main == index)
                {
                    ObjectsList[i].SetActive(true);
                    ObjectsScriptList[i].UpdateStat(
                        new int[] { AbilitiesStat.Stats.Damage, Piercing, MaxLevel ? 1 : 0, AbilitiesStat.Stats.DamageType.Value },
                        new float[] { AbilitiesStat.Stats.Knockback, Duration, OrbRecover, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                    ObjectsScriptList[i].StartUp();
                    return ObjectsList[i];
                }
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[index], MainChar.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.transform.localScale = ObjectNew.transform.localScale * SizeBuff;
        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] {AbilitiesStat.Stats.Damage, Piercing, MaxLevel ? 1 : 0, AbilitiesStat.Stats.DamageType.Value }, 
            new float[] {AbilitiesStat.Stats.Knockback, Duration, OrbRecover, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        return ObjectNew;
    }

    protected override void IncreaseStats(int upgradelevel)
    {
        int index = upgradelevel - 1;
        if (index < 0) return;
        MagneticSO.EnhanceUpgrade[] UpgradeTable = BonusAbilityData.NormalUpgrade;
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
            AbilitiesStat.Stats.Cooldown -= AbilitiesStat.Stats.Cooldown*(BaseStats.Cooldown / 100);
        if (BaseStats.Knockback != 0)
            AbilitiesStat.Stats.Knockback += BaseStats.Knockback;
        if (BaseStats.DamageScaling != 0)
            AbilitiesStat.Stats.DamageScaling += BaseStats.DamageScaling;
        if (BaseStats.CritRate != 0)
            AbilitiesStat.Stats.CritRate += BaseStats.CritRate;
        if (BaseStats.CritDamage != 0)
            AbilitiesStat.Stats.CritDamage += BaseStats.CritDamage;

        // Check special stats
        if (SpecialStats.NumbOfOrbs != 0) 
            NumbOfOrbs += SpecialStats.NumbOfOrbs;
        if (SpecialStats.NumbOfMini != 0)
            NumbOfMini += SpecialStats.NumbOfMini;
        if (SpecialStats.Piercing != 0)
            Piercing += SpecialStats.Piercing;
        if (SpecialStats.Duration != 0)
            Duration += SpecialStats.Duration;
        if (SpecialStats.OrbMoveSpeed != 0)
            OrbMoveSpeed += SpecialStats.OrbMoveSpeed;
        if (SpecialStats.OrbRecover != 0)
            OrbRecover += SpecialStats.OrbRecover;
        if (SpecialStats.SizeBuff != 0)
            SizeBuff += SpecialStats.SizeBuff;

    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
        IncreaseStats(WeaponUpgradeLevel);
        switch (WeaponUpgradeLevel) {
            case 2:
                for (int i = 0; i < NumbOfOrbs; i++) {
                    if (ObjectsList.Count > 0)
                    {
                        GameObject ClonedOrb = ObjectsList[i];
                        ClonedOrb.transform.localScale = ClonedOrb.transform.localScale * SizeBuff;
                    }       
                }
            break;
            case 5:
                MaxLevel = true;
            break;
            case 6:
                if (AbilitiesStat.EliteID == 1)
                    Overload = true;
                else if (AbilitiesStat.EliteID == 2)
                {
                    for (int i = 0; i < NumbOfMini; i++)
                    {
                        if (ObjectsList.Count > 0)
                        {
                            GameObject ClonedOrb = GetPooledObject(1);
                        }
                    }
                    Protection = true;
                    TotalOrbs = NumbOfOrbs + NumbOfMini;
                    dist += 0.5f;
                }
                break;
            case 7:
                if (AbilitiesStat.EliteID == 1)
                {
                    for (int i = 0; i < NumbOfOrbs; i++)
                        if (ObjectsList.Count > 0)
                        {
                            GameObject ClonedOrb = ObjectsList[i];
                            ClonedOrb.transform.localScale = ClonedOrb.transform.localScale * SizeBuff;
                        }
                }
                break;
            case 10:
                int index = 0;
                int spawnorb = 0;
                if (AbilitiesStat.EliteID == 1)
                    spawnorb = 1;         
                else if (AbilitiesStat.EliteID == 2)
                {
                    spawnorb = 3;
                    index = 1;
                }
                for (int i = 0; i < spawnorb; i++)
                {
                    GameObject ClonedOrb = GetPooledObject(index);
                }
                TotalOrbs += spawnorb;
                break;
        }
        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }


    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Stats.Damage = BaseDamage;
            AbilitiesStat.Stats.Damage += (int)((float)(dmg) * AbilitiesStat.Stats.DamageScaling);
            for (int i = 0; i < ObjectsList.Count; i++) {
                if (ObjectsList[i].activeInHierarchy) {
                    ObjectsScriptList[i].UpdateStat(
                        new int[] {AbilitiesStat.Stats.Damage, Piercing, MaxLevel ? 1 : 0, AbilitiesStat.Stats.DamageType.Value }, 
                        new float[] {AbilitiesStat.Stats.Knockback, Duration, OrbRecover, AbilitiesStat.Stats.CritRate, AbilitiesStat.Stats.CritDamage });
                }
            }
        }
        
    }
}
