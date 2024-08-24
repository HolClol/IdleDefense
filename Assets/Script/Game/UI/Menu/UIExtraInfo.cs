using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIExtraInfo : MonoBehaviour
{
    public GameObject IconPanel;
    public GameObject IconPrefab;
    public TMP_Text CoinsText;
    public TMP_Text PointsText;
    public CurrentStage CurrentStage;

    private List<GameObject> tempUniqueEnemies = new List<GameObject>();
    private StageInfo tempCurrentStage;

    private void SelectUnique(List<GameObject> enemyprefab)
    {
        //Create a list of unique objects to put on icon
        foreach (GameObject enemy in enemyprefab)
        {
            if (!tempUniqueEnemies.Contains(enemy))
            {
                tempUniqueEnemies.Add(enemy);
                GameObject enemyIcon = Instantiate(IconPrefab, IconPanel.transform);
                enemyIcon.GetComponent<Image>().sprite = enemy.GetComponent<EnemyMain>().EnemySprite;
            }

        }
    }

    public void StageExtraInfo(BaseEventData eventData)
    {
        if (CurrentStage.Stage != tempCurrentStage) //Prevent running if it's same stage
        {
            tempUniqueEnemies.Clear();
            foreach (Transform child in IconPanel.transform)
            {
                Destroy(child.gameObject);
            }
            tempCurrentStage = CurrentStage.Stage;
            for (int i = 0; i < CurrentStage.Stage.StageEnemies.LevelPrefab.Count; i++)
            {
                SelectUnique(CurrentStage.Stage.StageEnemies.LevelPrefab[i].EnemyPrefab);
            }
            SelectUnique(CurrentStage.Stage.StageEnemies.BossPrefab);
            CoinsText.text = "Coins: " + CurrentStage.Stage.StageReward.Coins.ToString();
            PointsText.text = "Points: " + CurrentStage.Stage.StageReward.UserRankExp.ToString();
        }


    }
}
