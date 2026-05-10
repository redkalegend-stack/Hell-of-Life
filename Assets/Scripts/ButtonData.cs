using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Button Data", menuName = "Button Data")]
public class ButtonData : ScriptableObject {
    [SerializeField] private string buttonName;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float bulletLifetime;
}