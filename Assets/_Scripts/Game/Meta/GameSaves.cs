
using System.Linq;
using Game.CodeTools;
using Tools.PlayerPrefs;
using UnityEngine;

public class GameSaves : Singleton<GameSaves>
{
    private string key = "_gg_gg_gg";
    public GameSaves()
    {
        Coins.ConnectToSaver(key + nameof(Coins));
        Stars.ConnectToSaver(key + nameof(Stars));
        Records.ConnectToSaver(key + nameof(Records));
    }
    public Reactive<int> Coins { private set; get; } = new Reactive<int>();
    public Reactive<int> Stars { private set; get; } = new Reactive<int>();
    public ReactiveList<(string, float)> Records = new ReactiveList<(string, float)>();

}
public static class SaveUtils
{
    public static ReactiveList<(string, float)> records => GameSaves.Instance.Records;
    public static float GetRecord(this LevelConfig levelConfig)
    {
        return records.Find(value => levelConfig.name == value.Item1).Item2;
    }
    public static void SetRecord(this LevelConfig levelConfig, float value)
    {
        if (records.Any(value => levelConfig.name == value.Item1))
        {
            records[records.FindIndex(value => levelConfig.name == value.Item1)] = (levelConfig.name, value);
            return;
        }
        records.Add((levelConfig.name, value));
    }
}
