using System.Collections;
using System.Runtime.CompilerServices;
using Unity.AppUI.Core;
using UnityEngine;

public class Enemy : MonoBehaviour {
    // Ray Casting
    [Header ("RayCasting")]
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float visionDistance = 12f;
    [SerializeField] private int rayCount = 10;
    [SerializeField] private LayerMask detectionMask;


    [SerializeField] private float rotationSpeed = 120f;


    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float wanderRotationSpeed = 1f;
    [SerializeField] private float wanderDirection;

    private bool isPlayerDetected;
    private bool isRotating;

    private Coroutine losePlayerCoroutine;

    private float enemySpeed;
    [SerializeField] private float detectionRange;
    [SerializeField] private float retreatDistanceMax;
    [SerializeField] private float retreatDistanceMin;

    private Transform player;
    private GameObject weapon;

    private void Awake() {
        detectionRange = 3f;

        enemySpeed = 3f;
        retreatDistanceMax = 5f;
        retreatDistanceMin = 3f;

        isPlayerDetected = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        // Player Detection
        if (isPlayerDetected) {
            MoveTowardsPlayer();

            if (!isRotating) {
                StartCoroutine(RotateTowardsPlayer());
            }

            if (weapon == null) {
                weapon = transform.GetChild(0).gameObject;
            }

            else if (weapon.CompareTag("Weapon")) {
                // Debug.Log("Enemy Shooting!");
                weapon.GetComponent<Weapon>().Shoot();
            }
        }

        else {
            Wander();
        }

    }

    private void FixedUpdate() {
   
        // Close Range Detection
        if (ComeTooClose()) {
            Debug.Log("Player Detected by coming too close!");
            isPlayerDetected = true;
        }

        else {
            ScanForPlayer();
        }
    }

    private bool ComeTooClose() {
        if (player == null) return false;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, detectionMask);
        return hit != null && hit.CompareTag("Player");
    }

    private void ScanForPlayer() {
        float halfAngle = visionAngle / 2f;
        bool localPlayerFound = false;

        for (int i = 0; i < rayCount; i++) {
            float angle = -halfAngle + (i * (visionAngle / (rayCount - 1)));
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionDistance, detectionMask);
            Debug.DrawRay(transform.position, direction * visionDistance, Color.red);


            if (hit.collider != null && hit.collider.CompareTag("Player")) {
                localPlayerFound = true;
                break; // stop once player is found
            }
        }

        if (localPlayerFound) {
            isPlayerDetected = true;
            if (losePlayerCoroutine != null) {
                StopCoroutine(losePlayerCoroutine);
                losePlayerCoroutine = null;
            }
        }

        else if (losePlayerCoroutine == null) {
            losePlayerCoroutine = StartCoroutine(LosePlayer(0.7f));
        }
    }
   

    private IEnumerator LosePlayer(float delay) {
        yield return new WaitForSeconds(delay);

        isPlayerDetected = false;
        losePlayerCoroutine = null;
    }

    private void Wander() {

        wanderDirection += (Random.value - 0.5f) * wanderSpeed;

        float angleDiff = wanderDirection - transform.eulerAngles.z * Mathf.Deg2Rad;

        while (angleDiff > Mathf.PI) angleDiff -= Mathf.PI * 2;
        while (angleDiff < -Mathf.PI) angleDiff += Mathf.PI * 2;

        if (Mathf.Abs(angleDiff) > wanderRotationSpeed) {
            transform.eulerAngles += new Vector3(0, 0, Mathf.Sign(angleDiff) * wanderRotationSpeed * Mathf.Rad2Deg);
        }
        else {
            transform.eulerAngles = new Vector3(0, 0, wanderDirection * Mathf.Rad2Deg);
        }

        transform.position += new Vector3(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) * wanderSpeed, Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad) * wanderSpeed, 0) * Time.deltaTime;

    }

    private void MoveTowardsPlayer() {
        // Retreat 
        if (player == null) {
            return;
        }

        if (Vector2.Distance(transform.position, player.position) < retreatDistanceMin) {
            transform.position = Vector2.MoveTowards(transform.position, player.position, -enemySpeed * Time.deltaTime);
        }

        else if (Vector2.Distance(transform.position, player.position) > retreatDistanceMax) {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemySpeed * Time.deltaTime);
        }
    }

    private IEnumerator RotateTowardsPlayer() {
        isRotating = true;

        Vector3 directionToPlayer = player.position - transform.position;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f) {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        isRotating = false;
    }

}
