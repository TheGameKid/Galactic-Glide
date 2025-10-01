using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Target")]
    public Transform target;

    [Header("Camera Offset")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Dead-Zone (world units)")]
    public Vector2 deadZoneSize = new Vector2(2.5f, 1.5f);

    [Header("Smoothing")]
    [Range(0.01f, 0.6f)]
    public float smoothTime = 0.15f;

    [Header("Follow Bounds (world space)")]
    public Vector2 xBounds = new Vector2(-5.096653f, 5.203342f);
    public Vector2 yBounds = new Vector2(-2.80453f, 1.79547f);

    Vector3 _vel;
    float _initialZ;

    void Awake()
    {
        _initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Dead-zone follow center (cameraPos - offset.xy)
        Vector2 followCenter = new Vector2(transform.position.x - offset.x,
                                           transform.position.y - offset.y);

        float halfX = deadZoneSize.x * 0.5f;
        float halfY = deadZoneSize.y * 0.5f;

        Vector2 tgt = new Vector2(target.position.x, target.position.y);

        // Horizontal dead-zone
        if (tgt.x < followCenter.x - halfX)
            followCenter.x = tgt.x + halfX;
        else if (tgt.x > followCenter.x + halfX)
            followCenter.x = tgt.x - halfX;

        // Vertical dead-zone
        if (tgt.y < followCenter.y - halfY)
            followCenter.y = tgt.y + halfY;
        else if (tgt.y > followCenter.y + halfY)
            followCenter.y = tgt.y - halfY;

        // Clamp follow to world bounds
        followCenter.x = Mathf.Clamp(followCenter.x, xBounds.x, xBounds.y);
        followCenter.y = Mathf.Clamp(followCenter.y, yBounds.x, yBounds.y);

        // Desired camera pos
        float camZ = (offset.z != 0f) ? target.position.z + offset.z : _initialZ;
        Vector3 desired = new Vector3(followCenter.x + offset.x,
                                      followCenter.y + offset.y,
                                      camZ);

        // Smooth movement
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref _vel, smoothTime);

        // Lock rotation (no tilt, no yaw/roll)
        transform.rotation = Quaternion.identity;
    }
}
