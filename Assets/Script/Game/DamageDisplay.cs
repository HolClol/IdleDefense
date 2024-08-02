using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageDisplay : MonoBehaviour
{
    public TextMeshPro damageText;
    public Animator _animator;

    public void UpdateDisplay(int dmg) {
        _animator.Play("MovingUp");
        damageText.text = dmg.ToString();
        StartCoroutine(Disable());
    }

    private IEnumerator Disable() {
        yield return new WaitForSeconds(0.6f);
        gameObject.SetActive(false);
    }
}
