using UnityEngine;

public class RootDisappearing : MonoBehaviour {
    private float fadeSpeed = 2f;
    private bool isPlayerNearby;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        isPlayerNearby = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (isPlayerNearby) {
            Color color = spriteRenderer.color;
            color.a = Mathf.MoveTowards(color.a, 0.4f, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = color;
        } else {
            Color color = spriteRenderer.color;
            color.a = Mathf.MoveTowards(color.a, 1f, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Player")) {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Player")) {
            isPlayerNearby = false;
        }
    }

}
