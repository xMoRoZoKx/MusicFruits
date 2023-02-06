using System.Collections;
using System.Collections.Generic;
using Game.CodeTools;
using Game.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class GameSession : Singleton<GameSession>
{
    public GameSession()
    {
        Application.targetFrameRate = 60;
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
        LoadMenu();
        WindowManager.instance.Show<StartGameScreen>().Show(currentLevelConfig);
        currentLevelConfig = null;
    }
    public void GiveReward(LevelConfig config)
    {

    }
}
