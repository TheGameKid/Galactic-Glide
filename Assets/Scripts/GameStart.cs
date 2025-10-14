using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.IO;

public class GameStart : MonoBehaviour
{
    public float gameSpeed;
    public float startTimer = 4;
    public float DDATimer;
    public float DDAStartTimer;
    public ParticleSystem[] WarpParticles = new ParticleSystem[3];
    public GameObject[] WarpObjects = new GameObject[3];
    public GameObject[] SpaceBackground = new GameObject[3];
    public string difficulty;

    public Player player;
    public AsteroidSpawner asteroidSpawner;
    public ItemSpawner itemSpawner;
    public TextMeshProUGUI Score;
    public GameObject[] Countdown = new GameObject[4];

    public GameObject[] HUD = new GameObject[2];

    public GameObject Menu;
    public GameObject GameOver;
    public GameObject BlackScreen;

    public int score;

    public AudioSource startJingle;
    public AudioSource GetReady;

    public bool startCountDown = false;

    public float timer;

    public TextMeshProUGUI FinalScore;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        StreamReader sr = new StreamReader("GalacticGlide/Mode.txt");
        difficulty = sr.ReadLine();
        sr.Close();
        
        if (difficulty == "Easy")
        {
            WarpObjects[0].SetActive(true);
            SpaceBackground[0].SetActive(true);
            gameSpeed = 1.5f;
            var main = WarpParticles[0].main;
            main.simulationSpeed = gameSpeed;
            DDATimer = 5;
            DDAStartTimer = 5;
            asteroidSpawner.speedRange = new Vector2(6f, 10f);
            asteroidSpawner.spawnInterval = 0.7f;
            itemSpawner.spawnEvery = 15f;
        }
        if (difficulty == "Normal")
        {
            WarpObjects[1].SetActive(true);
            SpaceBackground[1].SetActive(true);
            gameSpeed = 3f;
            var main = WarpParticles[1].main;
            main.simulationSpeed = gameSpeed;
            DDATimer = 10;
            DDAStartTimer = 10;
            asteroidSpawner.speedRange = new Vector2(8f, 12f);
            asteroidSpawner.spawnInterval = 0.5f;
            itemSpawner.spawnEvery = 10f;
        }
        if (difficulty == "Hard")
        {
            WarpObjects[2].SetActive(true);
            SpaceBackground[2].SetActive(true);
            gameSpeed = 5f;
            var main = WarpParticles[2].main;
            main.simulationSpeed = gameSpeed;
            DDATimer = 20;
            DDAStartTimer = 20;
            asteroidSpawner.speedRange = new Vector2(10f, 15f);
            asteroidSpawner.spawnInterval = 0.3f;
            itemSpawner.spawnEvery = 5f;
        }

       // player.enabled = false;
        asteroidSpawner.enabled = false;
        itemSpawner.enabled = false;
        Menu.SetActive(false);
        GameOver.SetActive(false);
        score = 0;
        startTimer = 4;
        player.Lives = 3;
        HUD[0].SetActive(true);
        HUD[1].SetActive(true);
        player.UpdateHealth(3);
        Score.text = score.ToString();
        Countdown[2].SetActive(false);
        Countdown[1].SetActive(false);
        Countdown[0].SetActive(false);
        StartCoroutine(CountDowner());

    }

    // Update is called once per frame
    void Update()
    {
        if (startCountDown)
        {
            startTimer -= Time.deltaTime;

            if (startTimer <= 0)
            {
                Countdown[2].SetActive(false);
                Countdown[3].SetActive(true);
                //player.enabled = true;
            }
            else if (startTimer <= 1)
            {
                Countdown[0].SetActive(false);
                Countdown[1].SetActive(false);
                Countdown[2].SetActive(true);
            }
            else if (startTimer <= 2)
            {
                Countdown[0].SetActive(false);
                Countdown[1].SetActive(true);
            }
            else if (startTimer <= 3)
            {
                Countdown[0].SetActive(true);

            }
           
           
           
        }

        if (startTimer < -1 && player.Lives > 0)
        {
            startCountDown = false;
            startTimer = -2;
            Countdown[3].SetActive(false);
            asteroidSpawner.enabled = true;
            itemSpawner.enabled = true;
            timer += Time.deltaTime;

            if (timer > 1)
            {
                timer = 0;
                score += 1;
            }
            
            DDATimer -= Time.deltaTime;

            if (DDATimer < 0)
            {
                DDATimer = 0;

                if (difficulty == "Easy")
                {
                    gameSpeed += 0.5f;
                    var main = WarpParticles[0].main;
                    main.simulationSpeed = gameSpeed;
                    DDATimer = DDAStartTimer;
                    asteroidSpawner.speedRange = new Vector2(asteroidSpawner.speedRange.x + 1, asteroidSpawner.speedRange.y + 1);
                    //itemSpawner.speedRange = new Vector2(itemSpawner.speedRange.x + 1, itemSpawner.speedRange.y + 1);
                }
                if (difficulty == "Normal")
                {
                    gameSpeed += 0.7f;
                    var main = WarpParticles[1].main;
                    main.simulationSpeed = gameSpeed;
                    DDATimer = DDAStartTimer;
                    asteroidSpawner.speedRange = new Vector2(asteroidSpawner.speedRange.x + 2, asteroidSpawner.speedRange.y + 2);
                    //itemSpawner.speedRange = new Vector2(itemSpawner.speedRange.x + 2, itemSpawner.speedRange.y + 2);
                }
                if (difficulty == "Hard")
                {
                    gameSpeed += 1f;
                    var main = WarpParticles[2].main;
                    main.simulationSpeed = gameSpeed;
                    DDATimer = DDAStartTimer;
                    asteroidSpawner.speedRange = new Vector2(asteroidSpawner.speedRange.x + 3, asteroidSpawner.speedRange.y + 3);
                    //itemSpawner.speedRange = new Vector2(itemSpawner.speedRange.x + 3, itemSpawner.speedRange.y + 3);
                }

            }

            Score.text = score.ToString();

        }

       if (Menu.activeInHierarchy || GameOver.activeInHierarchy)
        {
            Cursor.visible = true;
        }
       else
        {
            Cursor.visible = false;
        }

    }

    public IEnumerator CountDowner()
    {
        startJingle.Play();
        yield return new WaitForSeconds(1.7f);
        startCountDown = true;
        yield return new WaitForSeconds(1f);
        GetReady.Play();
    }

    public IEnumerator GameOverScreen()
    {
        yield return new WaitForSeconds(3f);
        GameOver.SetActive(true);
        FinalScore.text = "Final Score: " + score.ToString();
    }

    public void Resume()
    {
        Menu.SetActive(false);
        HUD[0].SetActive(true);
        HUD[1].SetActive(true);
        player.UpdateHealth(0);
        Time.timeScale = 1;
    }
    public void Quit()
    {
        Menu.SetActive(false);
        HUD[0].SetActive(false);
        HUD[1].SetActive(false);
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public void DestroyAllByTag(string tagName)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tagName);
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
    }
}
