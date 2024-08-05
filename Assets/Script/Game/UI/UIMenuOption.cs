using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMenuOption : MonoBehaviour
{
    [SerializeField] TMP_Text _FastForwardText;
    public FloatVariable GameSpeed;
    private bool FastForwardOn =  false;
    private bool Paused = false;

    public void FastForward() {
        if (Time.timeScale > 0) {
            if (!FastForwardOn) {
                FastForwardOn = true;
                _FastForwardText.text = ">>";
                GameSpeed.Value = 1.5f;
                Time.timeScale = GameSpeed.Value;
            }
            else {
                FastForwardOn = false;
                _FastForwardText.text = ">";
                GameSpeed.Value = 1f;
                Time.timeScale = GameSpeed.Value;
            }
        }
    }

    public void PauseMenu()
    {
        if (!Paused)
        {
            Time.timeScale = 0f;
            Paused = true;
        }
        else
        {
            Time.timeScale = GameSpeed.Value;
            Paused = false;
        }
    }
}
