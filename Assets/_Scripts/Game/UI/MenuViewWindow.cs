using System.Collections;
using System.Collections.Generic;
using Game.UI;
using TMPro;
using Tools;
using UITools;
using UnityEngine;
using DG.Tweening;

public class MenuViewWindow : WindowBase
{
    [SerializeField] private LevelConfigView levelViewPrefab;
    [SerializeField] private RectTransform menuScreen, viewContainer, container;
    [SerializeField] private TextMeshProUGUI coinsCounter, starsCounter;
    private void Start()
    {
        Show();
    }
    public void Show()
    {
        menuScreen.DOAnchorPos(new Vector2(0, 0), 0.35f);
        GameSaves.Instance.Coins.SubscribeAndInvoke(value => 
        {
            coinsCounter.text = value.ToString();
        });
        
        GameSaves.Instance.Stars.SubscribeAndInvoke(value => 
        {
            starsCounter.text = value.ToString();
        });
    }
    public void ShowLevels()
    {
        menuScreen.DOAnchorPos(new Vector2(-1000, 0), 0.35f);
        viewContainer.DOAnchorPos(new Vector2(0, 0), 0.35f);

        GameConfigs.Instance.AllLevels.Present(levelViewPrefab, container, (view, config) =>
        {
            view.Show(config, onClick: () =>
            {
                // WindowManager.instance.Show<StartGameScreen>().Show(config);
                GameSession.Instance.StartGame(config);
            });
        });
    }
    public void ShowMenu()
    {
        menuScreen.DOAnchorPos(new Vector2(-1000, 0), 0.35f);
        viewContainer.DOAnchorPos(new Vector2(0, 0), 0.35f);
    }
}
