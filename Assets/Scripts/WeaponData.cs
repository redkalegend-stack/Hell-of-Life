using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject {
    [SerializeField] private string weaponName;

    [SerializeField] private float cooldownTime;

} 