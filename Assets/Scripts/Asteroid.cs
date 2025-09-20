using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid : MonoBehaviour
{
    [Header("Motion")]
    public float speed = 12f;
    public Vector3 direction = Vector3.back; // set by spawner
    public Vector2 randomSpinRange = new Vector2(-180f, 180f); // deg/sec

    [Header("Lifetime / Culling")]
    public float maxLifetime = 8f;          // safety timer in seconds
    public float offscreenMargin = 0.1f;    // allow small margin outside viewport

    private Rigidbody rb;
    private Camera cam;
    private float life;
    private Vector3 spinDegPerSec;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        cam = Camera.main;
    }

    void OnEnable()
    {
        life = 0f;
        // random angular speed in degrees/sec
        spinDegPerSec = new Vector3(
            Random.Range(randomSpinRange.x, randomSpinRange.y),
            Random.Range(randomSpinRange.x, randomSpinRange.y),
            Random.Range(randomSpinRange.x, randomSpinRange.y)
        );
    }

    void FixedUpdate()
    {
        // forward motion
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

        // spin
        Quaternion delta = Quaternion.Euler(spinDegPerSec * Time.fixedDeltaTime);
        rb.MoveRotation(delta * rb.rotation);

        // lifetime guard
        life += Time.fixedDeltaTime;
        if (life >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // off-screen cull
        if (cam != null)
        {
            Vector3 vp = cam.WorldToViewportPoint(transform.position);
            if (vp.z < 0f ||
                vp.x < -offscreenMargin || vp.x > 1f + offscreenMargin ||
                vp.y < -offscreenMargin || vp.y > 1f + offscreenMargin)
            {
                Destroy(gameObject);
            }
        }
    }
}
