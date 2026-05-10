using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager main;

    [SerializeField] private List<GameObject> aliveEnemies;
    [SerializeField] private List<GameObject> deadEnemies;

    [SerializeField] private List<GameObject> enemySpawnPositionsOnHell;

    private void Awake() {
        if (main == null) {
            main = this;
        }
        else {
            Destroy(gameObject);
        }

        enemySpawnPositionsOnHell = new List<GameObject>(GameObject.FindGameObjectsWithTag("SpawnPosEnemy"));
    }

    private void Start() {
        Debug.Log("Game Manager Started");

        aliveEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        deadEnemies = new List<GameObject>();
    }

    public void ChangeToHell() {
        Debug.Log("Changing to Hell...");

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

    private void ChangeToLife() {
        Debug.Log("Changing to Life...");

        var player = GameObject.FindGameObjectWithTag("Player");
        var spawnPosition = GameObject.FindGameObjectWithTag("Life").transform.Find("SpawnPos").position;

        Debug.Log("Spawn Position: " + spawnPosition);

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
        }

        // If enemy killed in Hell, it just dies
        else if (deadEnemies.Contains(enemy)) {
            deadEnemies.Remove(enemy);
            CheckDeads();

            Destroy(enemy);
        }
    }

    private void CheckDeads() {
        if (deadEnemies.Count == 0) {
            ChangeToLife();
        }
    }
}
