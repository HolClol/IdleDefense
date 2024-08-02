using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieryEruptionController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject CrosshairTarget;
    [SerializeField] int AdditionalEruptions = 0;
    [SerializeField] float GroundDuration = 0;

    private GameObject MainEruption;
    private ProjectileController MainEruptionScript;

    private Vector3 DecreaseScale = new Vector3(0.6f, 0.6f, 0f);
    private Vector3 IncreaseScale = new Vector3(0f, 0f, 0f);

    protected override void Start()
    {
        // Initialize the base start
        base.Start();

        TimeBeforeFire = AbilitiesStat.Cooldown/4;
        BaseDamage = AbilitiesStat.Damage;
        DamageScaling += 0.4f;
        CheckUpgrade(WeaponUpgradeLevel);

        MainEruption = Instantiate(AbilitiesStat.ObjectsPrefab[0], CrosshairTarget.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);
        MainEruptionScript = MainEruption.GetComponent<ProjectileController>();
        MainEruption.SetActive(false);
    }

    void FixedUpdate() {
        if (TimeBeforeFire <= 0f) {
            FireEruption();
            TimeBeforeFire = AbilitiesStat.Cooldown;
        }
        else {
            TimeBeforeFire -= Time.deltaTime;
        }
    }

    private void FireEruption() {
        MainEruption.transform.position = CrosshairTarget.transform.position;
        MainEruption.SetActive(true);
        MainEruptionScript.StartUp();
        MainEruptionScript.UpdateStat(new int[] {AbilitiesStat.Damage}, new float[] {AbilitiesStat.Knockback, 2f, GroundDuration});
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
                ObjectsScriptList[i].StartUp();
                ObjectsScriptList[i].UpdateStat(new int[] {AbilitiesStat.Damage/2}, new float[] {AbilitiesStat.Knockback, 2f, GroundDuration});
                return ObjectsList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[prefabindex], MainEruption.transform.position + new Vector3(Random.Range(-3,3), Random.Range(-3,3), 0), Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.transform.localScale = ObjectNew.transform.localScale - DecreaseScale;
        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(new int[] {AbilitiesStat.Damage/2}, new float[] {AbilitiesStat.Knockback, 2f, GroundDuration});
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        return ObjectNew;
    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;

        switch (WeaponUpgradeLevel) {
            case 1:
                DamageScaling += 0.2f;
            break;
            case 2:
                IncreaseScale = IncreaseScale + new Vector3(1f, 1f, 1f);
                MainEruption.transform.localScale = MainEruption.transform.localScale + IncreaseScale;
                for (int i = 0; i < AdditionalEruptions; i++) {
                    GameObject MiniEruption = GetPooledObject(0);
                    MiniEruption.transform.localScale = MainEruption.transform.localScale - DecreaseScale;
                }
            break;
            case 3:
                AdditionalEruptions += 1;
            break;
            case 4:
                AbilitiesStat.Cooldown -= 2f;
                for (int i = 0; i < AdditionalEruptions; i++) {
                    GameObject MiniEruption = GetPooledObject(0);
                    MiniEruption.transform.localScale = MiniEruption.transform.localScale + IncreaseScale;
                }
            break;
            case 5:
                AdditionalEruptions += 1;
                GroundDuration = 2f;
            break;
            case 6:
                AdditionalEruptions = 4;
                GroundDuration = 5f;
                //Burning ground buff
            break;
        }

        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + dmg;
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * DamageScaling);
        }
        
    }
}
