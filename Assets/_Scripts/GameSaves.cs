
using Game.CodeTools;
using Tools.PlayerPrefs;
using UnityEngine;

public class GameSaves : Singleton<GameSaves>
{
    private string key = "_gg_gg_gg";
    public GameSaves()
    {
        Coins.ConnectToSaver(key + nameof(Coins));
    }
    public Reactive<int> Coins { private set; get; } = new Reactive<int>();
}
