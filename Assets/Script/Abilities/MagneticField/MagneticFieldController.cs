using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticFieldController : AbilitiesController
{
    [Header("Weapon Set Up")]
    [SerializeField] int NumbOfOrbs = 4;
    [SerializeField] int Piercing = 0;
    [SerializeField] float Duration = 5f;
    [SerializeField] float OrbMoveSpeed;
    [SerializeField] float OrbRecover = 2f;
    
    private float angle;
    private bool active = false;
    private bool MaxLevel = false;
    private bool LastOrb = false;
    private float SizeBuff = 1f;
    // Start is called before the first frame update
    protected override void Start() 
    {
        // Initialize the base start
        base.Start();

        TimeBeforeFire = AbilitiesStat.Cooldown/2;
        BaseDamage = AbilitiesStat.Damage;
        CheckUpgrade(WeaponUpgradeLevel);
        DamageScaling = 0.5f;
    }

    void FixedUpdate()
    {
        if (TimeBeforeFire <= 0f && !LastOrb) {
            active = false;
            FireOrb(active);
            TimeBeforeFire = AbilitiesStat.Cooldown;
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
            for (int i = 0; i < NumbOfOrbs; i++) {
                GameObject ClonedOrb = GetPooledObject();
            }
            active = true;
            
        }
        else if (activate) {
            for (int i = 0; i < ObjectsList.Count; i++) {
                RotateToTarget(i, ObjectsList.Count);
            }
        }
    }

    private void RotateToTarget(int order, int total) 
    {
        angle += OrbMoveSpeed * 0.5f; // Increment the angle
        float radians = angle * Mathf.Deg2Rad; // Convert degrees to radians

        // Calculate the new position
        float offsetAngle = (order * 360f / total) * Mathf.Deg2Rad; // Offset for each object
        float Target_x = MainChar.transform.position.x + 2f * Mathf.Cos(radians + offsetAngle);
        float Target_y = MainChar.transform.position.y + 2f * Mathf.Sin(radians + offsetAngle);

        ObjectsList[order].transform.position = new Vector3(Target_x, Target_y, 0); // Update the position
    }

    private GameObject GetPooledObject() {
        if (MaxLevel) {
            LastOrb = true;
        }
        for (int i = 0; i < ObjectsList.Count; i++) {
            if (!ObjectsList[i].activeInHierarchy) {
                ObjectsList[i].SetActive(true);
                ObjectsScriptList[i].UpdateStat(new int[] {AbilitiesStat.Damage, Piercing, (MaxLevel ? 1 : 0)}, new float[] {AbilitiesStat.Knockback, Duration, OrbRecover});
                ObjectsScriptList[i].StartUp();
                return ObjectsList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject ObjectNew = Instantiate(AbilitiesStat.ObjectsPrefab[0], MainChar.transform.position, Quaternion.identity, GameObject.Find("_Projectiles").transform);

        ObjectNew.transform.localScale = ObjectNew.transform.localScale * SizeBuff;
        ObjectNew.SetActive(true);
        ObjectNew.GetComponent<ProjectileController>().UpdateStat(new int[] {AbilitiesStat.Damage, Piercing, (MaxLevel ? 1 : 0)}, new float[] {AbilitiesStat.Knockback, Duration, OrbRecover});
        ObjectNew.GetComponent<ProjectileController>().MainScript = this;

        ObjectsList.Add(ObjectNew);
        ObjectsScriptList.Add(ObjectNew.GetComponent<ProjectileController>());
        return ObjectNew;
    }

    public override void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;

        switch (WeaponUpgradeLevel) {
            case 1:
                Piercing += 1;
                Duration = 6f;
                AbilitiesStat.Cooldown = Duration+5f;
            break;
            case 2:
                SizeBuff += 0.25f;
                Duration = 8f;
                AbilitiesStat.Cooldown = Duration+5f;
                for (int i = 0; i < NumbOfOrbs; i++) {
                    GameObject ClonedOrb = ObjectsList[i];
                    ClonedOrb.transform.localScale = ClonedOrb.transform.localScale * SizeBuff;
                }
            break;
            case 3:
                AbilitiesStat.Knockback += AbilitiesStat.Knockback;
                DamageScaling += 0.25f;
            break;
            case 4:
                Piercing += 2;
                NumbOfOrbs = 6;
                OrbMoveSpeed -= 1.5f;
            break;
            case 5:
                MaxLevel = true;
            break;
        }

        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }


    public override void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + (dmg / 2);
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * DamageScaling);
            for (int i = 0; i < ObjectsList.Count; i++) {
                if (ObjectsList[i].activeInHierarchy) {
                    ObjectsScriptList[i].UpdateStat(new int[] {AbilitiesStat.Damage, Piercing, (MaxLevel ? 1 : 0)}, new float[] {AbilitiesStat.Knockback, Duration, OrbRecover});
                }
            }
        }
        
    }
}
