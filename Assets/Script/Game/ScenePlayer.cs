using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenePlayer : MonoBehaviour
{
    // All stats should be saved here
    public CurrentStage CurrentStage;
    public FloatVariable GameSpeed;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator LoadLevel(int order)
    {
        switch (order)
        {
            case 0: // Replay
            case 1: // Stage
                var asyncLoadStage = SceneManager.LoadSceneAsync(CurrentStage.Stage.StageScene, LoadSceneMode.Single);

                while (!asyncLoadStage.isDone)
                    yield return null;
                break;
            case 2: // Main menu
                var asyncLoadMenu = SceneManager.LoadSceneAsync("MainMenuReal", LoadSceneMode.Single);

                while (!asyncLoadMenu.isDone)
                    yield return null;
                break;
            
        }
        Time.timeScale = 1;
        GameSpeed.Value = 1f;

    }

    public void SetUpStage(int[] stat)
    {
        StartCoroutine(LoadLevel(stat[0]));    
    }

}
