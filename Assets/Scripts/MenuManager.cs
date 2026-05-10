using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public void StartGame() {
        MusicManager.PlayLifeMusic();
        SceneManager.LoadScene("Game");
    }
    public void QuitGame() {
        Application.Quit();
    }
}
