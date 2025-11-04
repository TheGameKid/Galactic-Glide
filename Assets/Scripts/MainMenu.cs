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
    public GameObject EasySelect;
    public GameObject NormalSelect;
    public GameObject HardSelect;
    public GameObject LevelBackground;
    public GameObject EasyBackground;
    public GameObject NormalBackground;
    public GameObject HardBackground;
    public GameObject LeaderBoardBackground;
    public GameObject HowToPlayEasy;
    public GameObject HowToPlayNormal;
    public GameObject HowToPlayHard;
    public GameObject PanelEasy;
    public GameObject PanelNormal;
    public GameObject PanelHard;
    public GameObject Leaderboard;
    public GameObject HighScores;
    public TextMeshProUGUI mode;
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
    EasySelect.SetActive(false);
    NormalSelect.SetActive(false);
    HardSelect.SetActive(false);
    LevelBackground.SetActive(true);
    EasyBackground.SetActive(false);
    NormalBackground.SetActive(false);
    HardBackground.SetActive(false);
    HowToPlayEasy.SetActive(false);
    HowToPlayNormal.SetActive(false);
    HowToPlayHard.SetActive(false);
    PanelEasy.SetActive(false);
    PanelNormal.SetActive(false);
    PanelHard.SetActive(false);
    Leaderboard.SetActive(false);
    HighScores.SetActive(false);
    LeaderBoardBackground.SetActive(false);
    difficulty = "";
    }

    public void EasyButton()
    {
        EasyBackground.SetActive(true);
        EasySelect.SetActive(true);
        PanelEasy.SetActive(true);
        LevelSelect.SetActive(false);
        LevelBackground.SetActive(false);
        difficulty = "Easy";
    }

    public void NormalButton()
    {
        NormalBackground.SetActive(true);
        NormalSelect.SetActive(true);
        PanelNormal.SetActive(true);
        LevelSelect.SetActive(false);
        LevelBackground.SetActive(false);
        difficulty = "Normal";
    }
    public void HardButton()
    {
        HardBackground.SetActive(true);
        HardSelect.SetActive(true);
        PanelHard.SetActive(true);
        LevelSelect.SetActive(false);
        LevelBackground.SetActive(false);
        difficulty = "Hard";
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
        if (difficulty == "Easy")
        {
            PanelEasy.SetActive(false);
            HowToPlayEasy.SetActive(true);
        }
        if (difficulty == "Normal")
        {
            PanelNormal.SetActive(false);
            HowToPlayNormal.SetActive(true);
        }
        if (difficulty == "Hard")
        {
            PanelHard.SetActive(false);
            HowToPlayHard.SetActive(true);
        }

    
    }
    public void BackButton()
    {
        if (difficulty == "Easy")
        {
            PanelEasy.SetActive(true);
            HowToPlayEasy.SetActive(false);
        }
        if (difficulty == "Normal")
        {
            PanelNormal.SetActive(true);
            HowToPlayNormal.SetActive(false);
        }
        if (difficulty == "Hard")
        {
            PanelHard.SetActive(true);
            HowToPlayHard.SetActive(false);
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
