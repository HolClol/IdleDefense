using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilesController : ProjectileController
{
    [SerializeField] Crosshair _crosshairscript;
    [SerializeField] GameObject MainPlayer;
    [SerializeField] GameObject Missile;
 
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _collider;
    private Vector3 OldScale, TargetScale, LerpScale, OriginalScale;
    private Vector3 ScaleValue = new Vector3(0f, 0f, 0f);
    private float Delay;
    private float startTime;
    private bool Fired = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CircleCollider2D>();
        OriginalScale = gameObject.transform.localScale;

        StartUp();
    }

    void LateUpdate() {
        if (Fired) {
            _collider.radius = 0.5f;
            LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, Time.deltaTime * 8f);
            OldScale = LerpScale;
            gameObject.transform.localScale = LerpScale;
        }
    }

    private void FireMissile(float time)
    {
        Vector3 center = (Vector3.zero + transform.position) * 0.5f;
        center -= new Vector3(0, Random.Range(-10f,10f), 0);

        Vector3 riseRelCenter = Vector3.zero - center;
        Vector3 setRelCenter = transform.position - center;

        float fracComplete = (Time.time - startTime) / (Delay - time);
        Vector3 newPos = Vector3.Slerp(riseRelCenter, setRelCenter, fracComplete);
        Vector3 direction = (newPos + center) - Missile.transform.position;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Missile.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Missile.transform.position = newPos + center;
    }

    private IEnumerator BlastOff() {
        yield return new WaitForSeconds(1f);
        Missile.transform.position = Vector3.zero;
        startTime = Time.time;
        Missile.SetActive(true);

        //Fire a missile to the target location
        float time = 0;
        while (Vector2.Distance(Missile.transform.position, transform.position) > 0.5f && time <= 3f)
        {
            FireMissile(time);
            time += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }  

        _crosshairscript.enabled = false;
        _spriteRenderer.color = Color.white;
        _animator.Play("HomingMissiles");
        Missile.SetActive(false);
        Fired = true;

        yield return new WaitForSeconds(0.15f);
        OldScale = gameObject.transform.localScale;
        TargetScale = gameObject.transform.localScale + new Vector3(1f, 1f, 1f);
        LerpScale = Vector3.LerpUnclamped(OldScale, TargetScale, 0);

        yield return new WaitForSeconds(0.1f);
        Fired = false;

        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    public override void StartUp() {
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

    void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy") && Fired) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] {5f, 0.25f, CritRate, CritDamage });
        }  
    }
    void OnTriggerStay2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy") && Fired) {
            SendDamage(trigger.gameObject, new int[] { Damage, ID, DamageType, 0 }, new float[] { 5f, 0.25f, CritRate, CritDamage });
        }  
    }
}
