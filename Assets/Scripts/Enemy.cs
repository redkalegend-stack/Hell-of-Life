using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private GameObject weapon;
    private Rigidbody2D rb;
    [SerializeField] private EnemySettings enemySettings;

    [Header("RayCasting")]
    [SerializeField] private float visionAngle = 45f;
    [SerializeField] private float visionDistance = 12f;
    [SerializeField] private int rayCount = 10;
    [SerializeField] private LayerMask wallLayer; // Assign "Wall" layer in Inspector
    [SerializeField] private LayerMask playerLayer; // Assign "Player" layer in Inspector
    private bool isPlayerDetected;

    [Header("Alert")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Wandering")]
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float wanderRotationSpeed = 60f;
    private float wanderDirection;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 3f;
    [SerializeField] private float retreatDistanceMax = 8f;
    [SerializeField] private float retreatDistanceMin = 5f;

    // Rotation
    private bool isRotating;
    private Coroutine losePlayerCoroutine;

    private void Awake() {
        isPlayerDetected = false;
        rb = GetComponent<Rigidbody2D>();

        Initialize(enemySettings);
    }

    private void Initialize(EnemySettings s) {
        visionAngle = s.visionAngle;
        visionDistance = s.visionDistance;
        rayCount = s.rayCount;
        wallLayer = s.wallLayer;
        playerLayer = s.playerLayer;    

        speed = s.speed;
        rotationSpeed = s.rotationSpeed;

        wanderSpeed = s.wanderSpeed;
        wanderRotationSpeed = s.wanderRotationSpeed;

        detectionRange = s.detectionRange;
        retreatDistanceMax = s.retreatDistanceMax;
        retreatDistanceMin = s.retreatDistanceMin;
    }

    private void Update() {

        if (isPlayerDetected) {
            Transform player = GetPlayerTransform();
            if (player == null) return;

            if (!isRotating) {
                StartCoroutine(RotateTowardsPlayer(player));
            }

            if (weapon == null) {
                weapon = transform.GetChild(0).gameObject;
            }
            else if (weapon.CompareTag("Weapon")) {
                var weaponComponent = weapon.GetComponent<Weapon>();
                if (weaponComponent != null) {
                    weaponComponent.Shoot();
                }
            }
        }
    }

    private void FixedUpdate() {
        // Movement
        if (isPlayerDetected) {
            Transform player = GetPlayerTransform();
            if (player != null) {
                MoveTowardsPlayer(player);
            }
        }
        else if (ComeTooClose()) {
            Debug.Log("Player Detected by coming too close!");
            isPlayerDetected = true;
        }
        else {
            Wander();
        }
        ScanForPlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // When hitting a wall, rotate randomly between 120-240 degrees
        if (collision.gameObject.CompareTag("Wall")) {
            float randomRotation = Random.Range(120f, 240f);
            wanderDirection = transform.eulerAngles.z + randomRotation;
            wanderDirection = Mathf.Repeat(wanderDirection, 360f);
        }
    }

    private Transform GetPlayerTransform() {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        return playerObj != null ? playerObj.transform : null;
    }

    private bool ComeTooClose() {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange);
        return hit != null && hit.CompareTag("Player");
    }

    private void ScanForPlayer() {
        float halfAngle = visionAngle / 2f;
        bool localPlayerFound = false;

        // Combined layer mask for both walls and player
        LayerMask combinedMask = wallLayer | playerLayer;

        for (int i = 0; i < rayCount; i++) {
            float angle = -halfAngle + (i * (visionAngle / (rayCount - 1)));
            Vector3 direction = Quaternion.Euler(0, 0, angle) * transform.right;

            // Cast ray with layer mask to detect both walls and player
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionDistance, combinedMask);

            if (hit.collider != null) {
                // If we hit a wall first, stop this ray
                if (((1 << hit.collider.gameObject.layer) & wallLayer) != 0) {
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.yellow);
                    continue; // Check next ray
                }

                // If we hit the player and no wall was in the way
                if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0) {
                    Debug.DrawRay(transform.position, direction * hit.distance, Color.green);
                    localPlayerFound = true;
                    break;
                }
            }
            else {
                // No hit - draw red ray
                Debug.DrawRay(transform.position, direction * visionDistance, Color.red);
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
        // Calculate target wander direction with random variation
        wanderDirection += (Random.value - 0.5f) * Time.fixedDeltaTime * 100f;
        wanderDirection = Mathf.Repeat(wanderDirection, 360f);

        // Smoothly rotate towards wander direction using degrees/sec
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, wanderDirection, wanderRotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        // Move in the direction we're facing
        Vector2 moveDirection = new Vector2(
            Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad),
            Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad)
        );

        rb.linearVelocity = moveDirection * wanderSpeed;
    }

    private void MoveTowardsPlayer(Transform player) {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < retreatDistanceMin) {
            rb.linearVelocity = -directionToPlayer * speed;
        }
        else if (distanceToPlayer > retreatDistanceMax) {
            rb.linearVelocity = directionToPlayer * speed;
        }
        else {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator RotateTowardsPlayer(Transform player) {
        isRotating = true;

        while (isPlayerDetected && player != null) {
            Vector3 directionToPlayer = player.position - transform.position;
            float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;

            // Use the same rotation speed system as wandering (degrees/sec)
            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f) {
                currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                yield return null;
            }
            else {
                break;
            }
        }

        isRotating = false;
    }
}