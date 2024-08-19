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

    private List<GameObject> tempUniqueEnemies = new List<GameObject>();
    private StageInfo currentStage, tempCurrentStage;

    public void SetCurrentStage(StageInfo stage)
    {
        currentStage = stage;
    }
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
        if (currentStage != tempCurrentStage) //Prevent running if it's same stage
        {
            tempUniqueEnemies.Clear();
            foreach (Transform child in IconPanel.transform)
            {
                Destroy(child.gameObject);
            }
            tempCurrentStage = currentStage;
            for (int i = 0; i < currentStage.StageEnemies.LevelPrefab.Count; i++)
            {
                SelectUnique(currentStage.StageEnemies.LevelPrefab[i].EnemyPrefab);
            }
            SelectUnique(currentStage.StageEnemies.BossPrefab);
            CoinsText.text = "Coins: " + currentStage.StageReward.Coins.ToString();
            PointsText.text = "Points: " + currentStage.StageReward.UserRankExp.ToString();
        }


    }
}
