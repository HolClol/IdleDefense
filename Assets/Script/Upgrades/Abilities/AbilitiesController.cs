using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    [System.Serializable] public class AbilitiesStatClass
    {
        public GameObject[] ObjectsPrefab;
        public int Damage;
        public int ID;
        public int EliteID;
        public float Cooldown;
        public float Knockback;
        public float DamageScaling = 0f;
        [Range(0f, 1f)] public float CritRate = 0.05f;
        public float CritDamage = 1f;
        public IntVariable DamageType;
    }

    public AbilitiesSO AbilitySO;
    [SerializeField] protected AbilitiesStatClass AbilitiesStat;

    protected float TimeBeforeFire;
    protected int WeaponUpgradeLevel = 0;
    protected int BaseDamage;

    protected GameObject MainChar;    
    protected PlayerController playerController;
    protected List<GameObject> ObjectsList = new List<GameObject>();
    protected List<ProjectileController> ObjectsScriptList = new List<ProjectileController>();

    protected virtual void Start() {
        MainChar = GameObject.Find("Player");
        playerController = MainChar.GetComponent<PlayerController>();
        DeepCopyData();
    }

    protected virtual void DeepCopyData()
    {
        // Copy primitive types
        AbilitiesStat.Damage = AbilitySO.AbilityData.Damage;
        AbilitiesStat.ID = AbilitySO.AbilityData.ID;
        AbilitiesStat.EliteID = AbilitySO.AbilityData.EliteID;
        AbilitiesStat.Cooldown = AbilitySO.AbilityData.Cooldown;
        AbilitiesStat.Knockback = AbilitySO.AbilityData.Knockback;
        AbilitiesStat.DamageScaling = AbilitySO.AbilityData.DamageScaling;
        AbilitiesStat.CritRate = AbilitySO.AbilityData.CritRate;
        AbilitiesStat.CritDamage = AbilitySO.AbilityData.CritDamage;
        AbilitiesStat.DamageType = AbilitySO.AbilityData.DamageType;

        // Create a new array and copy its contents (deep copy for array)
        AbilitiesStat.ObjectsPrefab = new GameObject[AbilitySO.AbilityData.ObjectsPrefab.Length];
        for (int i = 0; i < AbilitySO.AbilityData.ObjectsPrefab.Length; i++)
        {
            AbilitiesStat.ObjectsPrefab[i] = AbilitySO.AbilityData.ObjectsPrefab[i];
        }
    }

    // If the projectile need to send on hit signal back to main script
    public virtual void TargetStruckSignal(GameObject TaggedObject) {

    }

    // Update damage, created cause some weapons scale differently from others
    public virtual void UpdateDamage(int dmg) {
        if (this.enabled) {
            AbilitiesStat.Damage = BaseDamage + dmg;
            AbilitiesStat.Damage += (int)((float)(AbilitiesStat.Damage) * AbilitiesStat.DamageScaling);
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
