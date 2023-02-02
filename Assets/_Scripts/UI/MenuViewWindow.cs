using System.Collections;
using System.Collections.Generic;
using Game.UI;
using Tools;
using UITools;
using UnityEngine;

public class MenuViewWindow : WindowBase
{
    [SerializeField] private LevelConfigView levelViewPrefab;
    [SerializeField] private RectTransform container;

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
                GameSession.Instance.StartGame(config);
            });
        });
    }
}
