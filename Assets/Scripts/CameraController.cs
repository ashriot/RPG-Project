using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour {

    public static CameraController instance;
    
    public Transform target;

    public Tilemap tilemap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;

    private float halfHeight;
    private float halfWidth;

    public string musicNameToPlay;
    private bool musicStarted = false;

    void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController == null) { return; }
        target = playerController.transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        bottomLeftLimit = tilemap.localBounds.min + new Vector3(halfWidth, halfHeight, 0f);
        topRightLimit = tilemap.localBounds.max + new Vector3(-halfWidth, -halfHeight, 0f);

        playerController.SetBounds(tilemap.localBounds.min, tilemap.localBounds.max);
    }

    // LateUpdate is called once per frame after Update
    void LateUpdate() {
        // this.transform.position = new Vector3(target.position.x, target.position.y, this.transform.position.z);

        // // keep the camera inside the bounds
        // transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x),
        // Mathf.Clamp(transform.position.y, bottomLeftLimit.y,
        // topRightLimit.y), transform.position.z);

        if (!musicStarted) {
            AudioManager.instance.PlayBgm(musicNameToPlay);
            musicStarted = true;
        }
    }
}
