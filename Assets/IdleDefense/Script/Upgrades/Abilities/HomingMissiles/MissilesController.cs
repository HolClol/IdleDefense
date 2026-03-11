using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilesController : ProjectileController
{
    [SerializeField] protected Crosshair _crosshairscript;
    [SerializeField] protected GameObject MainPlayer;
    [SerializeField] protected GameObject[] Missile;
    [SerializeField] protected bool Curve = false;
 
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected CircleCollider2D _collider;
    protected Vector3 OldScale, TargetScale, LerpScale, OriginalScale;
    protected Vector3 ScaleValue = new Vector3(0f, 0f, 0f);
    protected List<float> Curves = new List<float> { };
    protected float Delay;
    protected float startTime;
    protected bool Fired = false;
    // Start is called before the first frame update
    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        OriginalScale = gameObject.transform.localScale;
    }

    protected void LateUpdate() {
        if (Fired) {
            _collider.radius = 0.5f;
            LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, Time.deltaTime * 5f);
            OldScale = LerpScale;
            gameObject.transform.localScale = LerpScale;
        }
    }

    protected void FireMissile(GameObject missile, float time, float curve)
    {
        Vector3 center = (Vector3.zero + transform.position) * 0.5f;
        center -= new Vector3(0, curve, 0);

        Vector3 riseRelCenter = Vector3.zero - center;
        Vector3 setRelCenter = transform.position - center;

        float fracComplete = (Time.time - startTime) / (Delay - time);
        Vector3 newPos = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        Vector3 direction = (newPos + center) - missile.transform.position;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            missile.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        missile.transform.position = newPos + center;
    }

    protected virtual IEnumerator BlastOff() {
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

        float delay = 0f;
        if (MainProjectile)
        {
            _animator.Play("HomingMissiles");
            yield return new WaitForSeconds(0.15f);

            OldScale = gameObject.transform.localScale;
            TargetScale = gameObject.transform.localScale + new Vector3(1f, 1f, 1f);
            LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, 0);

            delay = 1f;
        }  

        _crosshairscript.enabled = false;
        _spriteRenderer.color = Color.white;
        Curves.Clear();
        foreach (var m in Missile)
            m.SetActive(false);

        yield return new WaitForSeconds(0.1f);
        Fired = false;

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    protected virtual IEnumerator Tracking(GameObject m, float curve)
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
        if (!MainProjectile)
        {
            MainScript.TargetStruckSignal(new GameObject[] { m });
        }  
    }

    protected override void OnEnable() {
        base.StartUp();
        OldScale = new Vector3(0.5f, 0.5f, 0.5f) + ScaleValue;
        TargetScale = new Vector3(3.5f, 3.5f, 3.5f) + ScaleValue;
        LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, 0);
        _animator.Play("IntroAnim");
        _crosshairscript.enabled = true;

        StartCoroutine(BlastOff());
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue) {
        Damage = intvalue[0];
        DamageType = intvalue[1];
        Delay = floatvalue[0];
        Knockback = floatvalue[1];
        ScaleValue = new Vector3 (floatvalue[2], floatvalue[3], floatvalue[4]);
        CritRate = floatvalue[5];
        CritDamage = floatvalue[6]; 

        if (gameObject.transform.localScale != OriginalScale + ScaleValue)
        {
            gameObject.transform.localScale = OriginalScale + ScaleValue;
            OldScale = new Vector3(0.5f, 0.5f, 0.5f) + ScaleValue;
            TargetScale = new Vector3(3.5f, 3.5f, 3.5f) + ScaleValue;
            LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, 0);
        }
        
    }

    protected void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy") && Fired) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] {5f, 0.2f, CritRate, CritDamage });
        }  
    }
    protected void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy") && Fired) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { 5f, 0.2f, CritRate, CritDamage });
        }  
    }
}
