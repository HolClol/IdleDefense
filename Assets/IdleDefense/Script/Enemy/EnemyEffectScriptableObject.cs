using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyEffectPrefab", menuName = "ScriptableObjects/Enemy/EnemyEffectScriptableObject", order = 1)]
public class EnemyEffectScriptableObject : ScriptableObject
{
    public GameObject[] EffectPrefab;
}
