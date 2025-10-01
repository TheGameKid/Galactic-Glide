using UnityEngine;
using UnityEngine.InputSystem;

public class Sphere : MonoBehaviour
{
    public Transform player;
    public float ahead = 20f; // how far in front (world Z+)

    void LateUpdate()
    {
        if (!player) return;
        // always ahead in world space (Z+), not tied to player rotation
        transform.position = new Vector3(player.position.x,
                                         player.position.y,
                                         player.position.z + ahead);
    }
}
