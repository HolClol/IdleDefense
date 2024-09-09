using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticOrb : ProjectileController
{
    private SpriteRenderer _spriteRenderer;
    private Color _spriteColor;
    private int Piercing;
    private float ExistingTime = 5f;
    private float RecoverDuration = 2f;
    private bool Recovering = false;
    private bool Maxed = false;

    protected override void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteColor = _spriteRenderer.color;
        Knockback = 6f;
        StartUp();
    }

    private IEnumerator Recover(float timer) {
        yield return new WaitForSeconds(timer);
        Recovering = false;
        _spriteColor.a = 1f;
        _spriteRenderer.color = _spriteColor;
    }

    private IEnumerator DisableObject(float timer) {
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }

    public override void StartUp() {
        StartCoroutine(Recover(0f));
        if (!Maxed) {
            StartCoroutine(DisableObject(ExistingTime));
        }
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue) {
        Damage = intvalue[0];
        Piercing = intvalue[1];
        if (intvalue[2] == 1)
            Maxed = true;
        DamageType = intvalue[3];
        Knockback = floatvalue[0];
        ExistingTime = floatvalue[1];
        RecoverDuration = floatvalue[2];
        CritRate = floatvalue[3];
        CritDamage = floatvalue[4];
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy") && !Recovering) {
            if (Piercing <= 0) {
                Recovering = true;
                _spriteColor.a = 0.3f;
                _spriteRenderer.color = _spriteColor;
                StartCoroutine(Recover(RecoverDuration));
            }
            else {
                Piercing -= 1;
            }
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { Knockback, 0f, CritRate, CritDamage });
            
        }
    }
}
