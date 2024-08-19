using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class UIDamageProgress : MonoBehaviour
{
    public GameObject AbilityPanelPrefab;
    public UpgradesScriptableObject UpgradeScriptableObject;
    public GameUpgradeInfo GameUpgradeInfo;

    public GameObject ContentPanel;
    public TMP_Text DamageText;

    [SerializeField] private List<GameObject> AbilityPanels = new List<GameObject>();
    private List<UIDmgSection> AbilityPanelsScript = new List<UIDmgSection>();
    private int Length = 0;

    public void UpdateDisplay()
    {
        if (Length < GameUpgradeInfo.AbilitiesStat.Count)
        {
            int loop = GameUpgradeInfo.AbilitiesStat.Count - Length;
            for (int i = 0; i < loop; i++)
            {
                GameObject Clone = Instantiate(AbilityPanelPrefab, ContentPanel.transform);
                AbilityPanels.Add(Clone);
                int NewCloneID = GameUpgradeInfo.AbilitiesStat[GameUpgradeInfo.AbilitiesStat.Count - 1 - i].ID;

                for (int c = 0; c < UpgradeScriptableObject.UpgradeInfoTable.Count; c++)
                {
                    if (UpgradeScriptableObject.UpgradeInfoTable[c].Upgrade.ID == NewCloneID) 
                    {
                        Clone.GetComponent<UIDmgSection>().UpdateName(UpgradeScriptableObject.UpgradeInfoTable[c].Upgrade.UpgradeName, NewCloneID);
                        AbilityPanelsScript.Add(Clone.GetComponent<UIDmgSection>());
                        break; 
                    }
                }
                
            }
            Length = GameUpgradeInfo.AbilitiesStat.Count;
            
        }
        DamageText.text = "Total Damage: " + GameUpgradeInfo.TotalDamage.ToString();
        int index = 0;
        foreach (var ability in GameUpgradeInfo.AbilitiesStat.OrderByDescending(ab => ab.Damage))
        {
            for (int i = 0; i < AbilityPanels.Count; i++)
            {
                if (AbilityPanelsScript[i].ID == ability.ID)
                {
                    AbilityPanels[i].transform.SetSiblingIndex(index);
                    AbilityPanelsScript[i].UpdateDisplay(ability.Damage, GameUpgradeInfo.TotalDamage, ability.UpgradeLevel);
                }
                    
                
            }
            index += 1;
        }

    }

}
