using UnityEngine;

public class SpaceJunkSpawner : MonoBehaviour
{
    [Header("What to spawn (drag prefabs here)")]
    public GameObject[] junkPrefabs;

    [Header("How many")]
    public int count = 16;

    [Header("Placement")]
    public Transform player;
    public float minDistanceFromPlayer = 3f;
    public float z = -3.9123f;

    [Header("Auto configure spawned objects")]
    public bool addDrifterIfMissing = true;
    public bool forceLayerToSpaceJunk = true;

    Camera cam;

    void Start()
    {
        if (junkPrefabs == null || junkPrefabs.Length == 0)
        {
            Debug.LogWarning("[SpaceJunkSpawner] No prefabs assigned.");
            return;
        }

        cam = Camera.main;
        float d = Mathf.Abs(z - cam.transform.position.z);
        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0, 0, d));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1, 1, d));

        for (int i = 0; i < count; i++)
        {
            Vector3 p;
            int tries = 0;
            do
            {
                p = new Vector3(Random.Range(bl.x, tr.x), Random.Range(bl.y, tr.y), z);
                tries++;
            }
            while (player && Vector3.Distance(p, player.position) < minDistanceFromPlayer && tries < 64);

            var prefab = junkPrefabs[Random.Range(0, junkPrefabs.Length)];
            var go = Instantiate(prefab, p, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));

            // add drifter automatically if missing
            if (addDrifterIfMissing && !go.GetComponent<SpaceJunkDrifter>())
            {
                go.AddComponent<SpaceJunkDrifter>();
            }

            // ensure layer is SpaceJunk (if it exists)
            if (forceLayerToSpaceJunk)
            {
                int lj = LayerMask.NameToLayer("SpaceJunk");
                if (lj != -1) SetLayerRecursive(go.transform, lj);
            }
        }
    }

    static void SetLayerRecursive(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++) SetLayerRecursive(t.GetChild(i), layer);
    }
}
