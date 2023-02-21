
using System.Linq;
using Game.CodeTools;
using Tools.PlayerPrefs;
using UnityEngine;
[System.Serializable]
public class Record
{
    public string levelName;
    public float progress;
}
public class GameSaves : Singleton<GameSaves>
{
    private string key = "_gg_gg_gg";
    public GameSaves()
    {
        Coins.ConnectToSaver(key + nameof(Coins));
        Records.ConnectToSaver(key + nameof(Records));
        Stars.ConnectToSaver(key + nameof(Stars));
    }
    public Reactive<int> Coins { private set; get; } = new Reactive<int>();
    public Reactive<int> Stars { private set; get; } = new Reactive<int>();
    public ReactiveList<Record> Records = new ReactiveList<Record>();

}
public static class SaveUtils
{
    public static ReactiveList<Record> records => GameSaves.Instance.Records;
    public static float GetRecord(this LevelConfig levelConfig)
    {
        var record = records.Find(value => levelConfig.levelName == value.levelName);
        return record == null ? 0 : record.progress;
    }
    public static void SetRecord(this LevelConfig levelConfig, float progress)
    {
        if (records.Any(value => levelConfig.levelName == value.levelName))
        {
            records[records.FindIndex(value => levelConfig.levelName == value.levelName)].progress = progress;
            return;
        }
        records.Add(new Record() { levelName = levelConfig.levelName, progress = progress });
    }
}
