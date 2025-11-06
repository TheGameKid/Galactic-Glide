using System.Collections.Generic;
using UnityEngine;

public class HitBoxColor : MonoBehaviour
{
    [Header("References")]
    public Transform playerRoot;          // Usually the Player root
    public string asteroidTag = "Asteroid";

    [Header("Prediction Settings")]
    public float lookaheadSeconds = 2.0f; // How far ahead to predict
    public float warnRadius = 15.0f;       // Lateral distance threshold for yellow
    public float dangerRadius = 7.0f;     // Lateral distance threshold for red
    [Range(0f, 1f)]
    public float minApproachDot = 0.25f;  // Require asteroid to face roughly toward the player

    [Header("Colors")]
    public Color safeColor = new Color(0.2f, 1f, 0.2f, 0.3f);
    public Color warnColor = new Color(1f, 0.9f, 0.2f, 0.35f);
    public Color dangerColor = new Color(1f, 0.25f, 0.25f, 0.4f);
    public float colorLerpSpeed = 10f;

    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (!playerRoot) playerRoot = transform.parent;
        if (rend && rend.material.HasProperty("_Color"))
            rend.material.color = safeColor;
    }

    void Update()
    {
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag(asteroidTag);
        if (asteroids.Length == 0)
        {
            SetColorSmooth(safeColor);
            return;
        }

        bool anyWarn = false;
        bool anyDanger = false;

        Vector3 playerPos = playerRoot.position;

        foreach (GameObject asteroid in asteroids)
        {
            if (asteroid == null) continue;

            // Try to get direction and speed
            Vector3 vhat = Vector3.zero;
            float speed = 0f;
            Asteroid a = asteroid.GetComponent<Asteroid>();
            Rigidbody rb = asteroid.GetComponent<Rigidbody>();

            if (a != null && a.direction.sqrMagnitude > 0.0001f && a.speed > 0f)
            {
                vhat = a.direction.normalized;
                speed = a.speed;
            }
            else if (rb != null && rb.linearVelocity.sqrMagnitude > 0.0001f)
            {
                vhat = rb.linearVelocity.normalized;
                speed = rb.linearVelocity.magnitude;
            }
            else continue; // skip invalid

            // Relative vector from asteroid to player
            Vector3 r = playerPos - asteroid.transform.position;

            // Must be roughly moving toward player
            float approach = Vector3.Dot(vhat, r.normalized);
            if (approach < minApproachDot) continue;

            // Predict time to closest approach
            float tClosest = -Vector3.Dot(r, vhat) / Mathf.Max(speed, 0.0001f);
            if (tClosest < 0f || tClosest > lookaheadSeconds) continue;

            // Lateral distance at closest approach
            float lateral = Vector3.ProjectOnPlane(r, vhat).magnitude;

            if (lateral <= dangerRadius) { anyDanger = true; break; }
            else if (lateral <= warnRadius) { anyWarn = true; }
        }

        if (anyDanger)
            SetColorSmooth(dangerColor);
        else if (anyWarn)
            SetColorSmooth(warnColor);
        else
            SetColorSmooth(safeColor);
    }

    void SetColorSmooth(Color target)
    {
        if (!rend || !rend.material.HasProperty("_Color")) return;
        rend.material.color = Color.Lerp(rend.material.color, target, Time.deltaTime * colorLerpSpeed);
    }
}
