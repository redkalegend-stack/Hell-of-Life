using UnityEngine;

public class Bullet : MonoBehaviour {
    private float bulletSpeed = 50f;
    private float bulletLifetime = 0.2f;

    private void Start() {
        Destroy(gameObject, bulletLifetime);
    }

    private void Update() {
        transform.Translate(Vector2.right * Time.deltaTime * bulletSpeed, Space.Self);
    }
}
