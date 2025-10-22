using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveShip : MonoBehaviour
{

    public bool once;

    public Vector3 startPos = new Vector3(0, 0, 30f);
    public Vector3 endPos = new Vector3(0, 0, 1f);

    public float moveDuration = 2f; // seconds to move from start to end
    public float timer = 0f;

    public GameObject Galactic;
    public float time2 = 0;
    public float dur2 = 1.5f;
    public bool once2;
    public GameObject Glide;
    public Vector3 targetScale = new Vector3(1f, 1f, 1f);
    public Vector3 targetScaleShip = new Vector3(1.8f, 1.8f, 1.8f);

    public GameObject Author;

    public GameObject Play;
    public GameObject Quit;

    public GameObject BlueFire;

    public GameObject B1;
    public GameObject B2;
    public GameObject B3;

    void Awake()
    {
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
        once = false;
        once2 = true;
        moveDuration = 1;
        transform.localScale = new Vector3(0, 0, 0);
        Galactic.transform.localScale = new Vector3(0, 0, 0);
        Glide.transform.localScale = new Vector3(0, 0, 0);
        time2 = 0;
        dur2 = 5;
        timer = 0;
        Play.SetActive(false);
        Quit.SetActive(false);
        Author.SetActive(false);
        BlueFire.SetActive(false);

        B1.SetActive(false);
        B2.SetActive(false);
        B3.SetActive(false);

        int random = Random.Range(1, 4);

        if (random == 1)
        {
            B1.SetActive(true);
        }
        else if (random == 2)
        {
            B2.SetActive(true);
        }
        else
        {
            B3.SetActive(true);
        }

    }

  
    void Update()
    {
        if (once)
        {
            //0.0033443f, -0.85858f, -3.853f
            //0.0033443f, -2.17f, -7.13f,
            timer += Time.deltaTime;
            Author.SetActive(true);

            if (timer < 1)
            {
                timer += Time.deltaTime;
                float t = timer / 1;
                transform.localScale = Vector3.Lerp(transform.localScale, targetScaleShip, t);
               // Glide.transform.localScale = Vector3.Lerp(Glide.transform.localScale, targetScale, t);
            }
            else
            {
                once = false;
                Play.SetActive(true);
                Quit.SetActive(true);
                BlueFire.SetActive(true);
            }


        }

       

         if (once2)
        {
            if (time2 < 1)
            {
                time2 += Time.deltaTime;
                float t = time2 / 1;
                Galactic.transform.localScale = Vector3.Lerp(Galactic.transform.localScale, targetScale, t);
                Glide.transform.localScale = Vector3.Lerp(Glide.transform.localScale, targetScale, t);
            }
            else
            {
                once2 = false;
                once = true;
            }

            
        }  
   


    }

    public void StartButton()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitButton()
    {
#if UNITY_EDITOR
        // This only runs in the Unity Editor
        EditorApplication.isPlaying = false;
#else
       
        Application.Quit();
#endif
    }
}
