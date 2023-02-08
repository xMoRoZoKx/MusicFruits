
using Game.CodeTools;
using Tools.PlayerPrefs;
using UnityEngine;

public class GameSaves : Singleton<GameSaves>
{
    private string key = "_gg_gg_gg";
    public GameSaves()
    {
        SubscribeToSave(Coins, key + nameof(Coins));
    }
    private void SubscribeToSave<T>(Reactive<T> element, string key)
    {
        element.value = PlayerPrefsPro.Get<T>(key);
        element.Subscribe(value => PlayerPrefsPro.Set(key, value));
    }
    public Reactive<int> Coins { private set; get; } = new Reactive<int>();
}
