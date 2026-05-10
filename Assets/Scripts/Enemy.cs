using System.Collections;
using System.Runtime.CompilerServices;
using Unity.AppUI.Core;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float visionDistance = 12f;
    [SerializeField] private int rayCount = 10;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float rotationSpeed = 120f;

    private bool isPlayerDetected;
    private bool isRotating;

    private Coroutine losePlayerCoroutine;

    private float enemySpeed;
    [SerializeField] private float detectionRange;
    [SerializeField] private float keepDistance; // Distance to maintain from the player when detected
    [SerializeField] private float retreatDistanceMax;
    [SerializeField] private float retreatDistanceMin;

    private Transform player;
    private GameObject weapon;

    private void Awake() {
        enemySpeed = 3f;
        retreatDistanceMax = 5f;
        retreatDistanceMin = 3f;

        isPlayerDetected = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        // Move towards player
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

        // Close Range Detection
        else if (Vector2.Distance(transform.position, player.position) < detectionRange) {
            Debug.Log("Player Detected by coming too close!");
            isPlayerDetected = true;
        }

        else {
            ScanForPlayer();
        }
    }

    private void ScanForPlayer() {
        float halfAngle = visionAngle / 2f;

        for (int i = 0; i < rayCount; i++) {
            float angle = -halfAngle + (i * (visionAngle / (rayCount - 1)));
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.right;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionDistance, detectionMask);
            Debug.DrawRay(transform.position, direction * visionDistance, Color.red);


            if (hit.collider != null && hit.collider.CompareTag("Player")) {
                isPlayerDetected = true;
                break; // stop once player is found
            }
        }

        if (isPlayerDetected) {
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
