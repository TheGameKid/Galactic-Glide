using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 22f;
    public Vector3 direction = Vector3.back;
    public Vector2 randomSpinRange = new Vector2(-180f, 180f); // deg/sec

    [Header("Lifetime")]
    public float maxLifetime = 90f;

    [Header("Fade Settings")]
    [Tooltip("Distance from camera where fading begins")]
    public float fadeStartDistance = 8f;
    [Tooltip("Distance from camera where asteroid is fully invisible")]
    public float fadeEndDistance = 2f;

    Rigidbody rb;
    Camera cam;
    float life;
    Vector3 spinDegPerSec;
    Renderer[] rends;
    Color[] baseColors;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;

        foreach (var c in GetComponentsInChildren<Collider>(true)) c.isTrigger = true;

        cam = Camera.main;

        // Cache renderers and base colors for fading
        rends = GetComponentsInChildren<Renderer>(true);
        if (rends != null && rends.Length > 0)
        {
            baseColors = new Color[rends.Length];
            for (int i = 0; i < rends.Length; i++)
            {
                if (rends[i].material.HasProperty("_Color"))
                    baseColors[i] = rends[i].material.color;
                else
                    baseColors[i] = Color.white;
            }
        }

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Collider itemCol = item.GetComponent<Collider>();
            Collider myCol = GetComponent<Collider>();
            if (itemCol != null && myCol != null)
                Physics.IgnoreCollision(myCol, itemCol);
        }

    }

    void OnEnable()
    {
        life = 0f;
        spinDegPerSec = new Vector3(
            Random.Range(randomSpinRange.x, randomSpinRange.y),
            Random.Range(randomSpinRange.x, randomSpinRange.y),
            Random.Range(randomSpinRange.x, randomSpinRange.y)
        );
    }

    private void Update()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Collider itemCol = item.GetComponent<Collider>();
            Collider myCol = GetComponent<Collider>();
            if (itemCol != null && myCol != null)
                Physics.IgnoreCollision(myCol, itemCol);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(spinDegPerSec * Time.fixedDeltaTime) * rb.rotation);

        life += Time.fixedDeltaTime;
        if (life >= maxLifetime) { Destroy(gameObject); return; }

        HandleFadeNearCamera();
    }

    void HandleFadeNearCamera()
    {
        if (!cam || rends == null) return;

        float dist = Vector3.Distance(cam.transform.position, transform.position);
        if (dist > fadeStartDistance) return;

        // compute alpha from 1 → 0 between start and end
        float t = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, dist);
        float alpha = Mathf.Clamp01(t);

        for (int i = 0; i < rends.Length; i++)
        {
            var m = rends[i].material;
            if (m.HasProperty("_Color"))
            {
                Color c = baseColors[i];
                c.a = alpha;
                m.color = c;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Asteroid"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Laser"))
        {
            return;
        }
        if (other.gameObject.CompareTag("Item"))
        {
            Physics.IgnoreCollision(other, GetComponent<Collider>());
            return;
        }
    }
}
