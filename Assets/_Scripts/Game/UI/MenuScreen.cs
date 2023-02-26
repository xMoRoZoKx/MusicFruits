using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuScreen : WindowBase
{
    [SerializeField] private Button playButton, levelButton;
    [SerializeField] private TextMeshProUGUI coinsCounter, starsCounter;
    private void Start()
    {
        Show(GameConfigs.Instance.AllLevels[0]);
    }
    public void Show(LevelConfig config)
    {
        playButton.OnClick(() => GameSession.Instance.StartGame(config));
        levelButton.OnClick(() => WindowManager.instance.Show<LevelsScreen>().Show());
        GameSaves.Instance.Coins.SubscribeAndInvoke(value =>
        {
            coinsCounter.text = value.ToString();
        });

        GameSaves.Instance.Stars.SubscribeAndInvoke(value =>
        {
            starsCounter.text = value.ToString();
        });
    }
}
