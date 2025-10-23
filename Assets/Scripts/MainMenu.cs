using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

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
    public GameObject HowToPlayEasy;
    public GameObject HowToPlayNormal;
    public GameObject HowToPlayHard;
    public GameObject PanelEasy;
    public GameObject PanelNormal;
    public GameObject PanelHard;
    public string difficulty;
    
    
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
