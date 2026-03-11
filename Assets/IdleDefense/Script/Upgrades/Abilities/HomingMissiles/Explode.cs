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
            LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, Time.deltaTime * 4f);
            OldScale = LerpScale;
            gameObject.transform.localScale = LerpScale;
        }
        
    }
    protected override void OnEnable()
    {
        _animator.Play("HomingMissiles");
        OldScale = new Vector3(0.5f, 0.5f, 0.5f) + ScaleValue;
        TargetScale = new Vector3(3.5f, 3.5f, 3.5f) + ScaleValue;
        LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, 0);
        Fired = true;
        Invoke(nameof(OnDisable), 0.5f);
    }

    private void OnDisable()
    {
        Fired = false;
        transform.gameObject.SetActive(false);
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue)
    {
        ScaleValue = new Vector3(floatvalue[2], floatvalue[3], floatvalue[4]);
        if (gameObject.transform.localScale != OriginalScale + ScaleValue)
        {
            gameObject.transform.localScale = OriginalScale + ScaleValue;
            OldScale = new Vector3(0.5f, 0.5f, 0.5f) + ScaleValue;
            TargetScale = new Vector3(3.5f, 3.5f, 3.5f) + ScaleValue;
            LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, 0);
        }
    }
}
