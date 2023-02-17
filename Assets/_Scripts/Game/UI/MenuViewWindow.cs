using System.Collections;
using System.Collections.Generic;
using Game.UI;
using TMPro;
using Tools;
using UITools;
using UnityEngine;

public class MenuViewWindow : WindowBase
{
    [SerializeField] private LevelConfigView levelViewPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private TextMeshProUGUI coinsCounter;
    private void Start()
    {
        Show();
    }
    public void Show()
    {
        GameConfigs.Instance.AllLevels.Present(levelViewPrefab, container, (view, config) =>
        {
            view.Show(config, onClick: () =>
            {
                // WindowManager.instance.Show<StartGameScreen>().Show(config);
                GameSession.Instance.StartGame(config);
            });
        });
        GameSaves.Instance.Coins.SubscribeAndInvoke(value => 
        {
            coinsCounter.text = value.ToString();
        });

    }
}
