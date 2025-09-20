using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public GameObject asteroidPrefab;
    public Transform player;
    public bool aimAtPlayer = false;

    [Header("Spawning")]
    public float spawnInterval = 0.35f;
    public float spawnDepthOffset = 50f; // distance from camera
    public Vector2 viewportSpread = new Vector2(0.04f, 0.02f);

    [Header("Asteroid Variations")]
    public Vector2 speedRange = new Vector2(9f, 16f);
    public Vector2 uniformScaleRange = new Vector2(0.6f, 1.8f);

    float timer;

    void Reset()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (!cam || !asteroidPrefab) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        // 1) FORCE center of the Game view, at a safe distance in front of camera
        float z = 30f;  // distance along camera forward
        Vector3 vp = new Vector3(0.5f, 0.5f, z);
        Vector3 spawnWorld = cam.ViewportToWorldPoint(vp);

        // 2) ALSO drop a Unity Sphere so we can SEE the spawn point even if prefab is broken
        var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = spawnWorld;
        marker.transform.localScale = Vector3.one * 1.2f; // big marker
        Destroy(marker, 5f); // auto-cleanup

        // 3) Instantiate your asteroid prefab at that exact spot, huge scale
        GameObject go = Instantiate(asteroidPrefab, spawnWorld, Quaternion.identity);
        go.transform.localScale = Vector3.one * 1.0f;

        // 4) Ensure Rigidbody exists + no gravity
        var rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        // 5) Ensure mover script exists and make it move slowly toward camera
        var mover = go.GetComponent<Asteroid>();
        if (!mover) mover = go.AddComponent<Asteroid>();
        mover.direction = -cam.transform.forward;  // TOWARD camera
        mover.speed = 4f;                         
        mover.maxLifetime = 15f;  

        // Debug info
        var vpNow = cam.WorldToViewportPoint(spawnWorld);
        Debug.Log($"[SpawnOne] VP={vpNow}  World={spawnWorld}  CamForward={cam.transform.forward}");
    }

}