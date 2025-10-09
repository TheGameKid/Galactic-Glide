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

    Rigidbody rb;
    Camera cam;
    float life;
    Vector3 spinDegPerSec;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        foreach (var c in GetComponentsInChildren<Collider>(true)) c.isTrigger = true;

        // fully free rotation/motion
        rb.constraints = RigidbodyConstraints.None;

        cam = Camera.main;
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
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Euler(spinDegPerSec * Time.fixedDeltaTime) * rb.rotation);

        life += Time.fixedDeltaTime;
        if (life >= maxLifetime) { Destroy(gameObject); }
        // NOTE: no aggressive offscreen culling; we let lifetime handle it
    }
}
