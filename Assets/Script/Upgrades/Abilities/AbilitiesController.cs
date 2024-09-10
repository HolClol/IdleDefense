using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    public AbilitiesSO AbilityData;
    public AbilitiesSO.AbilitiesStatClass AbilitiesStat;

    protected float TimeBeforeFire;
    protected float DamageScaling = 0;
    protected int WeaponUpgradeLevel = 0;
    protected int BaseDamage;

    protected GameObject MainChar;    
    protected PlayerController playerController;
    protected List<GameObject> ObjectsList = new List<GameObject>();
    protected List<ProjectileController> ObjectsScriptList = new List<ProjectileController>();

    protected virtual void Start() {
        MainChar = GameObject.Find("Player");
        playerController = MainChar.GetComponent<PlayerController>();
        AbilitiesStat = AbilityData.AbilitiesStat;

    }

    // If the projectile need to send on hit signal back to main script
    public virtual void TargetStruckSignal(GameObject TaggedObject) {

    }

    // Update damage, created cause some weapons scale differently from others
    public virtual void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + dmg;
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * DamageScaling);
        }
        
    }

    // Upgrade all available weapons
    public virtual void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
    }

    // Unlock elite
    public virtual void EliteUnlock(int eliteid)
    {
        AbilitiesStat.EliteID = eliteid;
    }
}
