using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private Rigidbody2D rb;
    [SerializeField] private LayerMask enemyLayer;

    private float bulletSpeed = 100f;
    private float bulletLifetime = 0.1f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        rb.linearVelocity = transform.right * bulletSpeed;

        Destroy(gameObject, bulletLifetime);
    }

    private void Update() {
        transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0) {
            Destroy(gameObject);

            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                GameManager.main.KillEnemy(collision.gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
                GameManager.main.ChangeToHell();
            }
        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Building")) {
            // Debug.Log("Bullet hit a building!");
            Destroy(gameObject);
        }
    }
}
