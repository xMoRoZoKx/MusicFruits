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
    public GamePlayModel gamePlayModel;
    public GameSession()
    {
        Application.targetFrameRate = 60;
        if (IsFirstStart)
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
        gamePlayModel = new GamePlayModel(config);
        LoadScene("GameScene");
    }
    public void ComliteLevel(bool isWon)
    {
        if (gamePlayModel == null) return;
        GameSaves.Instance.Coins.value += gamePlayModel.CountCatchedCoins;
        UpdateRecord(gamePlayModel.levelConfig, gamePlayModel.progress.value * 100);
        LoadMenu();
        WindowManager.instance.Show<StartGameScreen>().Show(gamePlayModel.levelConfig);
        gamePlayModel = null;
    }
    private void UpdateRecord(LevelConfig config, float progressInPercent)
    {
        foreach (var starPercent in config.starPlacesInPercent)
        {
            if (config.GetRecord() < starPercent && progressInPercent > starPercent)
            {
                GameSaves.Instance.Stars.value++;
            }
        }
        if (progressInPercent > config.GetRecord())
        {
            config.SetRecord(progressInPercent);
        }
    }
}
