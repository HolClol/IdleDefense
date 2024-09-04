using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIUpgradeManager : MonoBehaviour
{
    [System.Serializable] public class UpgradePanels
    {
        public TMP_Text UpgradeOptionsName;
        public TMP_Text UpgradeOptionsDesc;
        public TMP_Text UpgradeOptionsStar;
        [HideInInspector] public int UpgradeOptionsID;
        [HideInInspector] public int Elite;
    }

    [SerializeField] GameObject _upgradeManager;
    [SerializeField] FloatVariable GameSpeed;
    [SerializeField] List<UpgradePanels> UpgradeOptions;
    private UpgradeManager _upgradeManagerScript;
    private float LastTimeScale;
    private bool Selected;

    private void Start() {
        _upgradeManagerScript = _upgradeManager.GetComponent<UpgradeManager>();  
    }

    public void UpgradePlay() {
        Selected = false;
        LastTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;
        GameSpeed.Value = 0.0f;
        for (int i = 0; i < UpgradeOptions.Count; i++) {
            int[] UpgradeID = _upgradeManagerScript.UpgradeIntInfo();
            string[] UpgradeInfos = _upgradeManagerScript.UpgradeStringInfo();
            UpgradeOptions[i].UpgradeOptionsID = UpgradeID[0];
            UpgradeOptions[i].Elite = UpgradeID[3];
            //UpgradeOptions[i].UpgradeOptionsStar.text = UpgradeID[1].ToString();
            if (UpgradeID[1] > 0 && UpgradeID[1] + 1 < UpgradeID[2])
                UpgradeOptions[i].UpgradeOptionsName.text = UpgradeInfos[0] + " LV: " + (UpgradeID[1]).ToString() + " -> LV: " + (UpgradeID[1]+1).ToString();
            else if (UpgradeID[1] + 1 >= UpgradeID[2])
                UpgradeOptions[i].UpgradeOptionsName.text = UpgradeInfos[0] + " LV: " + (UpgradeID[1]).ToString() + " -> MAX";
            else if (UpgradeID[1] <= 0 && UpgradeID[3] == 0)
                UpgradeOptions[i].UpgradeOptionsName.text = UpgradeInfos[0] + " NEW";
            else if (UpgradeID[3] > 0)
                UpgradeOptions[i].UpgradeOptionsName.text = UpgradeInfos[0] + " ELITE";

            UpgradeOptions[i].UpgradeOptionsDesc.text = UpgradeInfos[1];
        }
    }

    public void UpgradeSelected(BaseEventData eventData)
    {
        if (!Selected)
        {
            int parse = int.Parse(eventData.selectedObject.name);
            int ID = UpgradeOptions[parse].UpgradeOptionsID;
            int Elite = UpgradeOptions[parse].Elite;
            _upgradeManagerScript.UpgradeOptionSelected(ID, Elite);
            Selected = true;
            
        }
        Time.timeScale = LastTimeScale;
        GameSpeed.Value = Time.timeScale;
    }

}
