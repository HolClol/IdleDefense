using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIStageInfo : MonoBehaviour
{
    public StageInfo StageInfo;
    public TMP_Text StageText;

    void Start()
    {
        StageText.text = StageInfo.StageName;
    }

}
