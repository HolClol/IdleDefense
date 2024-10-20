using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : ProjectileController
{
    [SerializeField] Animator _animator;

    private Vector3 OldScale, TargetScale, LerpScale, OriginalScale;
    private Vector3 ScaleValue = new Vector3(0f, 0f, 0f);

    private bool Fired = false;

    protected override void Awake()
    {
        OriginalScale = gameObject.transform.localScale;
    }

    void LateUpdate()
    {
        if (Fired)
        {
            LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, Time.deltaTime * 8f);
            OldScale = LerpScale;
            gameObject.transform.localScale = LerpScale;
        }
    }

    private void OnEnable()
    {
        _animator.Play("Explode");
        Invoke("OnDisable", 0.25f);
    }

    private void OnDisable()
    {
        transform.gameObject.SetActive(false);
    }
}
