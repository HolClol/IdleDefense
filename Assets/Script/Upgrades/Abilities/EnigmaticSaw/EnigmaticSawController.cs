using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaticSawController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] int RazorbladeNumbers = 1;

    private float AdditionalScale = 0f;
    private float Duration = 5f;
    private float DamageInterval = 0.6f;
    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        TimeBeforeFire = AbilitiesStat.Cooldown / 3;
        BaseDamage = AbilitiesStat.Damage;
        DamageScaling -= 0.6f;
        CheckUpgrade(WeaponUpgradeLevel);
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f && playerController.EnemyInZone.Count > 0) {
            FireRazorblade();
            TimeBeforeFire = AbilitiesStat.Cooldown;
        }
        else {
            TimeBeforeFire -= Time.deltaTime;
        }
    }

    private void FireRazorblade() {
        for (int i = 0; i < RazorbladeNumbers; i++) {
            GameObject ClonedBullet = GetPooledObject();
        }
        
    }

    private GameObject GetPooledObject() {
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                ObjectsList[i].SetActive(true);
                ObjectsList[i].transform.position = transform.position;
                ObjectsScriptList[i].UpdateStat(
                    new int[] {AbilitiesStat.Damage, AbilitiesStat.DamageType.Value }, 
                    new float[] { AbilitiesStat.Knockback, Duration, DamageInterval, AdditionalScale, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
                ObjectsScriptList[i].StartUp();
                return ObjectsList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[0], MainChar.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(
            new int[] {AbilitiesStat.Damage, AbilitiesStat.DamageType.Value }, 
            new float[] {AbilitiesStat.Knockback, Duration, DamageInterval, AdditionalScale, AbilitiesStat.CritRate, AbilitiesStat.CritDamage });
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        if (ObjectNew.GetComponent<ProjectileController>() is RazorbladeController derivedInstance)
        {
            derivedInstance.GetPooledTargets(playerController.EnemyInZone);
        }
        return ObjectNew;
    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;

        switch (WeaponUpgradeLevel) {
            case 1:
                BaseDamage += 6;
            break;
            case 2:
                Duration += 2f;
                AbilitiesStat.Cooldown += 2f;
            break;
            case 3:
                DamageInterval = 0.4f;
            break;
            case 4:
                RazorbladeNumbers = 2;
                AdditionalScale += 0.5f;
            break;
            case 5:
                RazorbladeNumbers = 3;
                DamageScaling += 0.2f;
            break;
        }
        BaseDamage += 2;
        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + dmg;
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * DamageScaling);
        }
        
    }
}
