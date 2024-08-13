using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Set Up")]
    [SerializeField] GameObject[] FiringPoint;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] int NumbOfBullets = 1;
    [SerializeField] int Damage;
    [SerializeField] int Piercing = 0;
    [SerializeField] float FireRate = 0.15f;
    [SerializeField] float AdditionalBulletSpeed = 0f;

    private List<GameObject> BulletList = new List<GameObject>();
    private List<BulletController> BulletScriptList = new List<BulletController>();
    private List<Animator> _animators = new List<Animator>();
    private GameObject MainChar;    
    private PlayerController playerController;

    private float FireCooldown;
    private float DamageScaling;
    private bool AnimationDisabled = false;
    private int WeaponUpgradeLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        MainChar = GameObject.Find("Player");
        playerController = MainChar.GetComponent<PlayerController>();
        for (int i = 0; i < FiringPoint.Length; i++) { _animators.Add(FiringPoint[i].GetComponent<Animator>()); }
        DamageScaling -= 0.6f;
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
        for (int i = 0; i < _animators.Count; i++)
        {
            if (_animators[i].isActiveAndEnabled)
                if (_animators[i].GetBool("Firing"))
                    _animators[i].SetBool("Firing", false);
            AnimationDisabled = true;
        }
    }
    private void FireBullet(int repeat) {
        if (repeat > 3) 
            repeat = 3;
        for (int i = 0; i < repeat; i++) {
            int index = i;
            if (repeat == 2) 
                index += 1;

            if (_animators[i].isActiveAndEnabled)
                if (!_animators[i].GetBool("Firing"))
                {
                    AnimationDisabled = false;
                    _animators[i].SetBool("Firing", true);
                }
                    
            GameObject ClonedBullet = GetPooledObject(FiringPoint[index].transform.position, FiringPoint[index].transform.rotation * Quaternion.Euler(0f, 0f, 90f));

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
                BulletScriptList[i].UpdateStat(Damage, Piercing, AdditionalBulletSpeed);
                BulletScriptList[i].StartUp();
                return BulletList[i];
            }
        }

        // Optionally expand pool if needed
        GameObject BulletNew = Instantiate(BulletPrefab, pos, rotation, GameObject.Find("_Projectiles").transform);

        BulletNew.SetActive(true);
        BulletNew.GetComponent<BulletController>().UpdateStat(Damage, Piercing, AdditionalBulletSpeed);

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
                DamageScaling += 0.15f;
            break;
            case 4:
                FireRate -= 0.025f;
            break;
            case 5:
                NumbOfBullets += 1;
                FiringPoint[0].SetActive(false);
                FiringPoint[1].SetActive(true);
                FiringPoint[2].SetActive(true);
                break;
            case 6:
                AdditionalBulletSpeed += 10f;
            break;
            case 7:
                DamageScaling += 0.2f;
            break;
            case 8:
                FireRate -= 0.025f;
            break;
            case 9:
                Piercing += 1;
            break;
            case 10:
                NumbOfBullets += 1;
                FiringPoint[0].SetActive(true);
            break;

        }

        UpdateDamage(playerController.PlayerStats.BaseDamage);
    }

    public void UpdateDamage(int dmg) {
        if (this.enabled) {
            Damage = dmg;
            Damage += (int)((float)(Damage) * DamageScaling);
        }
    }
}
