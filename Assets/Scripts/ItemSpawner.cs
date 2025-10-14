using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public GameObject[] itemPrefabs;

    [Header("Spawn Timing")]
    public float spawnEvery = 1.0f;

    [Header("Spawn Region (screen-relative)")]
    [Range(0f, 1f)] public float minViewport = 0.40f;
    [Range(0f, 1f)] public float maxViewport = 0.60f;

    [Header("Extra Padding")]
    [Tooltip("How much to shrink the viewport box on each side (0..0.5).")]
    [Range(0f, 0.5f)] public float viewportPadding = 0.08f;
    [Tooltip("Extra world-space margin from the camera edges after spawning (units).")]
    public float worldPadding = 0.25f;

    [Header("Depth From Camera")]
    public float spawnDepthFromCamera = 40f;

    [Header("Item Motion")]
    public Vector2 speedRange = new Vector2(6f, 10f);

    private float timer;

    public Player player;

    float delay = 5f;
    bool isDelay = false;

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        if (!cam || itemPrefabs == null || itemPrefabs.Length == 0) return;

        if (isDelay == false)
        {
            timer += Time.deltaTime;
            delay = 0f;
        }
        else
        {
            delay -= Time.deltaTime;
            if (delay <= 0f)
            {
                delay = 0f;
                isDelay = false;
            }
        }

        if (timer >= spawnEvery)
        {
            timer = 0f;
            isDelay = true;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        GameObject prefab = null;

        if (player.shield.activeInHierarchy)
        {
            prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length - 1)];
        }
        else if (player.laserAmmo > 0)
        {
            int random = Random.Range(0, 2);
            if (random == 0)
            {
                prefab = itemPrefabs[0];
            }
            if (random == 1)
            {
                prefab = itemPrefabs[2];
            }
        }
        else
        {
            prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        }

        if (!prefab) return;

        // --- Viewport padding: shrink the allowed UV box ---
        float a = Mathf.Min(minViewport, maxViewport);
        float b = Mathf.Max(minViewport, maxViewport);

        float minU = Mathf.Clamp01(a + viewportPadding);
        float maxU = Mathf.Clamp01(b - viewportPadding);
        float minV = minU;  // keep same padding vertically as horizontally
        float maxV = maxU;

        // Fallback if padding collapses the range
        if (maxU <= minU) { minU = 0.48f; maxU = 0.52f; }
        if (maxV <= minV) { minV = 0.48f; maxV = 0.52f; }

        float u = Random.Range(minU, maxU);
        float v = Random.Range(minV, maxV);

        Vector3 spawnPos = cam.ViewportToWorldPoint(new Vector3(u, v, spawnDepthFromCamera));
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);

        // --- World padding: clamp away from camera edges in world units ---
        if (cam.orthographic)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            Vector3 c = cam.transform.position;

            Vector3 p = go.transform.position;
            p.x = Mathf.Clamp(p.x, c.x - halfW + worldPadding, c.x + halfW - worldPadding);
            p.y = Mathf.Clamp(p.y, c.y - halfH + worldPadding, c.y + halfH - worldPadding);
            go.transform.position = p;
        }
        // (Perspective doesn’t need this clamp usually, but you can keep it if desired by projecting edges.)

        var item = go.GetComponent<Item>();
        if (!item) item = go.AddComponent<Item>();

        item.speed = Random.Range(speedRange.x, speedRange.y);
        item.direction = -cam.transform.forward.normalized;
    }
}


