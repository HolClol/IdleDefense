using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BeamController : ProjectileController
{
    [SerializeField] private GameObject _TempTarget;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private LineRenderer LaserPointer;
    [HideInInspector] public List<GameObject> FractionBeam = new List<GameObject>();
    private List<BeamController> FractionBeamScript = new List<BeamController>();
    private List<GameObject> Targets = new List<GameObject> { };
    private GameObject IgnoreMain;

    private Vector3 TargetPos = new Vector3(0f, 0f, 0f);
    private Vector3 MainTarget;
    private LayerMask enemyLayer;

    private float LaserDistance = 12f;
    private float Duration = 4f;
    private float DamageInterval = 0.5f;
    private int FractionBeamCount = 3;
    private int TargetNumber;
    private int Piercing = 0;
    private bool FractionUnlocked = false;
    private bool SetUpFractions = false;
    private bool Fired = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        if (!MainProjectile)
            LaserDistance = LaserDistance / 3;
        StartUp();  
    }

    protected override void FixedUpdate()
    { 
        if (!Fired)
            return;
        if (Targets.Count <= 0)
            return;

        FindTarget();

        if (_TempTarget != null)
        {
            TargetPos = _TempTarget.transform.position;
            RotateToTarget(TargetPos);

            if (IsFacingTarget(TargetPos) && Vector3.Distance(transform.position, TargetPos) <= LaserDistance)
            {
                ShootLaser();
                if (MainProjectile && FractionUnlocked)
                    UpdatePosition();
            }
                
            else
            {
                StartCoroutine(ResetBeam());
                if (MainProjectile && FractionUnlocked)
                    foreach (var fraction in FractionBeam)
                        fraction.SetActive(false);
                        
            }
                
        }   

    }

    private void FindTarget()
    {
        while (_TempTarget == null || !Targets.Contains(_TempTarget) || Vector3.Distance(transform.position, TargetPos) > LaserDistance)
        {
            do
            {
                TargetNumber = Random.Range(0, Targets.Count);
            }
            while (Targets[TargetNumber] == null || (Targets[TargetNumber] == IgnoreMain && !MainProjectile));

            _TempTarget = Targets[TargetNumber];
            TargetPos = Targets[TargetNumber].transform.position;
                
            if (MainProjectile && FractionUnlocked)
            {
                IgnoreMain = Targets[TargetNumber];
                MainTarget = Targets[TargetNumber].transform.position;
            }
                     
        }
    }

    private void UpdatePosition()
    {
        for (int i = 0; i < FractionBeam.Count; i++)
        {
            FractionBeam[i].transform.position = MainTarget;
            FractionBeamScript[i].SetMainTarget(MainTarget, IgnoreMain);     
        }      
    }

    private void ShootLaser()
    {
        if (!Physics2D.Raycast(transform.position, transform.right))
            return;

        RaycastHit2D[] _hit = Physics2D.RaycastAll(FirePoint.position, transform.right, LaserDistance, enemyLayer);
        if (_hit.Length == 0) 
            return; 

        // Find how many targets are within the raycast and maximum amount it can pierce
        int index = _hit.Length - 1;
        if (index > Piercing)
            index = Piercing;

        if (!MainProjectile && _hit[index].collider != null && _hit[index].collider.gameObject == IgnoreMain)
            index = _hit.Length - 1;

        if (_hit[index].collider != null) // Main Target
        {
            Draw2DRay(FirePoint.position, _hit[index].point);

            if (MainProjectile && FractionUnlocked)
            {  
                IgnoreMain = _hit[index].collider.gameObject;
                MainTarget = IgnoreMain.transform.position;
            }
                
            for (int i = 0; i < index + 1; i++)
            {
                if (_hit[i].collider != null) // Check other targets within the raycast
                    if (_hit[i].collider.gameObject.CompareTag("Enemy"))
                    {
                        if (_hit[i].collider.gameObject == IgnoreMain && !MainProjectile)
                            continue;

                        SendDamage(_hit[i].collider.gameObject,
                        new int[] { Damage, ID, DamageType, 0 },
                        new float[] { Knockback, DamageInterval, CritRate, CritDamage });
                    }
                        
            }

        }

    }

    private void Draw2DRay(Vector2 StartPos, Vector2 EndPos)
    {
        LaserPointer.SetPosition(0, StartPos);
        LaserPointer.SetPosition(1, EndPos);
    }

    private bool IsFacingTarget(Vector3 target)
    {
        Vector2 forward = FirePoint.transform.right;
        Vector2 directionToTarget = (target - FirePoint.transform.position).normalized;

        float angle = Vector2.Angle(forward, directionToTarget);

        return angle < 5f;
    }

    private void RotateToTarget(Vector3 targetPos)
    {
        Vector2 Target_direction = targetPos - transform.position;
        float angle = Mathf.Atan2(Target_direction.y, Target_direction.x) * Mathf.Rad2Deg;

        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = q;
    }

    private IEnumerator ResetBeam()
    {
        Draw2DRay(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        Fired = false;
        yield return new WaitForSeconds(0.5f);
        Fired = true;
    }

    private IEnumerator ActivateBeam()
    {
        Fired = true;          
        if (MainProjectile && FractionUnlocked && !SetUpFractions)
        {
            for (int i = 0; i < FractionBeam.Count; i++)
            {
                FractionBeamScript.Add(FractionBeam[i].GetComponent<BeamController>());
            }
            SetUpFractions = true;
        }
        yield return new WaitForSeconds(Duration);
        gameObject.SetActive(false);    
    }

    public void OnDisable()
    {
        Fired = false;
        Draw2DRay(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        TargetNumber = 0;
        TargetPos = new Vector3(0, 0, 0);
        _TempTarget = null;

        if (MainProjectile && FractionUnlocked)
            foreach (var fraction in FractionBeam)
                fraction.SetActive(false);
    }

    public override void StartUp()
    {
        base.StartUp();
        StartCoroutine(ActivateBeam());
    }

    public override void UpdateStat(int[] intvalue, float[] floatvalue)
    {
        Damage = intvalue[0];
        DamageType = intvalue[1];
        Piercing = intvalue[2];
        FractionUnlocked = intvalue[3] == 1 ? true : false;
        FractionBeamCount = intvalue[4];
        Knockback = floatvalue[0];
        Duration = floatvalue[1];
        DamageInterval = floatvalue[2];
        CritRate = floatvalue[3];
        CritDamage = floatvalue[4];
    }

    public void GetPooledTargets(List<GameObject> EnemyPool)
    {
        Targets = EnemyPool;
    }

    public void SetMainTarget(Vector3 target, GameObject maintarget)
    {
        MainTarget = target;
        IgnoreMain = maintarget;
    }
}
