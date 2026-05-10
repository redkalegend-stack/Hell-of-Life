using System.Collections;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour {
    [SerializeField] private float cooldownTime; // Time between shots
    [SerializeField] private GameObject bulletPrefab;

    private bool canShoot;
    public bool isMusic;

    private void Awake() {
        canShoot = true;
    }
 
    public void Shoot() {
        if (isMusic) {
            MusicManager.PlayShootSound();
        }

        if (canShoot) {
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
