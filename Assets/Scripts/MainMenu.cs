using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public GameObject LevelSelect;
    public GameObject ModeSelect;
    //public GameObject NormalSelect;
    //public GameObject HardSelect;
    public GameObject LevelBackground;
    public GameObject EasyBackground;
    public GameObject NormalBackground;
    public GameObject HardBackground;
    public GameObject LeaderBoardBackground;
    public GameObject HowToPlay;
    //public GameObject HowToPlayEasy2;
    //public GameObject HowToPlayNormal;
    //public GameObject HowToPlayNormal2;
    //public GameObject HowToPlayHard;
    //public GameObject HowToPlayHard2;
    public GameObject Panel;
   // public GameObject PanelNormal;
    //public GameObject PanelHard;
    public GameObject Leaderboard;
    public GameObject HighScores;
    public TextMeshProUGUI mode;
    public TextMeshProUGUI PanelTitle;
    public string difficulty;
    public string filePath;

    public List<string> names = new List<string>();
    public List<int> bestScore = new List<int>();

    public TextMeshProUGUI[] BestNames = new TextMeshProUGUI[5];
    public TextMeshProUGUI[] BestScores = new TextMeshProUGUI[5];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ResetStuff();
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetStuff()
    {
    LevelSelect.SetActive(true);
    ModeSelect.SetActive(false);
    LevelBackground.SetActive(true);
    EasyBackground.SetActive(false);
    NormalBackground.SetActive(false);
    HardBackground.SetActive(false);
    HowToPlay.SetActive(false);
    Panel.SetActive(false);
    Leaderboard.SetActive(false);
    HighScores.SetActive(false);
    LeaderBoardBackground.SetActive(false);
    difficulty = "";
    PanelTitle.text = "";
    }

    public void EasyButton()
    {
        EasyBackground.SetActive(true);
        ModeSelect.SetActive(true);
        Panel.SetActive(true);
        LevelSelect.SetActive(false);
        LevelBackground.SetActive(false);
        difficulty = "Easy";
        PanelTitle.text = "EASY";

    }

    public void NormalButton()
    {
        NormalBackground.SetActive(true);
        ModeSelect.SetActive(true);
        Panel.SetActive(true);
        LevelSelect.SetActive(false);
        LevelBackground.SetActive(false);
        difficulty = "Normal";
        PanelTitle.text = "NORMAL";
    }
    public void HardButton()
    {
        HardBackground.SetActive(true);
        ModeSelect.SetActive(true);
        Panel.SetActive(true);
        LevelSelect.SetActive(false);
        LevelBackground.SetActive(false);
        difficulty = "Hard";
        PanelTitle.text = "HARD";
    }
    public void LeaderBoard()
    {
        LevelSelect.SetActive(false);
        LevelBackground.SetActive(false);
        LeaderBoardBackground.SetActive(true);
        Leaderboard.SetActive(true);
    }
    public void EasyBoard()
    {
        Leaderboard.SetActive(false);
        HighScores.SetActive(true);
        difficulty = "Easy";
        DisplayScores();

    }
    public void NormalBoard()
    {
        Leaderboard.SetActive(false);
        HighScores.SetActive(true);
        difficulty = "Normal";
        DisplayScores();

    }
    public void HardBoard()
    {
        Leaderboard.SetActive(false);
        HighScores.SetActive(true);
        difficulty = "Hard";
        DisplayScores();

    }
    public void BackToLeaderBoard()
    {
        Leaderboard.SetActive(true);
        HighScores.SetActive(false);
    }
    public void BacktoMainMenu()
    {
        ResetStuff();
    }
    public void NextButton()
    {
      
            Panel.SetActive(false);
            HowToPlay.SetActive(true);
        

    
    }


    public void BackButton()
    {
        Panel.SetActive(true);
        HowToPlay.SetActive(false);
     
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

    public void Go()
    {
        // Write to a per-user, OS-safe location
        string dir = Path.Combine(Application.persistentDataPath, "GalacticGlide");
        Directory.CreateDirectory(dir); // ensure folder exists

        string filePath = Path.Combine(dir, "Mode.txt");
        File.WriteAllText(filePath, difficulty);

        SceneManager.LoadScene("Game");
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
