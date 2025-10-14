using System.Collections;
using TMPro;
using UnityEditor;
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

    public int Lives = 3;
    public GameObject[] hearts = new GameObject[3];
    public GameObject shield;
    public int laserAmmo;
    public GameObject LeftShotSpawn;
    public GameObject RightShotSpawn;
    public GameObject Laser;

    // --- Invincibility (ADD) ---
    public float invincibilityDuration = 2f;   // seconds of i-frames after a hit
    public float flashInterval = 0.1f;         // how fast to flash red
    bool isInvincible = false;

    Renderer[] _rends;     // cached renderers to tint
    Color[] _baseColors;   // original colors to restore
    public GameStart game;
    public AudioSource Fire;
    public AudioSource Hit;
    public AudioSource Dead;
    public AudioSource Heart;
    public AudioSource LaserPack;
    public AudioSource Shield;
    public AudioSource ShieldGone;
    public AudioSource Open;
    public AudioSource Close;

    public TextMeshPro ammoAmount;

    public GameObject BlueFire;
    public GameObject Ship;
    public GameObject ExplosionParticle;
    public bool once;

    public Vector3 startPos = new Vector3(0.0033443f, -2.17f, -7.13f);
    public Vector3 endPos = new Vector3(0.0033443f, -0.85858f, -3.853f);

    public float moveDuration = 2f; // seconds to move from start to end
    public float timer = 0f;

    public GameObject laserText;

    void Awake() // NEW
    {
        rb = GetComponent<Rigidbody>();
        transform.position = startPos;
        Ship.SetActive(true);
        if (rb != null)
        {
            rb.useGravity = false;
            // Freeze all rotation so physics can’t tilt the ship
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        Lives = 3;
        BlueFire.SetActive(true);
        ExplosionParticle.SetActive(false);
        once = true;
        _rends = GetComponentsInChildren<Renderer>(true);
        if (_rends != null && _rends.Length > 0)
        {
            _baseColors = new Color[_rends.Length];
            for (int i = 0; i < _rends.Length; i++)
            {
                if (_rends[i].material.HasProperty("_Color"))
                    _baseColors[i] = _rends[i].material.color;
                else
                    _baseColors[i] = Color.white;
            }
        }

        laserAmmo = 10;
        laserText.SetActive(false);

    }

    void Start() { }

    void Update()
    {
        if (Lives > 0 && game.startTimer <= 0)
        {

            CubeTranslation();
            laserText.SetActive(true);
            //transform.LookAt(Forward.transform);

            if (laserAmmo > 0)
            {
                ammoAmount.text = laserAmmo.ToString();
                var kb = Keyboard.current;
                if (kb == null) return;

                if (kb.spaceKey.wasPressedThisFrame)
                {
                    Instantiate(Laser, LeftShotSpawn.transform.position, Quaternion.identity);
                    Instantiate(Laser, RightShotSpawn.transform.position, Quaternion.identity);
                    laserAmmo -= 1;
                    Fire.Play();

                    if (laserAmmo == 0)
                    {
                        ammoAmount.text = "";
                    }
                    
                }
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                game.Menu.SetActive(true);
                game.HUD[0].SetActive(false);
                game.HUD[1].SetActive(false);
                hearts[0].SetActive(false);
                hearts[1].SetActive(false);
                hearts[2].SetActive(false);
                Open.Play();
                Time.timeScale = 0;
            }

        }

        if (once)
        {
            //0.0033443f, -0.85858f, -3.853f
            //0.0033443f, -2.17f, -7.13f,
            timer += Time.deltaTime;

            // Calculate normalized time (0 to 1)
            float t = timer / moveDuration;

            // Clamp so it doesn't go past 1
            t = Mathf.Clamp01(t);

            // Move the object smoothly between start and end
            transform.position = Vector3.Lerp(startPos, endPos, t);

            if (t >= 1f)
            {
                once = false;
                timer = 0;
            }


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Laser"))
        {
            return;
        }
        
        if (other.gameObject.CompareTag("Asteroid"))
        {
            if (!shield.activeInHierarchy && !isInvincible)
            {
                UpdateHealth(-1);
                Destroy(other.gameObject);
                Hit.Play();
            }
            else
            {
                if (shield.activeInHierarchy)
                {
                    ShieldGone.Play();
                    shield.SetActive(false);
                }
                Destroy(other.gameObject);
            }
            
        }
        if (other.gameObject.CompareTag("Item"))
        {
            if (other.gameObject.name == "Heart(Clone)")
            {
                UpdateHealth(1);
                Destroy(other.gameObject);
                Heart.Play();
            }
            if (other.gameObject.name == "Lasers(Clone)")
            {
                laserAmmo = 10;
                Destroy(other.gameObject);
                LaserPack.Play();
            }
            if (other.gameObject.name == "Shield(Clone)")
            {
                shield.SetActive(true);
                Destroy(other.gameObject);
                Shield.Play();
            }
        }
    }

    public void UpdateHealth(int health)
    {
        Lives += health;

        if (health < 0 && isInvincible) return;

        // If we took damage and are still alive, start invincibility (ADD)
        if (health < 0 && Lives > 0 && !isInvincible)
        {
            if (health < 0)
            StartCoroutine(InvincibilityFlash());
        }
        else if (health < 0 && Lives == 0)
        {
            // flash red
            if (_rends != null)
            {
                for (int i = 0; i < _rends.Length; i++)
                {
                    var mat = _rends[i].material;
                    if (!mat.HasProperty("_Color")) continue;

                    Color c = Color.red;
                    mat.color = c;
                }
            }
        }

        if (Lives >= 3)
        {
            Lives = 3;
            hearts[2].SetActive(true);
            hearts[1].SetActive(true);
            hearts[0].SetActive(true);
        }
        else if (Lives == 2)
        {
            hearts[2].SetActive(false);
            hearts[1].SetActive(true);
            hearts[0].SetActive(true);
        }
        else if (Lives == 1)
        {
            hearts[2].SetActive(false);
            hearts[1].SetActive(false); 
            hearts[0].SetActive(true);
        }
        else
        {
            hearts[0].SetActive(false);
            game.BlackScreen.SetActive(true);
            game.WarpObjects[0].SetActive(false);
            game.WarpObjects[1].SetActive(false);
            game.WarpObjects[2].SetActive(false);
            game.SpaceBackground[0].SetActive(false);
            game.SpaceBackground[1].SetActive(false);
            game.SpaceBackground[2].SetActive(false);
            game.HUD[0].SetActive(false);
            game.HUD[1].SetActive(false);
            BlueFire.SetActive(false);
            ammoAmount.text = "";
            laserAmmo = 0;
            StartCoroutine(Explosion());
        }
    }

    // --- Invincibility routine (ADD) ---
    System.Collections.IEnumerator InvincibilityFlash()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool redPhase = false;

        while (elapsed < invincibilityDuration)
        {
            elapsed += flashInterval;
            redPhase = !redPhase;

            // flash red <-> original colors
            if (_rends != null)
            {
                for (int i = 0; i < _rends.Length; i++)
                {
                    var mat = _rends[i].material;
                    if (!mat.HasProperty("_Color")) continue;

                    Color c = redPhase ? Color.red : _baseColors[i];
                    mat.color = c;
                }
            }

            yield return new WaitForSeconds(flashInterval);
        }

        // restore colors
        if (_rends != null)
        {
            for (int i = 0; i < _rends.Length; i++)
            {
                var mat = _rends[i].material;
                if (mat.HasProperty("_Color"))
                    mat.color = _baseColors[i];
            }
        }

        isInvincible = false;
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

    public IEnumerator Explosion()
    {
        game.DestroyAllByTag("Asteroid");
        game.DestroyAllByTag("Item");
        game.DestroyAllByTag("Laser");
        game.asteroidSpawner.enabled = false;
        game.itemSpawner.enabled = false;
        yield return new WaitForSeconds(0.7f);
        Dead.Play();
        Ship.SetActive(false);
        ExplosionParticle.SetActive(true);
        StartCoroutine(game.GameOverScreen());
        yield return new WaitForSeconds(3f);
        
        this.gameObject.SetActive(false);
    }
}