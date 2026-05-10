using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assemblies;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {
    public static GameManager main;

    [SerializeField] private List<GameObject> aliveEnemies;
    [SerializeField] private List<GameObject> deadEnemies;

    [SerializeField] private List<GameObject> enemySpawnPositionsOnHell;

    private Light2D light;

    [Header("UI Panels")]
    [SerializeField] private Sprite winPanel;
    [SerializeField] private Sprite losePanel;

    [SerializeField] private Image displayImage;
    [SerializeField] private Button restartButton;

    private void Awake() {
        if (main == null) {
            main = this;
        }
        else {
            Destroy(gameObject);
        }

        enemySpawnPositionsOnHell = new List<GameObject>(GameObject.FindGameObjectsWithTag("SpawnPosEnemy"));
        light = GetComponent<Light2D>();
    }

    private void Start() {
        // Debug.Log("Game Manager Started");

        displayImage.sprite = null;
        displayImage.enabled = false;
        restartButton.gameObject.SetActive(false);

        aliveEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        deadEnemies = new List<GameObject>();
    }

    private void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            SceneManager.LoadScene("Menu");
        }
    }

    public void ChangeToHell() {
        if (deadEnemies.Count == 0) {
            LoseGame();
        }

        else {
            MusicManager.PlayHellMusic();
            // Debug.Log("Changing to Hell...");
            light.intensity = 0.6f;

            var player = GameObject.FindGameObjectWithTag("Player");
            var spawnPosition = GameObject.FindGameObjectWithTag("Hell").transform.Find("SpawnPos").position;

            if (spawnPosition == null) {
                Debug.LogError("Spawn position not found for Hell!");
                return;
            }

            //Debug.Log("Spawn Position: " + spawnPosition);

            if (player != null) {
                player.transform.position = spawnPosition;
            }
        }

    }

    private void ChangeToLife() {
        // Debug.Log("Changing to Life...");
        MusicManager.PlayLifeMusic();  
        light.intensity = 1f;

        var player = GameObject.FindGameObjectWithTag("Player");
        var spawnPosition = GameObject.FindGameObjectWithTag("Life").transform.Find("SpawnPos").position;

        // Debug.Log("Spawn Position: " + spawnPosition);

        if (player != null) {
            player.transform.position = spawnPosition;
        }
    }


    public void KillEnemy(GameObject enemy) {
        Debug.Log("Killing Enemy...");

        // If enemy killed in Life, it goes to Hell
        if (aliveEnemies.Contains(enemy)) {
            aliveEnemies.Remove(enemy);
            deadEnemies.Add(enemy);

            enemy.transform.position = enemySpawnPositionsOnHell[Random.Range(0, enemySpawnPositionsOnHell.Count)].transform.position;

            CheckAliveEnemies();
        }

        // If enemy killed in Hell, it just dies
        else if (deadEnemies.Contains(enemy)) {
            deadEnemies.Remove(enemy);
            Destroy(enemy);


            CheckDeads();
        }
    }

    private void CheckAliveEnemies() {
        if (aliveEnemies.Count == 0) {
            WinGame();
        }
    }

    private void CheckDeads() {
        if (deadEnemies.Count == 0) {
            ChangeToLife();
        }
    }

    private void WinGame() {
        displayImage.sprite = winPanel;
        displayImage.enabled = false;
    }

    private void LoseGame() {
        displayImage.sprite = losePanel;
        displayImage.enabled = false;
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
