using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    bool MoveLeft = false, MoveRight = false, MoveForward = false, MoveBackward = false;
    bool RotateLeft = false, RotateRight = false;
    float VZ = 0f, VY = 0f;
    public float rotX;
    public GameObject Forward;

    private Rigidbody rb; // NEW

    void Awake() // NEW
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            // Freeze all rotation so physics can’t tilt the ship
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    void Start() { }

    void Update()
    {
        CubeTranslation();
        //transform.LookAt(Forward.transform);
    }

    public void CubeTranslation()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.upArrowKey.isPressed) VY = 0.3f;
        if (kb.downArrowKey.isPressed) VY = -0.3f;

        if (kb.leftArrowKey.isPressed) VZ = 0.3f;
        if (kb.rightArrowKey.isPressed) VZ = -0.3f;

        // NO rotation code
        Vector3 p = transform.position;
        if (VZ == 0)
        {
            transform.rotation = Quaternion.Euler(0f, -90f, 21.866f);
        }
        if (VY != 0)
        {
            p += VY * Vector3.up;
        }
        if (VZ != 0)
        {
            p += VZ * -Vector3.right;
            if (VZ < 0)
            {
                transform.rotation = Quaternion.Euler(-20f, -90f, 21.866f);
            }
            if (VZ > 0)
            {
                transform.rotation = Quaternion.Euler(20f, -90f, 21.866f);
            }
        }

        // --- Camera-relative clamp (orthographic) ---
        var cam = Camera.main;
        if (cam != null)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            Vector3 c = cam.transform.position;

            // Keep the whole sprite/mesh on-screen if possible
            var r = GetComponent<Renderer>();
            Vector3 ext = r != null ? r.bounds.extents : Vector3.zero;

            p.x = Mathf.Clamp(p.x, c.x - halfW + ext.x + 0.5f, c.x + halfW - ext.x - 0.5f);
            p.y = Mathf.Clamp(p.y, c.y - halfH + ext.y + 0.5f, c.y + halfH - ext.y - 1.4f);
        }
        // -------------------------------------------

        transform.position = p;

        MoveForward = MoveRight = MoveLeft = MoveBackward = false;
        RotateLeft = RotateRight = false;
        VZ = VY = rotX = 0;
    }
}