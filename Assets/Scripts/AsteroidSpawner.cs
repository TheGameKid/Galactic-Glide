using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public GameObject[] asteroidPrefabs;   // drag prefab assets (blue cubes)
    public Transform player;               // drag SpaceShuttle v1 here
    public bool aimAtPlayer = true;

    [Header("Center Tunnel Spawn")]
    public float spawnDepth = 25f;                  // closer so you see them sooner (was 55)
    public float spawnInterval = 0.10f;             // faster stream
    public Vector2 centerJitter = new Vector2(0.25f, 0.18f);

    [Header("Pass & Scatter")]
    public float behindPassDistance = 15f;          // how far behind camera they keep flying
    public Vector2 scatterAtPass = new Vector2(9f, 6f); // fan width/height behind camera
    public float extraConeSpreadDeg = 0f;           // leave 0; raise to 6–10 for more randomness

    [Header("Asteroid Variations")]
    public Vector2 speedRange = new Vector2(22f, 34f);
    public Vector2 uniformScaleRange = new Vector2(0.9f, 1.6f);

    [Header("Layer/Collision")]
    public string asteroidLayerName = "Asteroid";

    float timer;
    int asteroidLayer = -1;

    void Reset() { cam = Camera.main; }

    void Start()
    {
        if (!cam) cam = Camera.main;
        asteroidLayer = LayerMask.NameToLayer(
            LayerMask.NameToLayer(asteroidLayerName) != -1 ? asteroidLayerName :
            (LayerMask.NameToLayer("Asteroids") != -1 ? "Asteroids" : asteroidLayerName)
        );
    }

    void Update()
    {
        if (!cam || asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        Vector3 spawnPos = SpawnPosOnCenterLineWS();
        GameObject src = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        SpawnOne(spawnPos, src);
    }

    // --- spawn exactly on camera optical axis (tiny world jitter) ---
    Vector3 SpawnPosOnCenterLineWS()
    {
        Vector3 camSpace = new Vector3(0f, 0f, spawnDepth);
        Vector3 world = cam.transform.TransformPoint(camSpace);
        world += cam.transform.right * Random.Range(-centerJitter.x, centerJitter.x);
        world += cam.transform.up    * Random.Range(-centerJitter.y, centerJitter.y);
        return world;
    }

    void SpawnOne(Vector3 spawnPos, GameObject prefab)
    {
        if (!prefab) return;

        GameObject go = ExtractSingleRock(prefab, spawnPos, Random.rotation);
        if (!go) return;

        go.name = $"Asteroid_{prefab.name}_{Time.time}";

        // scale
        float scale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
        go.transform.localScale = Vector3.one * scale;

        // physics
        var rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // behaviour
        var asteroid = go.GetComponent<Asteroid>();
        if (!asteroid) asteroid = go.AddComponent<Asteroid>();

        // --- aim THROUGH the ship, then fan out behind the camera ---
        // base point to cross (ship if available, else exact screen center in front of camera)
        Vector3 crossPoint = (aimAtPlayer && player)
            ? player.position
            : cam.transform.position + cam.transform.forward * 2f;

        // plane BEHIND the camera for the fan
        Vector3 passPlaneCenter = cam.transform.TransformPoint(0f, 0f, -behindPassDistance);

        // start from the cross point and push it behind camera, then add spread
        Vector3 basePassPoint = crossPoint - cam.transform.forward * behindPassDistance;
        Vector3 spread =
            cam.transform.right * Random.Range(-scatterAtPass.x, scatterAtPass.x) +
            cam.transform.up    * Random.Range(-scatterAtPass.y, scatterAtPass.y);

        Vector3 finalTarget = basePassPoint + spread;

        // direction
        Vector3 dir = (finalTarget - spawnPos).normalized;

        if (extraConeSpreadDeg > 0f)
        {
            Vector2 ang = Random.insideUnitCircle * extraConeSpreadDeg;
            dir = (Quaternion.AngleAxis(ang.x, cam.transform.up) *
                   Quaternion.AngleAxis(ang.y, cam.transform.right) * dir).normalized;
        }

        asteroid.direction = dir;
        asteroid.speed = Random.Range(speedRange.x, speedRange.y);
        asteroid.maxLifetime = 90f;    // long; we handle culling conservatively

        // layer/trigger
        if (asteroidLayer != -1) SetLayerRecursive(go.transform, asteroidLayer);
        foreach (var c in go.GetComponentsInChildren<Collider>(true)) c.isTrigger = true;
    }

    // Build a one-mesh asteroid from any prefab (belt or single); add SphereCollider(trigger)
    GameObject ExtractSingleRock(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        GameObject temp = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        var parts = temp.GetComponentsInChildren<MeshRenderer>(true);
        if (parts == null || parts.Length == 0) { Destroy(temp); return null; }

        var src = parts[Random.Range(0, parts.Length)].gameObject;
        var mfSrc = src.GetComponent<MeshFilter>();
        var mrSrc = src.GetComponent<MeshRenderer>();
        if (!mfSrc || !mrSrc || !mfSrc.sharedMesh) { Destroy(temp); return null; }

        GameObject go = new GameObject("Asteroid_Single");
        go.transform.SetPositionAndRotation(pos, rot);

        var mf = go.AddComponent<MeshFilter>();    mf.sharedMesh = mfSrc.sharedMesh;
        var mr = go.AddComponent<MeshRenderer>();  mr.sharedMaterials = mrSrc.sharedMaterials;

        // simple trigger collider (avoids convex-256 warnings)
        var b = mfSrc.sharedMesh.bounds;
        var sc = go.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = Mathf.Max(b.extents.x, b.extents.y, b.extents.z);
        sc.center = b.center;

        Destroy(temp);
        return go;
    }

    static void SetLayerRecursive(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++) SetLayerRecursive(t.GetChild(i), layer);
    }
}
