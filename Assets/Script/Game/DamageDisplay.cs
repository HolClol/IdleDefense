using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    
    public TextMeshPro damageText;
    private Vector3 targetPos = new Vector3(0,1,0);
    private Tween moveTween, fadeTween, scaleTween/*, colorTween*/;

    private void Awake()
    {
        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(500, 50);
        DOTween.defaultAutoPlay = AutoPlay.None;
        // Store the tweens in variables
        moveTween = DOTween.To(() => transform.position, x => transform.position = x, targetPos + new Vector3(0, 2f, 0), 0.6f)
            .SetAutoKill(false)
            .Pause();
        fadeTween = DOTween.ToAlpha(() => damageText.color, x => damageText.color = x, 0f, 0.4f)
            .From(1f)
            .SetDelay(0.3f)
            .SetAutoKill(false)
            .Pause();
        scaleTween = DOTween.To(() => transform.localScale, x => transform.localScale = x, Vector3.zero, 0.4f)
            .From(Vector3.one)
            .SetDelay(0.4f)
            .SetAutoKill(false)
            .Pause();
        /*colorTween = damageText.DOColor(Color.yellow, 0.2f)
            .SetAutoKill(false);*/
    }

    public void UpdateDisplay(int dmg, Vector3 pos, bool crit) {

        moveTween.Rewind();
        fadeTween.Rewind();
        scaleTween.Rewind();

        targetPos = pos + new Vector3(0, 1f, 0);
        ((Tweener)moveTween).ChangeStartValue(targetPos, -1f);

        if (!crit)
            SetDamageText(crit, dmg, Color.white, 6f, 0.6f, 0.4f, 0.4f);
        else
            SetDamageText(crit, dmg, Color.yellow, 8f, 1.2f, 1f, 1f);

        // Replay the tweens 
        ResetAndRestartTween(moveTween);
        ResetAndRestartTween(fadeTween);
        ResetAndRestartTween(scaleTween);

        StartCoroutine(Disable(crit));
    }

    void ResetAndRestartTween(Tween tween)
    {
        if (tween.IsComplete())
        {
            tween.Rewind();  // Rewind to reset the tween fully
        }

        tween.Restart();  // Restart the tween after rewinding
    }

    private void SetDamageText(bool crit, int dmg, Color textColor, float fontSize, float moveDuration, float fadeDuration, float scaleDuration)
    {
        damageText.color = textColor;
        damageText.text = crit ? $"{dmg}!" : dmg.ToString();
        damageText.fontSize = fontSize;

        Vector3 moveEndValue = targetPos + new Vector3(0, 2, 0);
        Color fadeEndColor = damageText.color; 
        fadeEndColor.a = 0f;

        UpdateTweenValues(moveEndValue, fadeEndColor, fadeDuration, scaleDuration, moveDuration);
    }

    private void UpdateTweenValues(Vector3 moveEndValue, Color fadeEndColor, float fadeDuration, float scaleDuration, float moveDuration)
    {
        ((Tweener)moveTween).ChangeEndValue(moveEndValue, moveDuration, true);
        ((Tweener)fadeTween).ChangeEndValue(fadeEndColor, fadeDuration, true);
        ((Tweener)scaleTween).ChangeEndValue(Vector3.zero, scaleDuration, true);
    }

    private IEnumerator Disable(bool crit) {
        if (crit)
            yield return new WaitForSeconds(1.5f);
        else
            yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
    }
}
