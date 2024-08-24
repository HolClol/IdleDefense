using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class UISelectStage : MonoBehaviour
{
    public RectTransform ContentPanel;
    public TMP_Text StageText;
    public CurrentStage CurrentStage;
    public UnityEvent<int[]> ChangeScene;

    private GameObject[] ContentStageInfos;

    // Start is called before the first frame update
    void Start()
    {
        CurrentStage.ResetStage();
        int childCount = ContentPanel.childCount;
        ContentStageInfos = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            ContentStageInfos[i] = ContentPanel.GetChild(i).gameObject;
        }
    }

    public void StageSelected(BaseEventData eventData)
    {
        StageInfo tempStage = eventData.selectedObject.GetComponent<UIStageInfo>().StageInfo;
        if (CurrentStage.Stage != tempStage) //Prevent running if it's same stage
        {
            CurrentStage.Stage = tempStage;
            StageText.text = tempStage.StageName;
        }
        
    }

    public void StageConfirm(BaseEventData eventData)
    {
        ChangeScene.Invoke(new int[] {1});
    }
}
