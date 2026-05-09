using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour {
    [SerializeField] private Transform player;
    [SerializeField] private float smoothSpeed = 5f;
    private Vector3 offset;

    [Header("Look Ahead Settings")]
    [SerializeField] public bool useLookAhead;
    [SerializeField] private float lookAheadDistance;
    [SerializeField] private float lookAheadSpeed;

    private Vector3 currentLookAhead;

    private void Awake() {
        offset = new Vector3(0, 0, -10);

        smoothSpeed = 10f;
        lookAheadDistance = 1;
        lookAheadSpeed = 5;
    }

    private void Update() {
        
    }


    private void LateUpdate() {

        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;

        if (useLookAhead) {
            Vector3 targetLookAhead = player.right * lookAheadDistance;
            currentLookAhead = Vector3.Lerp(currentLookAhead, targetLookAhead, lookAheadSpeed * Time.deltaTime);
            desiredPosition += currentLookAhead;

        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // transform.LookAt(player);
    }
}

