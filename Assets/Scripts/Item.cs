using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 22f;
    public Vector3 direction = Vector3.back;
    public Vector2 randomSpinRange = new Vector2(-180f, 180f); // deg/sec

    [Header("Lifetime")]
    public float maxLifetime = 10f;

    [Header("Fade Settings")]
    public float fadeStartDistance = 6f;
    public float fadeEndDistance = 2f;

    private Rigidbody rb;
    private Camera cam;
    public float life = 0;
    private Vector3 spinDegPerSec;
    private Renderer rend;
    private Color baseColor;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;

        cam = Camera.main;

        rend = GetComponent<Renderer>();
        if (rend != null && rend.material.HasProperty("_Color"))
            baseColor = rend.material.color;
        else
            baseColor = Color.white;
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

    void FixedUpdate()
    {
        // Move and spin
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(spinDegPerSec * Time.fixedDeltaTime) * rb.rotation);

        // Fade near camera
        HandleFadeNearCamera();
    }

    private void Update()
    {
        // Lifetime
        life += Time.deltaTime;
        if (life >= maxLifetime)
        {
            Destroy(this.gameObject);
        }
    }

    void HandleFadeNearCamera()
    {
        if (!cam || rend == null) return;

        float dist = Vector3.Distance(cam.transform.position, transform.position);
        if (dist > fadeStartDistance) return;

        float t = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, dist);
        float alpha = Mathf.Clamp01(t);

        if (rend.material.HasProperty("_Color"))
        {
            Color c = baseColor;
            c.a = alpha;
            rend.material.color = c;
        }


    }



}

