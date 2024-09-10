using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitterShotgun", menuName = "ScriptableObjects/Abilities/SplitterShotgun", order = 1)]
public class SplitterSO : AbilitiesSO
{
    [Header("Weapon Set Up")]
    public float BulletLifetime = 0.65f;
    public int BulletNumb = 5;
    public int Radius = 7;
    public int Bounce = 0;
    public int Repeat = 1;
}
