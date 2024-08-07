using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIDmgSection : MonoBehaviour
{
    public TMP_Text AbilityName;
    public TMP_Text AbilityDamage;
    public TMP_Text AbilityLVL;
    public Image DamageBar;
    public int ID;

    private string abilityName;

    public void UpdateName(string name, int id)
    {
        AbilityName.text = name;
        ID = id;
    }
    public void UpdateDisplay(int dmg, int totaldmg, int level)
    {
        if (level > 0)
        {
            AbilityLVL.text = "LVL: " + level.ToString();
        }
            
        float dmgpercent = (float)dmg / (float)totaldmg;
        DamageBar.fillAmount = dmgpercent;
        AbilityDamage.text = dmg.ToString();
    }
}
