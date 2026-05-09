using System.Collections;
using System.Runtime.CompilerServices;
using Unity.AppUI.Core;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float visionDistance = 10f;
    [SerializeField] private int rayCount = 10;

    private bool isPlayerDetected;

    private float enemySpeed;
    [SerializeField] private float detectionRange;
    [SerializeField] private float retreatDistanceMax;
    [SerializeField] private float retreatDistanceMin;

    private Transform player;

    private void Awake() {
        enemySpeed = 5f;
        detectionRange = 2f;
        retreatDistanceMax = 5f;
        retreatDistanceMin = 3f;

        isPlayerDetected = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        ScanForPlayer();

        // Move towards player
        if (isPlayerDetected) {
            // Retreat 
            if (Vector2.Distance(transform.position, player.position) < retreatDistanceMin) {
                transform.position = Vector2.MoveTowards(transform.position, player.position, -enemySpeed * Time.deltaTime);
            }

            else if (Vector2.Distance(transform.position, player.position) > retreatDistanceMax) { 
                transform.position = Vector2.MoveTowards(transform.position, player.position, enemySpeed * Time.deltaTime);
            }

            StartCoroutine(RotateTowardsPlayer());

            GameObject weapon = transform.GetChild(0).gameObject;
            if (weapon != null && weapon.CompareTag("Weapon")) {
                // Debug.Log("Enemy Shooting!");
                weapon.GetComponent<Weapon>().Shoot();
            }
        }

        // Close Range Detection
        if (Vector2.Distance(transform.position, player.position) < detectionRange) {
            Debug.Log("Player Detected by coming too close!");
            isPlayerDetected = true;
        }

    }


    private void ScanForPlayer() {
        float halfAngle = visionAngle / 2f;

        for (int i = 0; i < rayCount; i++) {
            float angle = -halfAngle + (i * (visionAngle / (rayCount - 1)));
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.right;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionDistance);
            //Debug.DrawRay(transform.position, direction * visionDistance, Color.red);

            if (hit.collider != null && hit.collider.CompareTag("Player")) {
                isPlayerDetected = true;
            }
        }
    }

    private IEnumerator RotateTowardsPlayer() {
        Vector3 directionToPlayer = player.position - transform.position;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float currentAngle = transform.eulerAngles.z;
        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f) {
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, enemySpeed * Time.deltaTime * 100);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }
    }

}
