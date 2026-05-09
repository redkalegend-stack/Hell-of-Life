using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour {
    [SerializeField] private GameObject bulletPrefab;

    private void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            Shoot();
        }
    }   

    private void Shoot() {
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
}
