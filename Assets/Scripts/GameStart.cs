using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;

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
    public GameObject NewRecordScreen;

    public int score;

    public AudioSource startJingle;
    public AudioSource GetReady;

    public bool startCountDown = false;

    public float timer;

    public TextMeshProUGUI FinalScore;

    public AudioSource BGM;
    public AudioSource GOver;
    public GameObject GameOverRetry;
    public GameObject GameOverQuit;
    public GameObject GameOverNext;

    public GameObject EnterYourName;
    public TMP_InputField PlayerName;
    public GameObject HighScore;
    public AudioSource NewRecord;
    public TextMeshProUGUI ErrorMessage;

    public float MultiplySpawnTimer;
    public float MultiplyTimer;
    public bool SpawnMultiply;

    public int index = 0;
    public string filePath = "";
    public List<string> names = new List<string>();
    public List<int> bestScore = new List<int>();
    public TextMeshProUGUI[] BestNames = new TextMeshProUGUI[5];
    public TextMeshProUGUI[] BestScores = new TextMeshProUGUI[5];
    public TextMeshProUGUI mode;

    public bool newHighScore = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        string dir = Path.Combine(Application.persistentDataPath, "GalacticGlide");
        string filePath = Path.Combine(dir, "Mode.txt");
        ErrorMessage.text = "";
        if (File.Exists(filePath))
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                difficulty = sr.ReadLine();
            }
        }
        else
        {
            difficulty = "Easy"; // fallback if file missing
        }

        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
        
        if (difficulty == "Easy")
        {
            WarpObjects[0].SetActive(true);
            SpaceBackground[0].SetActive(true);
            gameSpeed = 1f;
            var main = WarpParticles[0].main;
            main.simulationSpeed = gameSpeed;
            DDATimer = 10;
            DDAStartTimer = 10;
            asteroidSpawner.speedRange = new Vector2(2f, 6f);
            asteroidSpawner.spawnInterval = 2.5f;
            itemSpawner.spawnEvery = 15f;
        }
        if (difficulty == "Normal")
        {
            WarpObjects[1].SetActive(true);
            SpaceBackground[1].SetActive(true);
            gameSpeed = 2f;
            var main = WarpParticles[1].main;
            main.simulationSpeed = gameSpeed;
            DDATimer = 15;
            DDAStartTimer = 15;
            asteroidSpawner.speedRange = new Vector2(6f, 10f);
            asteroidSpawner.spawnInterval = 1.5f;
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
            asteroidSpawner.spawnInterval = 1f;
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
        MultiplySpawnTimer = 0;
        MultiplyTimer = 0;
        SpawnMultiply = false;
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
                BGM.Play();
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

            GameObject target = GameObject.Find("Multiply(Clone)");

            if (SpawnMultiply == false && !player.TimesTwo.activeInHierarchy && target == null && itemSpawner.SpawnStar == false)
            {
                MultiplySpawnTimer += Time.deltaTime;
            }

            if (timer > 1)
            {
                timer = 0;
                if (player.TimesTwo.activeInHierarchy)
                {
                    score += 2;
                }
                else
                {
                    score += 1;
                }
            }

            if (difficulty == "Easy")
            {
                if (MultiplySpawnTimer >= 60f)
                {
                    SpawnMultiply = true;
                    MultiplySpawnTimer = 0;
                    itemSpawner.SpawnStar = true;
                    itemSpawner.SpawnOne();
                }
            }
            if (difficulty == "Normal")
            {
                if (MultiplySpawnTimer >= 45f)
                {
                    SpawnMultiply = true;
                    MultiplySpawnTimer = 0;
                    itemSpawner.SpawnStar = true;
                    itemSpawner.SpawnOne();
                }
            }
            if (difficulty == "Hard")
            {
                if (MultiplySpawnTimer >= 30f)
                {
                    SpawnMultiply = true;
                    MultiplySpawnTimer = 0;
                    itemSpawner.SpawnStar = true;
                    itemSpawner.SpawnOne();
                }
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

                    if (gameSpeed == 2.5)
                    {
                        asteroidSpawner.spawnInterval = 2f;
                    }
                    if (gameSpeed == 3.5)
                    {
                        asteroidSpawner.spawnInterval = 1.5f;
                    }
                    if (gameSpeed == 4.5)
                    {
                        asteroidSpawner.spawnInterval = 1f;
                    }
                    if (gameSpeed == 6.5)
                    {
                        asteroidSpawner.spawnInterval = 0.7f;
                    }
                    if (gameSpeed == 7)
                    {
                        asteroidSpawner.spawnInterval = 0.5f;
                    }
                    if (gameSpeed == 9)
                    {
                        asteroidSpawner.spawnInterval = 0.3f;
                    }
                    if (gameSpeed == 10)
                    {
                        asteroidSpawner.spawnInterval = 0.2f;
                    }

                    
                }
                if (difficulty == "Normal")
                {
                    gameSpeed += 0.7f;
                    var main = WarpParticles[1].main;
                    main.simulationSpeed = gameSpeed;
                    DDATimer = DDAStartTimer;
                    asteroidSpawner.speedRange = new Vector2(asteroidSpawner.speedRange.x + 2, asteroidSpawner.speedRange.y + 2);
                    //itemSpawner.speedRange = new Vector2(itemSpawner.speedRange.x + 2, itemSpawner.speedRange.y + 2);
                    if (gameSpeed >= 11)
                    {
                        asteroidSpawner.spawnInterval = 0.2f;
                    }
                    else if (gameSpeed >= 7.5)
                    {
                        asteroidSpawner.spawnInterval = 0.3f;
                    }
                    else if (gameSpeed >= 5.3)
                    {
                        asteroidSpawner.spawnInterval = 0.4f;
                    }
                    else if (gameSpeed >= 3.2)
                    {
                        asteroidSpawner.spawnInterval = 0.7f;
                    }

                   
                }
                if (difficulty == "Hard")
                {
                    gameSpeed += 1f;
                    var main = WarpParticles[2].main;
                    main.simulationSpeed = gameSpeed;
                    DDATimer = DDAStartTimer;
                    asteroidSpawner.speedRange = new Vector2(asteroidSpawner.speedRange.x + 3, asteroidSpawner.speedRange.y + 3);
                    //itemSpawner.speedRange = new Vector2(itemSpawner.speedRange.x + 3, itemSpawner.speedRange.y + 3);

                    if (gameSpeed >= 15)
                    {
                        asteroidSpawner.spawnInterval = 0.2f;
                    }
                    else if (gameSpeed >= 10)
                    {
                        asteroidSpawner.spawnInterval = 0.3f;
                    }
                    else if (gameSpeed >= 7)
                    {
                        asteroidSpawner.spawnInterval = 0.5f;
                    }
                    else if (gameSpeed >= 5)
                    {
                        asteroidSpawner.spawnInterval = 0.7f;
                    }
                    else if (gameSpeed >= 3)
                    {
                        asteroidSpawner.spawnInterval = 0.9f;
                    }
                }

                

            }

            Score.text = score.ToString();

        }

       if (Menu.activeInHierarchy || GameOver.activeInHierarchy || EnterYourName.activeInHierarchy || HighScore.activeInHierarchy)
        {
            Cursor.visible = true;
        }
       else
        {
            Cursor.visible = false;
        }

       if (EnterYourName.activeInHierarchy && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            NextToLeaderboard();
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
        GOver.Play();
        FinalScore.text = "Final Score: " + score.ToString();

        names.Clear();
        bestScore.Clear();

        string dir = Path.Combine(Application.persistentDataPath, "GalacticGlide");

        if (difficulty == "Easy")
        {
            mode.text = "EASY";
            mode.color = Color.lightBlue;
            filePath = Path.Combine(dir, "EasyHighScore.txt");
        }
        if (difficulty == "Normal")
        {
            mode.text = "NORMAL";
            mode.color = Color.yellow;
            filePath = Path.Combine(dir, "NormalHighScore.txt");
        }
        if (difficulty == "Hard")
        {
            mode.text = "HARD";
            mode.color = Color.red;
            filePath = Path.Combine(dir, "HardHighScore.txt");
        }

        newHighScore = false;
        index = 0;
       
        if (File.Exists(filePath))
        {
           
            using (StreamReader sr = new StreamReader(filePath))
            {
                foreach (string a in File.ReadLines(filePath))
                {
                    string[] data = a.Split(" ");
                    names.Add(data[0]);
                    bestScore.Add(int.Parse(data[1]));
                }

                foreach (int a in bestScore)
                {
                    if (score > a)
                    {
                        newHighScore = true; 
                        break;
                    }
                    index++;
                }

                    
                    GameOverNext.SetActive(true);
                    GameOverRetry.SetActive(false);
                    GameOverQuit.SetActive(false);
                
         
            }
        }
        else
        {
            for(int i = 0; i < 5; i++)
            {
                names.Add("N/A");
                bestScore.Add(0);
            }
            
            GameOverNext.SetActive(true);
            GameOverRetry.SetActive(false);
            GameOverQuit.SetActive(false);
            
        }
    }

    public void NextToEnterName()
    {
        if (newHighScore)
        {
            GameOver.SetActive(false);
            BlackScreen.SetActive(false);
            EnterYourName.SetActive(true);
            NewRecordScreen.SetActive(true);
            NewRecord.Play();
        }
        else
        {
            GameOver.SetActive(false);
            BlackScreen.SetActive(false);
            HighScore.SetActive(true);
            NewRecordScreen.SetActive(true);
            DisplayScores();

        }
    }

    public void NextToLeaderboard()
    {
        if (PlayerName.text.Length > 12)
        {
            ErrorMessage.text = "Must enter a maximum of 12 characters";
            return;
        }
        else if (string.IsNullOrWhiteSpace(PlayerName.text))
        {
            ErrorMessage.text = "Must enter a name";
            return;
        }

        for (int j = 4; j > index; j--)
        {
            names[j] = names[j - 1];
            bestScore[j] = bestScore[j - 1];
        }

        ErrorMessage.text = "";
        names[index] = PlayerName.text;
        bestScore[index] = score;
        
        
        StreamWriter sw = new StreamWriter(filePath);

        for (int i = 0; i < 5; i++)
        {
            sw.WriteLine(names[i] + " " + bestScore[i]);
        }

        sw.Close();

        EnterYourName.SetActive(false);
        HighScore.SetActive(true);
        BlackScreen.SetActive(false);
        NewRecordScreen.SetActive(true);

        for (int i = 0; i < 5; i++)
        {
            if (names[i] == PlayerName.text)
            {
                BestNames[i].color = Color.yellow;
                BestScores[i].color = Color.yellow;
            }
            else
            {
                BestNames[i].color = Color.white;
                BestScores[i].color = Color.white;
            }

            BestNames[i].text = (i+1).ToString() + ". " + names[i];
            BestScores[i].text = bestScore[i].ToString();
        }

        names.Clear();
        bestScore.Clear();

    }

    public void NextToBackToGameOver()
    {
        HighScore.SetActive(false);
        GameOver.SetActive(true);
        GameOverNext.SetActive(false);
        GameOverRetry.SetActive(true);
        NewRecordScreen.SetActive(false);
        BlackScreen.SetActive(true);
        GameOverQuit.SetActive(true);
    }

    public void Resume()
    {
        Menu.SetActive(false);
        HUD[0].SetActive(true);
        HUD[1].SetActive(true);
        player.UpdateHealth(0);
        player.laserText.SetActive(true);
        if (player.mult)
        {
            player.TimesTwo.SetActive(true);
        }
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

    public void DisplayScores()
    {
        string dir = Path.Combine(Application.persistentDataPath, "GalacticGlide");

        if (difficulty == "Easy")
        {
            mode.text = "EASY";
            mode.color = Color.lightBlue;
            filePath = Path.Combine(dir, "EasyHighScore.txt");
        }
        if (difficulty == "Normal")
        {
            mode.text = "NORMAL";
            mode.color = Color.yellow;
            filePath = Path.Combine(dir, "NormalHighScore.txt");
        }
        if (difficulty == "Hard")
        {
            mode.text = "HARD";
            mode.color = Color.red;
            filePath = Path.Combine(dir, "HardHighScore.txt");
        }

        if (File.Exists(filePath))
        {

            using (StreamReader sr = new StreamReader(filePath))
            {
                foreach (string a in File.ReadLines(filePath))
                {
                    string[] data = a.Split(" ");
                    names.Add(data[0]);
                    bestScore.Add(int.Parse(data[1]));
                }


            }

            for (int i = 0; i < 5; i++)
            {
                BestNames[i].color = Color.white;
                BestScores[i].color = Color.white;
                BestNames[i].text = (i + 1).ToString() + ". " + names[i];
                BestScores[i].text = bestScore[i].ToString();
            }

            names.Clear();
            bestScore.Clear();
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                names.Add("N/A");
                bestScore.Add(0);
            }

            for (int i = 0; i < 5; i++)
            {
                BestNames[i].color = Color.white;
                BestScores[i].color = Color.white;
                BestNames[i].text = (i + 1).ToString() + ". " + names[i];
                BestScores[i].text = bestScore[i].ToString();
            }

            names.Clear();
            bestScore.Clear();

        }
    }
}
