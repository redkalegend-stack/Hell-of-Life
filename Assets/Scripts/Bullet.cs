using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour {
    private Rigidbody2D rb;

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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Building")) {
            Destroy(gameObject);
        }
    }
}
