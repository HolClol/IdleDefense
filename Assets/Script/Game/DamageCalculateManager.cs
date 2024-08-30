using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageDisplay
{

}

public class DamageCalculateManager : MonoBehaviour
{
    [System.Serializable] public class AbilitiesStat
    {
        public int UpgradeLevel;
        public int Damage;

        public AbilitiesStat(int level, int damage)
        {
            UpgradeLevel = level;
            Damage = damage;
        }
    }

    public GameUpgradeInfo UpgradesInfo;
    public GameObject _damageDisplayPrefab;

    private Dictionary<int, AbilitiesStat> AbilitiesDict = new Dictionary<int, AbilitiesStat>();
    private float UpdateCooldown = 0f;
    private int TotalDamage = 0;

    private List<GameObject> PooledDisplay = new List<GameObject>();
    private List<DamageDisplay> PooledDisplayScript = new List<DamageDisplay>();

    private void Start()
    {
        UpgradesInfo.Reset();
    }

    private void Update()
    {
        if (UpdateCooldown <= 0f)
        {
            foreach (var upgrade in UpgradesInfo.AbilitiesStat)
            {
                if (AbilitiesDict.ContainsKey(upgrade.ID)) {
                    upgrade.Damage = AbilitiesDict[upgrade.ID].Damage;
                    upgrade.UpgradeLevel = AbilitiesDict[upgrade.ID].UpgradeLevel;
                }
                
            }
            UpgradesInfo.TotalDamage = TotalDamage;
            UpdateCooldown = 5f;
        }
        else { UpdateCooldown -= Time.deltaTime; }
    }

    // Generally the array consist of follow: int[] {dmg, id, damagetype}; float[] {knockback, debouncetime}
    public void DamageCalculate(GameObject enemy, int[] intstat, float[] floatstat) {
        if (!enemy.GetComponent<EnemyMain>().GetDebounce(intstat, floatstat))
        {
            int Damage = enemy.GetComponent<EnemyMain>().DamageDealt;
            if (intstat[0] > 0)
            {
                GameObject damageDisplay = GetPooledObject(enemy.transform.position, Damage);
                damageDisplay.transform.position = enemy.transform.position + new Vector3(0, 1, 0);
            }

            if (AbilitiesDict.ContainsKey(intstat[1]))
            {
                AbilitiesDict[intstat[1]].Damage += intstat[0];
            }   
            else
            {
                AbilitiesDict[intstat[1]] = new AbilitiesStat(1, intstat[0]);
                UpgradesInfo.AbilitiesStat.Add(new GameUpgradeInfo.AbilitiesInfo(intstat[1], 1, intstat[0]));
            }
                

            TotalDamage += intstat[0];
        }  
    }

    public void ReceiveUpgrade(int[] stat)
    {
        if (stat[0] == 1)
            if (AbilitiesDict.ContainsKey(stat[1]))
                AbilitiesDict[stat[1]].UpgradeLevel += 1;
    }

    private GameObject GetPooledObject(Vector3 pos, int damage)
    {
        for (int i = 0; i < PooledDisplay.Count; i++)
        {
            if (!PooledDisplay[i].activeInHierarchy)
            {
                PooledDisplay[i].SetActive(true);
                PooledDisplayScript[i].UpdateDisplay(damage);
                return PooledDisplay[i];
            }
        }

        // Optionally expand pool if needed
        GameObject DisplayNew = Instantiate(_damageDisplayPrefab, pos, Quaternion.identity, GameObject.Find("_Effects").transform);

        DisplayNew.SetActive(true);
        DisplayNew.GetComponent<DamageDisplay>().UpdateDisplay(damage);

        PooledDisplay.Add(DisplayNew);
        PooledDisplayScript.Add(DisplayNew.GetComponent<DamageDisplay>());
        return DisplayNew;
    }
}
