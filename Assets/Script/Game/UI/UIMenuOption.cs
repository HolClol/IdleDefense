using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMenuOption : MonoBehaviour
{
    [SerializeField] TMP_Text _FastForwardText;
    bool FastForwardOn =  false;

    public void FastForward() {
        if (Time.timeScale > 0) {
            if (!FastForwardOn) {
                FastForwardOn = true;
                _FastForwardText.text = ">>";
                Time.timeScale = 1.5f;
            }
            else {
                FastForwardOn = false;
                _FastForwardText.text = ">";
                Time.timeScale = 1f;
            }
        }
        
    }
}
