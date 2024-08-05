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
        public Button UpgradeOptionsButton;
        public TMP_Text UpgradeOptionsName;
        public TMP_Text UpgradeOptionsDesc;
        public TMP_Text UpgradeOptionsStar;
        public int UpgradeOptionsID;
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
        GameSpeed.Value = Time.timeScale;
        for (int i = 0; i < UpgradeOptions.Count; i++) {
            int[] UpgradeID = _upgradeManagerScript.UpgradeIntInfo();
            string[] UpgradeInfos = _upgradeManagerScript.UpgradeStringInfo();
            UpgradeOptions[i].UpgradeOptionsID = UpgradeID[0];
            //UpgradeOptions[i].UpgradeOptionsStar.text = UpgradeID[1].ToString();
            if (UpgradeID[1] > 0)
                UpgradeOptions[i].UpgradeOptionsName.text = UpgradeInfos[0] + " LV: " + UpgradeID[1].ToString();
            else
                UpgradeOptions[i].UpgradeOptionsName.text = UpgradeInfos[0];

            UpgradeOptions[i].UpgradeOptionsDesc.text = UpgradeInfos[1];
        }
    }

    public void UpgradeSelected(BaseEventData eventData)
    {
        if (!Selected)
        {
            int parse = int.Parse(eventData.selectedObject.name);
            _upgradeManagerScript.UpgradeOptionSelected(UpgradeOptions[parse].UpgradeOptionsID);
            Selected = true;
        }
        Time.timeScale = LastTimeScale;
        GameSpeed.Value = Time.timeScale;
    }

}
