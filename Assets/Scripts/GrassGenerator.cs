using UnityEngine;

public class GrassGenerator : MonoBehaviour {
    [SerializeField] private GameObject grassPrefab;

    private void Start() {
        for (int i = 0; i < 50; i++) {
            for (int j = 0; j < 50; j++) {
                Vector3 position = new Vector3(i - 25, j - 25, 0);
                Instantiate(grassPrefab, position, Quaternion.identity);
            }   

        }
    }

}
