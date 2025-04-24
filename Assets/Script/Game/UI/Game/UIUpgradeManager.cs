using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Advertisements;

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
    private Animator _animator;
    private bool Rerolled = false;
    private float LastTimeScale;
    private bool Selected;

    private void Start() {
        _animator = GetComponent<Animator>();
        _upgradeManagerScript = _upgradeManager.GetComponent<UpgradeManager>();  
    }

    private void SuccessReroll()
    {
        _animator.Play("UpgradeOutro");
        _upgradeManagerScript.ResetTable();
        StartCoroutine(DelayedRun());
    }

    private IEnumerator DelayedRun()
    {
        yield return new WaitForSecondsRealtime(0.75f);
        UpgradePlay();
    }
    private void FailedReroll()
    {

    }

    // Upgrade selection
    public void UpgradePlay() {
        _animator.Play("UpgradeIntro");
        Selected = false;
        Time.timeScale = 0.0f;
        if (GameSpeed.Value > 0)
        {
            LastTimeScale = GameSpeed.Value;
            GameSpeed.Value = 0;
        }

        // Change all three tabs into selected upgrades
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

    public void Reroll(BaseEventData eventData)
    {
        if (!Selected)
        {
            AdsManager.instance.OnShowRewardVideo(SuccessReroll, FailedReroll);
        }
    }

}
