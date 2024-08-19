using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class UISelectStage : MonoBehaviour
{
    public RectTransform ContentPanel;
    public TMP_Text StageText;
    public UIExtraInfo UIInfo;

    private GameObject[] ContentStageInfos;
    private StageInfo currentStage;

    // Start is called before the first frame update
    void Start()
    {
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
        if (currentStage != tempStage) //Prevent running if it's same stage
        {
            currentStage = tempStage;
            StageText.text = currentStage.StageName;
            UIInfo.SetCurrentStage(currentStage);
        }
        
    }

    public void StageConfirm(BaseEventData eventData)
    {
        SceneManager.LoadScene(currentStage.name, LoadSceneMode.Single);
    }
}
