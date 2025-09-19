using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    bool MoveLeft = false, MoveRight = false, MoveForward = false, MoveBackward = false;
    bool RotateLeft = false, RotateRight = false;
    float VZ = 0f, VY = 0f;
    public float rotX;

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
    }

    public void CubeTranslation()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.upArrowKey.isPressed) VY = 0.1f;
        if (kb.downArrowKey.isPressed) VY = -0.1f;

        if (kb.leftArrowKey.isPressed) VZ = 0.1f;
        if (kb.rightArrowKey.isPressed) VZ = -0.1f;

        // NO rotation code

        if (VY != 0) transform.position += VY * transform.up;
        if (VZ != 0) transform.position += VZ * transform.forward;

        MoveForward = MoveRight = MoveLeft = MoveBackward = false;
        RotateLeft = RotateRight = false;
        VZ = VY = rotX = 0;
    }
}