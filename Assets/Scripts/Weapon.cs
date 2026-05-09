using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour {
    [SerializeField] private float cooldownTime = 1f; // Time between shots
    [SerializeField] private GameObject bulletPrefab;

    private bool canShoot;

    private void Awake() {
        canShoot = true;
    }
 
    public void Shoot() {
        if (canShoot) {
            Debug.Log("Shooting!");
            Instantiate(bulletPrefab, transform.position, transform.rotation);
            StartCoroutine(ResetShoot());
        }
    }

    private IEnumerator ResetShoot() {
        canShoot = false;
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }
}
