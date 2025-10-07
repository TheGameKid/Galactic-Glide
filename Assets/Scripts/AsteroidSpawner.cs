using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public GameObject[] asteroidPrefabs;      // drag your asteroid prefabs here
    public Transform player;
    public bool aimAtPlayer = false;

    [Header("Prefab Settings")]
    [Tooltip("Set true if your prefabs are big groups like asteroid_belt_group_SRP_01")]
    public bool prefabsAreGroups = true;
    [Tooltip("When using group prefabs, spawn single rocks extracted from the group")]
    public bool extractIndividualAsteroids = true;

    [Header("Spawning")]
    public float spawnInterval = 0.8f;
    public float spawnDepthOffset = 20f;      // distance in front of camera
    public Vector2 viewportSpread = new Vector2(10f, 8f); // world-unit spread

    [Header("Group Spawning")]
    [Range(0f, 1f)] public float groupSpawnChance = 0.25f;
    public Vector2Int groupSizeRange = new Vector2Int(2, 3);
    public float groupSpread = 8f;
    public Vector2 groupPositionVariation = new Vector2(4f, 3f);

    [Header("Asteroid Variations")]
    public Vector2 speedRange = new Vector2(9f, 16f);
    public Vector2 uniformScaleRange = new Vector2(0.6f, 1.8f);

    [Header("Layer/Collision")]
    [Tooltip("Must match your Physics layer name. Use \"Asteroid\" or \"Asteroids\".")]
    public string asteroidLayerName = "Asteroid";

    float timer;
    int asteroidLayer = -1;

    void Reset() { cam = Camera.main; }

    void Start()
    {
        if (!cam) cam = Camera.main;

        // support both singular/plural layer names
        asteroidLayer = LayerMask.NameToLayer(asteroidLayerName);
        if (asteroidLayer == -1) asteroidLayer = LayerMask.NameToLayer("Asteroids");
    }

    void Update()
    {
        if (!cam || asteroidPrefabs == null || asteroidPrefabs.Length == 0) return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        if (prefabsAreGroups && extractIndividualAsteroids)
        {
            if (Random.value < groupSpawnChance) SpawnCustomGroup();
            else SpawnIndividualFromGroup();
        }
        else if (prefabsAreGroups)
        {
            SpawnSingle();
        }
        else
        {
            if (Random.value < groupSpawnChance) SpawnGroup();
            else SpawnSingle();
        }
    }

    void SpawnSingle()
    {
        var prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        SpawnAsteroidAt(GetRandomSpawnPosition(), prefab);
    }

    void SpawnGroup()
    {
        int groupSize = Random.Range(groupSizeRange.x, groupSizeRange.y + 1);
        Vector3 basePos = GetRandomSpawnPosition();

        for (int i = 0; i < groupSize; i++)
        {
            var prefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
            Vector3 offset = new Vector3(
                Random.Range(-groupPositionVariation.x, groupPositionVariation.x),
                Random.Range(-groupPositionVariation.y, groupPositionVariation.y),
                Random.Range(-groupSpread * 0.5f, groupSpread * 0.5f)
            );
            SpawnAsteroidAt(basePos + offset, prefab);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 basePos = cam.transform.position + cam.transform.forward * spawnDepthOffset;
        float sx = Random.Range(-viewportSpread.x, viewportSpread.x);
        float sy = Random.Range(-viewportSpread.y, viewportSpread.y);
        return basePos + cam.transform.right * sx + cam.transform.up * sy;
    }

    void SpawnAsteroidAt(Vector3 spawnPosition, GameObject prefab)
    {
        if (!prefab) return;

        GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity);
        go.name = $"Asteroid_{prefab.name}_{Time.time}";

        float scale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
        go.transform.localScale = Vector3.one * scale;

        // Rigidbody
        var rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Asteroid behaviour
        var asteroid = go.GetComponent<Asteroid>();
        if (!asteroid) asteroid = go.AddComponent<Asteroid>();
        Vector3 dir = (aimAtPlayer && player)
            ? (player.position - spawnPosition).normalized
            : -cam.transform.forward;
        asteroid.direction = dir;
        asteroid.speed = Random.Range(speedRange.x, speedRange.y);
        asteroid.maxLifetime = 30f;

        // Layer + triggers so they never physically block
        ApplyLayerAndTriggers(go);
    }

    void SpawnIndividualFromGroup()
    {
        var groupPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        Vector3 spawnPos = GetRandomSpawnPosition();

        GameObject temp = Instantiate(groupPrefab, Vector3.zero, Quaternion.identity);
        var pieces = temp.GetComponentsInChildren<MeshRenderer>(true);

        if (pieces.Length > 0)
        {
            var piece = pieces[Random.Range(0, pieces.Length)].gameObject;

            GameObject go = new GameObject($"IndividualAsteroid_{Time.time}");
            go.transform.SetPositionAndRotation(spawnPos, Random.rotation);

            var srcMF = piece.GetComponent<MeshFilter>();
            var srcMR = piece.GetComponent<MeshRenderer>();
            if (srcMF && srcMR)
            {
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = srcMF.sharedMesh;

                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterials = srcMR.sharedMaterials;

                var mc = go.AddComponent<MeshCollider>();
                mc.sharedMesh = srcMF.sharedMesh;
                mc.convex = true;
                mc.isTrigger = true;
            }

            float scale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
            go.transform.localScale = Vector3.one * scale;

            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var asteroid = go.AddComponent<Asteroid>();
            asteroid.direction = (aimAtPlayer && player)
                ? (player.position - spawnPos).normalized
                : -cam.transform.forward;
            asteroid.speed = Random.Range(speedRange.x, speedRange.y);
            asteroid.maxLifetime = 30f;

            ApplyLayerAndTriggers(go);
        }
        else
        {
            // fallback to spawn the full group
            SpawnAsteroidAt(spawnPos, groupPrefab);
        }

        Destroy(temp);
    }

    void SpawnCustomGroup()
    {
        int groupSize = Random.Range(groupSizeRange.x, groupSizeRange.y + 1);
        Vector3 basePos = GetRandomSpawnPosition();

        for (int i = 0; i < groupSize; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-groupPositionVariation.x, groupPositionVariation.x),
                Random.Range(-groupPositionVariation.y, groupPositionVariation.y),
                Random.Range(-groupSpread * 0.5f, groupSpread * 0.5f)
            );
            SpawnIndividualFromGroupAt(basePos + offset);
        }
    }

    void SpawnIndividualFromGroupAt(Vector3 spawnPos)
    {
        var groupPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];

        GameObject temp = Instantiate(groupPrefab, Vector3.zero, Quaternion.identity);
        var pieces = temp.GetComponentsInChildren<MeshRenderer>(true);

        if (pieces.Length > 0)
        {
            var piece = pieces[Random.Range(0, pieces.Length)].gameObject;

            GameObject go = new GameObject($"GroupAsteroid_{Time.time}_{Random.Range(0, 1000)}");
            go.transform.SetPositionAndRotation(spawnPos, Random.rotation);

            var srcMF = piece.GetComponent<MeshFilter>();
            var srcMR = piece.GetComponent<MeshRenderer>();
            if (srcMF && srcMR)
            {
                var mf = go.AddComponent<MeshFilter>();
                mf.sharedMesh = srcMF.sharedMesh;

                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterials = srcMR.sharedMaterials;

                var mc = go.AddComponent<MeshCollider>();
                mc.sharedMesh = srcMF.sharedMesh;
                mc.convex = true;
                mc.isTrigger = true;
            }

            float scale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
            go.transform.localScale = Vector3.one * scale;

            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var asteroid = go.AddComponent<Asteroid>();
            asteroid.direction = (aimAtPlayer && player)
                ? (player.position - spawnPos).normalized
                : -cam.transform.forward;
            asteroid.speed = Random.Range(speedRange.x, speedRange.y);
            asteroid.maxLifetime = 30f;

            ApplyLayerAndTriggers(go);
        }

        Destroy(temp);
    }

    // ----- helpers -----

    void ApplyLayerAndTriggers(GameObject go)
    {
        if (asteroidLayer != -1) SetLayerRecursive(go.transform, asteroidLayer);
        foreach (var col in go.GetComponentsInChildren<Collider>(true))
            col.isTrigger = true; // never physically block
    }

    static void SetLayerRecursive(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++) SetLayerRecursive(t.GetChild(i), layer);
    }
}
