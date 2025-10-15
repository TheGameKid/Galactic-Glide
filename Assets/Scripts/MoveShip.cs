using UnityEngine;

public class MoveShip : MonoBehaviour
{
    [Header("Path")]
    public Transform startPoint;
    public Transform endPoint;
    public float duration = 3f; // seconds
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Look & Feel")]
    public bool faceDirection = true;
    public float rollAmount = 12f;

    [Header("FX (optional)")]
    public ParticleSystem warpInFX;   // e.g., Warp_Fast_Blue
    public ParticleSystem engineFX;   // thruster particles
    public AudioSource whooshSfx;

    float t;               // 0..1
    bool moving;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim) anim.enabled = false;      // make sure Animator can’t fight us
    }

    void OnEnable()
    {
        // start at startPoint each time we run
        if (startPoint) transform.position = startPoint.position;
        t = 0f;
        moving = true;

        if (warpInFX) warpInFX.Play();
        if (engineFX)
        {
            engineFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            engineFX.Play();
        }
        if (whooshSfx) whooshSfx.Play();

    }
    void OnDrawGizmosSelected()
    {
        if (startPoint && endPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.DrawSphere(startPoint.position, 0.2f);
            Gizmos.DrawSphere(endPoint.position, 0.2f);
        }
    }


    void Update()
    {
        if (!moving || !startPoint || !endPoint || duration <= 0f) return;

        t += Time.deltaTime / duration;
        float k = ease.Evaluate(Mathf.Clamp01(t));

        Vector3 next = Vector3.Lerp(startPoint.position, endPoint.position, k);

        if (faceDirection)
        {
            Vector3 dir = (next - transform.position);
            if (dir.sqrMagnitude > 1e-6f)
            {
                Quaternion look = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = look * Quaternion.Euler(0, 0, Mathf.Sin(k * Mathf.PI) * rollAmount);
            }
        }

        transform.position = next;

        if (t >= 1f) // arrived -> stop and stay
        {
            transform.position = endPoint.position;
            moving = false;
            if (warpInFX) warpInFX.Stop();
            // keep Animator OFF so the ship stays put
        }
    }
}
