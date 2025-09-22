using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public GameObject[] asteroidPrefabs; // Array of different asteroid prefabs
    public Transform player;
    public bool aimAtPlayer = false;
    
    [Header("Prefab Settings")]
    [Tooltip("Check this if your prefabs are already groups of asteroids")]
    public bool prefabsAreGroups = true; // Set to true if using group prefabs like asteroid_belt_group_SRP_01
    [Tooltip("When using group prefabs, extract individual asteroids for single spawning")]
    public bool extractIndividualAsteroids = true;

    [Header("Spawning")]
    public float spawnInterval = 0.8f; // Balanced for mixed single/group spawning
    public float spawnDepthOffset = 20f; // distance from camera (reduced from 50f)
    public Vector2 viewportSpread = new Vector2(10f, 8f); // world units spread (increased for visibility)

    [Header("Group Spawning")]
    [Range(0f, 1f)]
    public float groupSpawnChance = 0.25f; // 25% chance for full groups, 75% for individual asteroids
    public Vector2Int groupSizeRange = new Vector2Int(2, 3); // Max 3 asteroids per group
    public float groupSpread = 8f; // Increased from 5f - How spread out the group formation is
    public Vector2 groupPositionVariation = new Vector2(4f, 3f); // Increased variation

    [Header("Asteroid Variations")]
    public Vector2 speedRange = new Vector2(9f, 16f);
    public Vector2 uniformScaleRange = new Vector2(0.6f, 1.8f);

    float timer;

    void Reset()
    {
        cam = Camera.main;
    }

    void Start()
    {
        // Initialize camera if not already set
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (!cam)
        {
            Debug.LogWarning("[AsteroidSpawner] No camera assigned!");
            return;
        }
        
        if (asteroidPrefabs == null || asteroidPrefabs.Length == 0)
        {
            Debug.LogWarning("[AsteroidSpawner] No asteroid prefabs assigned! Please assign asteroid prefabs in the Inspector.");
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            
            // Decide whether to spawn single or group
            if (prefabsAreGroups && extractIndividualAsteroids)
            {
                if (Random.value < groupSpawnChance)
                {
                    SpawnCustomGroup();
                }
                else
                {
                    SpawnIndividualFromGroup();
                }
            }
            else if (prefabsAreGroups)
            {
                SpawnSingle();
            }
            else
            {
                if (Random.value < groupSpawnChance)
                {
                    SpawnGroup();
                }
                else
                {
                    SpawnSingle();
                }
            }
        }
    }

    void SpawnSingle()
    {
        // Randomly select an asteroid prefab from the array
        GameObject selectedPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        
        // Spawn a single asteroid
        SpawnAsteroidAt(GetRandomSpawnPosition(), selectedPrefab);
    }

    void SpawnGroup()
    {
        // Determine group size
        int groupSize = Random.Range(groupSizeRange.x, groupSizeRange.y + 1);
        
        // Get base spawn position
        Vector3 basePosition = GetRandomSpawnPosition();
        
        // Spawn multiple asteroids in formation
        for (int i = 0; i < groupSize; i++)
        {
            // Randomly select an asteroid prefab for each asteroid
            GameObject selectedPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
            
            // Calculate offset position for this asteroid in the group
            Vector3 offset = new Vector3(
                Random.Range(-groupPositionVariation.x, groupPositionVariation.x),
                Random.Range(-groupPositionVariation.y, groupPositionVariation.y),
                Random.Range(-groupSpread * 0.5f, groupSpread * 0.5f)
            );
            
            Vector3 spawnPosition = basePosition + offset;
            SpawnAsteroidAt(spawnPosition, selectedPrefab);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector3 cameraForward = cam.transform.forward;
        Vector3 cameraRight = cam.transform.right;
        Vector3 cameraUp = cam.transform.up;
        
        Vector3 basePosition = cam.transform.position + cameraForward * spawnDepthOffset;
        
        float spreadX = Random.Range(-viewportSpread.x, viewportSpread.x);
        float spreadY = Random.Range(-viewportSpread.y, viewportSpread.y);
        
        return basePosition + (cameraRight * spreadX) + (cameraUp * spreadY);
    }

    void SpawnAsteroidAt(Vector3 spawnPosition, GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("[SpawnAsteroidAt] Prefab is null!");
            return;
        }

        GameObject go = Instantiate(prefab, spawnPosition, Quaternion.identity);
        go.name = $"Asteroid_{prefab.name}_{Time.time}";
        
        float scale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
        go.transform.localScale = Vector3.one * scale;

        var rb = go.GetComponent<Rigidbody>();
        if (!rb) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = false;

        var asteroid = go.GetComponent<Asteroid>();
        if (!asteroid) asteroid = go.AddComponent<Asteroid>();
        
        Vector3 direction = aimAtPlayer && player != null ? 
            (player.position - spawnPosition).normalized : 
            -cam.transform.forward;
            
        asteroid.direction = direction;
        asteroid.speed = Random.Range(speedRange.x, speedRange.y);
        asteroid.maxLifetime = 30f;
    }

    void SpawnIndividualFromGroup()
    {
        // Randomly select a group prefab
        GameObject selectedGroupPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        
        // Get spawn position
        Vector3 spawnPosition = GetRandomSpawnPosition();
        
        // Temporarily instantiate the group to extract a random asteroid from it
        GameObject tempGroup = Instantiate(selectedGroupPrefab, Vector3.zero, Quaternion.identity);
        
        // Find all child objects with MeshRenderer (actual asteroid pieces)
        MeshRenderer[] asteroidPieces = tempGroup.GetComponentsInChildren<MeshRenderer>();
        
        if (asteroidPieces.Length > 0)
        {
            // Pick a random asteroid piece from the group
            MeshRenderer selectedPiece = asteroidPieces[Random.Range(0, asteroidPieces.Length)];
            GameObject selectedAsteroid = selectedPiece.gameObject;
            
            // Create a new individual asteroid
            GameObject individualAsteroid = new GameObject($"IndividualAsteroid_{Time.time}");
            individualAsteroid.transform.position = spawnPosition;
            individualAsteroid.transform.rotation = Random.rotation;
            
            // Copy the mesh and materials
            MeshFilter originalMeshFilter = selectedAsteroid.GetComponent<MeshFilter>();
            MeshRenderer originalRenderer = selectedAsteroid.GetComponent<MeshRenderer>();
            
            if (originalMeshFilter != null)
            {
                MeshFilter newMeshFilter = individualAsteroid.AddComponent<MeshFilter>();
                newMeshFilter.mesh = originalMeshFilter.mesh;
                
                MeshRenderer newRenderer = individualAsteroid.AddComponent<MeshRenderer>();
                newRenderer.materials = originalRenderer.materials;
            }
            
            // Add collider
            MeshCollider collider = individualAsteroid.AddComponent<MeshCollider>();
            collider.convex = true;
            
            // Apply random scale
            float scale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
            individualAsteroid.transform.localScale = Vector3.one * scale;
            
            // Add physics and movement
            var rb = individualAsteroid.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            var asteroid = individualAsteroid.AddComponent<Asteroid>();
            asteroid.direction = -cam.transform.forward;
            asteroid.speed = Random.Range(speedRange.x, speedRange.y);
            asteroid.maxLifetime = 30f;
            
            Debug.Log($"[SpawnIndividualFromGroup] Created individual asteroid from group at {spawnPosition}");
        }
        else
        {
            Debug.LogWarning("[SpawnIndividualFromGroup] No mesh renderers found in group prefab, falling back to full group");
            SpawnAsteroidAt(spawnPosition, selectedGroupPrefab);
        }
        
        // Clean up the temporary group
        Destroy(tempGroup);
    }

    void SpawnCustomGroup()
    {
        int groupSize = Random.Range(groupSizeRange.x, groupSizeRange.y + 1);
        Vector3 basePosition = GetRandomSpawnPosition();
        
        for (int i = 0; i < groupSize; i++)
        {
            Vector3 offset = new Vector3(
                Random.Range(-groupPositionVariation.x, groupPositionVariation.x),
                Random.Range(-groupPositionVariation.y, groupPositionVariation.y),
                Random.Range(-groupSpread * 0.5f, groupSpread * 0.5f)
            );
            
            Vector3 spawnPosition = basePosition + offset;
            SpawnIndividualFromGroupAt(spawnPosition);
        }
    }

    void SpawnIndividualFromGroupAt(Vector3 spawnPosition)
    {
        // This is similar to SpawnIndividualFromGroup but takes a specific position
        GameObject selectedGroupPrefab = asteroidPrefabs[Random.Range(0, asteroidPrefabs.Length)];
        
        // Temporarily instantiate the group to extract a random asteroid from it
        GameObject tempGroup = Instantiate(selectedGroupPrefab, Vector3.zero, Quaternion.identity);
        
        // Find all child objects with MeshRenderer (actual asteroid pieces)
        MeshRenderer[] asteroidPieces = tempGroup.GetComponentsInChildren<MeshRenderer>();
        
        if (asteroidPieces.Length > 0)
        {
            // Pick a random asteroid piece from the group
            MeshRenderer selectedPiece = asteroidPieces[Random.Range(0, asteroidPieces.Length)];
            GameObject selectedAsteroid = selectedPiece.gameObject;
            
            // Create a new individual asteroid
            GameObject individualAsteroid = new GameObject($"GroupAsteroid_{Time.time}_{Random.Range(0, 1000)}");
            individualAsteroid.transform.position = spawnPosition;
            individualAsteroid.transform.rotation = Random.rotation;
            
            // Copy the mesh and materials
            MeshFilter originalMeshFilter = selectedAsteroid.GetComponent<MeshFilter>();
            MeshRenderer originalRenderer = selectedAsteroid.GetComponent<MeshRenderer>();
            
            if (originalMeshFilter != null)
            {
                MeshFilter newMeshFilter = individualAsteroid.AddComponent<MeshFilter>();
                newMeshFilter.mesh = originalMeshFilter.mesh;
                
                MeshRenderer newRenderer = individualAsteroid.AddComponent<MeshRenderer>();
                newRenderer.materials = originalRenderer.materials;
            }
            
            // Add collider
            MeshCollider collider = individualAsteroid.AddComponent<MeshCollider>();
            collider.convex = true;
            
            // Apply random scale
            float scale = Random.Range(uniformScaleRange.x, uniformScaleRange.y);
            individualAsteroid.transform.localScale = Vector3.one * scale;
            
            // Add physics and movement
            var rb = individualAsteroid.AddComponent<Rigidbody>();
            rb.useGravity = false;
            
            var asteroid = individualAsteroid.AddComponent<Asteroid>();
            asteroid.direction = -cam.transform.forward;
            asteroid.speed = Random.Range(speedRange.x, speedRange.y);
            asteroid.maxLifetime = 30f;
        }
        
        Destroy(tempGroup);
    }

}