using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnigmaticSaw", menuName = "ScriptableObjects/Abilities/EnigmaticSaw", order = 1)]
public class EnigmaticSO : AbilitiesSO
{ 
    [Header("Weapon Set Up")]
    public int RazorbladeNumbers = 1;
    public float AdditionalScale = 0f;
    public float Duration = 5f;
    public float DamageInterval = 0.6f;
}
