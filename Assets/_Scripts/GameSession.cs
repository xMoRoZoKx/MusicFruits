using System.Collections;
using System.Collections.Generic;
using Game.CodeTools;
using Game.UI;
using Tools.PlayerPrefs;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameSession : Singleton<GameSession>
{
    public const string FIRST_START_KEY = "FIRST_START_KEY"; 
    public bool IsFirstStart => !PlayerPrefsPro.Get<bool>(FIRST_START_KEY); 
    public GameSession()
    {
        Application.targetFrameRate = 60;
        if(IsFirstStart)
        {
            FirstStart();
            PlayerPrefsPro.Set(FIRST_START_KEY, true);
        }
    }
    public void FirstStart()
    {
        GameSaves.Instance.Coins.value = 100;
    }
    [HideInInspector] public LevelConfig currentLevelConfig { get; private set; }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        WindowManager.instance?.CloseAll();
    }
    public void LoadMenu()
    {
        LoadScene("MenuScene");
    }
    public void StartGame(LevelConfig config)
    {
        currentLevelConfig = config;
        LoadScene("GameScene");
    }
    public void ComliteLevel(bool isWon, int coins = 0)
    {
        if (currentLevelConfig == null) return;
        GameSaves.Instance.Coins.value += coins;


        LoadMenu();
        WindowManager.instance.Show<StartGameScreen>().Show(currentLevelConfig);
        currentLevelConfig = null;
    }
}
