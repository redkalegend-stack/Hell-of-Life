using UnityEngine;

public class Bullet : MonoBehaviour {
    private Rigidbody2D rb;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask wallLayer;

    private float bulletSpeed = 100f;
    private float bulletLifetime = 0.1f;
    private Vector3 lastPosition;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        rb.linearVelocity = transform.right * bulletSpeed;
        lastPosition = transform.position;
        Destroy(gameObject, bulletLifetime);
    }

    private void Update() {
        // Raycast from last position to current position
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - lastPosition;
        float distance = direction.magnitude;

        if (distance > 0) {
            // Check for hits along the path
            RaycastHit2D hit = Physics2D.Raycast(lastPosition, direction.normalized, distance, enemyLayer | wallLayer);

            if (hit.collider != null) {
                // Check if it's an enemy
                if (((1 << hit.collider.gameObject.layer) & enemyLayer) != 0) {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                        GameManager.main.KillEnemy(hit.collider.gameObject);
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                        GameManager.main.ChangeToHell();
                    }
                    Destroy(gameObject);
                }
                // Check if it's a wall
                else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall")) {
                    Destroy(gameObject);
                }
            }
        }

        lastPosition = currentPosition;
        transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
    }
}