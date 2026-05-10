using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Settings", menuName = "AI/Enemy Settings", order = 1)]
public class EnemySettings : ScriptableObject {
    [Header("RayCasting")]
    public float visionAngle;
    public float visionDistance;
    public int rayCount;
    public LayerMask wallLayer;
    public LayerMask playerLayer;

    [Header("Alert")]
    public float speed;
    public float rotationSpeed;

    [Header("Wandering")]
    public float wanderSpeed;
    public float wanderRotationSpeed;

    [Header("Detection")]
    public float detectionRange;
    public float retreatDistanceMax;
    public float retreatDistanceMin;
}
