using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissilesController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] int MissileNumbers = 3;
    [SerializeField] float InternalExplode = 1f;

    private Vector3 AdditionalScale = new Vector3(0f, 0f, 0f);
    // Start is called before the first frame update
    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        TimeBeforeFire = AbilitiesStat.Cooldown;
        BaseDamage = AbilitiesStat.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
        DamageScaling = -0.6f;
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f && playerController.EnemyInZone.Count > 0) {
            StartCoroutine(FireMissiles());
            TimeBeforeFire = AbilitiesStat.Cooldown;
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
                yield return new WaitForSeconds(0.2f);
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
                ObjectsScriptList[i].UpdateStat(new int[] {AbilitiesStat.Damage}, new float[] {InternalExplode, AbilitiesStat.Knockback, AdditionalScale.x, AdditionalScale.y, AdditionalScale.z});
                ObjectsScriptList[i].StartUp();
                return ObjectsList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[0], MainChar.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(new int[] {AbilitiesStat.Damage}, new float[] {InternalExplode, AbilitiesStat.Knockback, AdditionalScale.x, AdditionalScale.y, AdditionalScale.z});
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        return ObjectNew;
    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;

        switch (WeaponUpgradeLevel) {
            case 1:
                AbilitiesStat.Cooldown -= 1f;
            break;
            case 2:
                DamageScaling += 0.2f;
            break;
            case 3:
                InternalExplode -= 0.15f;
            break;
            case 4:
                MissileNumbers += 2;
            break;
            case 5:
                DamageScaling += 0.2f;
                AdditionalScale += new Vector3(2, 2, 2);
            break;
            case 6:
                AbilitiesStat.Cooldown -= 1.5f;
                MissileNumbers += 3;
            break;
        }
        BaseDamage += 5;
        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + dmg;
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * DamageScaling);
        }
        
    }

}
