using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : ProjectileController
{
    private Vector3 OldScale, TargetScale, OriginalScale;
    private Vector3 ScaleValue = new Vector3(0f, 0f, 0f);
    protected override void Awake()
    {
        OriginalScale = gameObject.transform.localScale;
    }
    protected override void OnEnable()
    {
        Invoke(nameof(OnDisable), 3f);
    }

    private void OnDisable()
    {
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
        }
    }
}
