using UnityEngine;

[RequireComponent(typeof(Transform))]
public class SpaceJunkDrifter : MonoBehaviour
{
    [Header("Motion")]
    public Vector2 speedRange = new Vector2(0.5f, 2f);
    public float torqueZDegPerSec = 25f;
    public float wrapPadding = 1f;
    public Vector2 randomScaleRange = new Vector2(0.85f, 1.25f);

    Rigidbody rb;
    Camera cam;
    float halfW, halfH, zDist;

    void Awake()
    {
        // 1) Ensure Rigidbody exists & is set up for “floating”
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        // 2) Make sure colliders don't cause pushes (remove or make trigger)
        foreach (var col in GetComponentsInChildren<Collider>(true))
        {
            col.isTrigger = true;  // turn any collider into a trigger (harmless)
        }

        // 3) Put object on SpaceJunk layer if it exists
        int lj = LayerMask.NameToLayer("SpaceJunk");
        if (lj != -1) SetLayerRecursive(transform, lj);

        cam = Camera.main;

        // 4) Random size, speed, direction & spin
        float s = Random.Range(randomScaleRange.x, randomScaleRange.y);
        transform.localScale *= s;

        float speed = Random.Range(speedRange.x, speedRange.y);
        Vector2 dir2 = Random.insideUnitCircle.normalized;
        rb.linearVelocity = new Vector3(dir2.x, dir2.y, 0f) * speed;

        rb.angularVelocity = new Vector3(0f, 0f, torqueZDegPerSec * Mathf.Deg2Rad);
    }

    void Start()
    {
        zDist = Mathf.Abs(transform.position.z - cam.transform.position.z);
        Vector3 bl = cam.ViewportToWorldPoint(new Vector3(0, 0, zDist));
        Vector3 tr = cam.ViewportToWorldPoint(new Vector3(1, 1, zDist));
        halfW = (tr.x - bl.x) * 0.5f;
        halfH = (tr.y - bl.y) * 0.5f;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 c = cam.transform.position; c.z = pos.z;

        float left = c.x - halfW - wrapPadding, right = c.x + halfW + wrapPadding;
        float bottom = c.y - halfH - wrapPadding, top = c.y + halfH + wrapPadding;

        if (pos.x < left) pos.x = right; else if (pos.x > right) pos.x = left;
        if (pos.y < bottom) pos.y = top; else if (pos.y > top) pos.y = bottom;

        transform.position = pos;
    }

    static void SetLayerRecursive(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; i++) SetLayerRecursive(t.GetChild(i), layer);
    }
}
