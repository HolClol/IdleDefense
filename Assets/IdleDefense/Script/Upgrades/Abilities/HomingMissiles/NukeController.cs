using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class NukeController : MissilesController
{
    public bool SecondExplosion;
    private float SlowRate;
    private float SlowDuration = 3f;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        OriginalScale = gameObject.transform.localScale;
        _animator.SetFloat("Speed", 0.2f);
    }
    protected override IEnumerator BlastOff()
    {
        yield return new WaitForSeconds(1f);
        startTime = Time.time;

        foreach (var m in Missile)
        {
            m.transform.position = Vector3.zero;
            m.SetActive(true);
            float curve = 0;
            if (Curve)
                curve = Random.Range(0, 2) == 0 ? Random.Range(8f, 15f) : Random.Range(-15f, -8f);

            Curves.Add(curve);
            StartCoroutine(Tracking(m, curve));
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }

        while (Curves.Count > 0) yield return new WaitForSeconds(0.1f);

        _animator.Play("HomingMissiles");
        yield return new WaitForSeconds(0.15f);

        OldScale = gameObject.transform.localScale;
        TargetScale = gameObject.transform.localScale + new Vector3(1f, 1f, 1f);
        LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, 0);

        _crosshairscript.enabled = false;
        _spriteRenderer.color = Color.yellow;
        Curves.Clear();
        foreach (var m in Missile)
            m.SetActive(false);

        yield return new WaitForSeconds(1f);
        Fired = false;
        Color color = Color.yellow;
        color.a = 1f;
        _spriteRenderer.color = color;

        MainScript.TargetStruckSignal(new GameObject[] { transform.gameObject });
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    protected override IEnumerator Tracking(GameObject m, float curve)
    {
        float time = 0;
        while (Vector2.Distance(m.transform.position, transform.position) > 0.5f && time <= 3f)
        {
            FireMissile(m, time, curve);
            time += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        Curves.Remove(curve);
        Fired = true;
    }
}
