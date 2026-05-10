using UnityEngine;

public class MusicManager : MonoBehaviour {
    private static MusicManager instance;

    [SerializeField] private AudioClip lifeMusic;
    [SerializeField] private AudioClip hellMusic;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip dieSound;

    [SerializeField] private float volume = 0.5f;

    private AudioSource audioSource;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = volume;
            audioSource.loop = true;
        }
        else {
            Destroy(gameObject);
        }
    }

    public static void PlayShootSound() {
        if (instance != null && instance.shootSound != null) {
            instance.audioSource.PlayOneShot(instance.shootSound);
        }
    }

    public static void PlayLifeMusic() {
        if (instance != null && instance.hellMusic != null) {
            instance.audioSource.clip = instance.lifeMusic;
            instance.audioSource.Play();
        }
    }

    public static void PlayHellMusic() {
        if (instance != null && instance.hellMusic != null) {
            instance.audioSource.clip = instance.hellMusic;
            instance.audioSource.Play();
        }
    }
}