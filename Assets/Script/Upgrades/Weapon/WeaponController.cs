using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject FiringPoint;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] IntVariable DamageType;
    [SerializeField] int NumbOfBullets = 1;
    [SerializeField] int Damage = 20;
    [SerializeField] int Piercing = 0;
    [SerializeField] float FireRate = 0.15f;
    [SerializeField] float AdditionalBulletSpeed = 0f;
    [Range(0f, 1f)] [SerializeField] private float CritRate = 0.1f;
    [SerializeField] private float CritDamage = 1f;

    private List<GameObject> BulletList = new List<GameObject>();
    private List<BulletController> BulletScriptList = new List<BulletController>();
    private Animator _animator;
    private GameObject MainChar;    
    private PlayerController playerController;

    private float FireCooldown;
    private float DamageScaling = 1.0f;
    private float SpreadAngle = 15f;
    private bool AnimationDisabled = false;
    private int BaseDamage;
    private int WeaponUpgradeLevel = 0;
    private int EliteID = 0;

    // Start is called before the first frame update
    void Start()
    {
        MainChar = GameObject.Find("Player");
        playerController = MainChar.GetComponent<PlayerController>();
        _animator = FiringPoint.GetComponent<Animator>(); 
        DamageScaling -= 0.6f;
        BaseDamage = Damage;
        CheckUpgrade(WeaponUpgradeLevel);
    }

    void FixedUpdate() {

        if (playerController.LockedIn && FireCooldown <= 0) {
            FireBullet(NumbOfBullets);
            FireCooldown = FireRate;
        }
        else {
            FireCooldown -= Time.deltaTime;
            if (!AnimationDisabled)
                DisableAnimator();
        }
    }

    private void DisableAnimator()
    {
        if (_animator.isActiveAndEnabled)
            if (_animator.GetBool("Firing"))
                _animator.SetBool("Firing", false);
        AnimationDisabled = true;
    }
    private void FireBullet(int repeat) 
    {
        if (_animator.isActiveAndEnabled)
            if (!_animator.GetBool("Firing"))
            {
                AnimationDisabled = false;
                _animator.SetBool("Firing", true);
            }
        if (repeat == 1)
        {
            GameObject ClonedBullet = GetPooledObject(FiringPoint.transform.position, FiringPoint.transform.rotation);
            return;
        }

        // Spread bullets horizontally around the firePoint
        float spreadWidth = 0.25f; // Distance between bullets on the horizontal axis
        float halfBulletCount = (repeat - 1) / 2f; // Used for centering the spread

        for (int i = 0; i < repeat; i++)
        {
            // Calculate the offset relative to the firePoint
            float horizontalOffset = (i - halfBulletCount) * spreadWidth;

            // Calculate the bullet's position (offset along the right axis of firePoint)
            Vector3 bulletPosition = FiringPoint.transform.position + FiringPoint.transform.right * horizontalOffset;

            GameObject ClonedBullet = GetPooledObject(bulletPosition, FiringPoint.transform.rotation /** Quaternion.Euler(0,0,90f)*/);
        }
        
           
    }

    private GameObject GetPooledObject(Vector3 pos, Quaternion rotation)
    {
        for (int i = 0; i < BulletList.Count; i++)
        {
            if (!BulletList[i].activeInHierarchy)
            {
                BulletList[i].transform.position = pos;
                BulletList[i].transform.rotation = rotation;
                BulletList[i].SetActive(true);
                BulletScriptList[i].UpdateStat(new int[] { Damage, Piercing }, new float[] { AdditionalBulletSpeed, CritRate, CritDamage });
                BulletScriptList[i].StartUp();
                return BulletList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject BulletNew = Instantiate(BulletPrefab, pos, rotation, GameObject.Find("_Projectiles").transform);

        BulletNew.SetActive(true);
        BulletNew.GetComponent<BulletController>().UpdateStat(new int[] { Damage, Piercing }, new float[] { AdditionalBulletSpeed, CritRate, CritDamage });

        BulletList.Add(BulletNew);
        BulletScriptList.Add(BulletNew.GetComponent<BulletController>());
        return BulletNew;
    }

    public void CheckUpgrade(int upgradelevel) {
        WeaponUpgradeLevel = upgradelevel;
        switch (WeaponUpgradeLevel) {
            case 1:
                DamageScaling += 0.1f;
                break;
            case 2:
                Piercing += 1;
                break;
            case 3:
                FireRate -= 0.02f;
                break;
            case 4:
                NumbOfBullets += 1;
                DamageScaling -= 0.1f;
                break;
            case 5:
                CritRate += 0.1f;
                break;

        }

        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public virtual void UnlockELite(int eliteid)
    {
        EliteID = eliteid;
    }

    public void UpdateDamage(int dmg) {
        if (this.enabled) {
            Damage = BaseDamage + (int)((float)(Damage) * DamageScaling);
        }
    }
}
