using System.Collections;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

public class StartGameScreen : WindowBase
{
    [SerializeField] private Button playButton;
    public void Show(LevelConfig config)
    {
        playButton.OnClick(() => GameSession.Instance.StartGame(config));
    }
}
