using System;
using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;
using static TaskTools;

public partial class GameView
{
    public float spawnHeight => Screen.height * 2;
    private Vector2 spawnScreenPoint => new Vector3(Screen.width / 2, spawnHeight, cameraDistance);
    private Vector2 downScreenPoint => new Vector3(Screen.width / 2, -1, cameraDistance);
    private Vector3 spawnPoint;
    private Vector3 downPoint;
    private List<MusicObject> musicObjectsInScene = new List<MusicObject>();
    private List<MusicObject> obstaclesInScene = new List<MusicObject>();
    private List<MusicObject> coinsInScene = new List<MusicObject>();
    private float startOffset = 0;
    private TaskWaiting spawnTaskAwaiter;
    private async void SpawnWithRhythm(LevelConfig config)
    {
        long difference = 0;
        spawnTaskAwaiter = new TaskWaiting();
        for (int i = 1; i < config.GetTimes().timeKeysMilliseconds.Count - 1; i++)
        {
            var currentkey = source == null ? timeKeys[i] : source.time.SecondsToMilliseconds();
            long timeToNextKey = timeKeys[i] - timeKeys[i - 1] - difference;
            var task = spawnTaskAwaiter.WaitForMilliseconds(timeToNextKey);//TaskTools.WaitForMilliseconds(timeToNextKey, false);
            if (task == null) return;

            SpawnMusicObject(config, timeKeys[i]);
            SpawnObstacle(config);

            long timeToWait = source == null ? 0 : source.time.SecondsToMilliseconds();// temp problem
            await task;
            difference = source == null || timeToWait == 0 ? 0 : source.time.SecondsToMilliseconds() - timeToWait - timeToNextKey;
        }
    }
    private void SpawnCoins()
    {
        int countInLine = 5;
        int countInRow = 10;
        float cameraDistance = 10;
        Vector3 spawnPos = spawnScreenPoint;
        spawnPos = spawnPos.WithZ(cameraDistance);

        float spacing = Screen.height / countInRow;
        for (int i = 0; i < currentConfig.prize / countInLine; i++)
        {
            spawnPos = spawnPos.WithX(0).WithY(spawnPos.y + spacing);
            SpawnLine(countInLine);
        }
        spawnPos = spawnPos.WithX(0).WithY(spawnPos.y + spacing);
        SpawnLine(currentConfig.prize % countInLine);

        void SpawnLine(int count)
        {
            float spacing = Screen.width / (count + 1);
            for (int j = 1; j < count + 1; j++)
            {
                spawnPos = spawnPos.WithX(spacing * j);
                var newObj = Instantiate(currentConfig.randomCoins.GetRandom(), currentCamera.ScreenToWorldPoint(spawnPos), Quaternion.identity);
                newObj.OnCatch(() =>
                {
                    model.CountCatchedCoins++;
                    coinsInScene.Remove(newObj);
                });
                newObj.OnDestroyEvent = () =>
                {
                    if (coinsInScene.Count == 0) GameSession.Instance.ComliteLevel(true);
                };
                coinsInScene.Add(newObj);
            }
        }

    }
    public void SpawnMusicObject(LevelConfig config, long currentTimeKey)
    {
        var prefab = config.randomObjects.GetRandom();
        if (prefab == null)
        {
            Debug.LogError(config.name + " havent music objects");
            return;
        }
        musicObjectsInScene.Add(SpawnObject(prefab, newObj =>
        {
            newObj.OnCatch(() =>
            {
                animationBG.StartDanceAnimation();
                musicObjectsInScene.Remove(newObj);
            });
            newObj.timeKey = currentTimeKey;
        }));
    }
    public void SpawnObstacle(LevelConfig config)
    {
        RandomTools.InvokWithChance(() => SpawnObject(config.randomObstacles.GetRandom(), obstacle =>
        {
            obstacle.OnCatch(() =>
            {
                obstaclesInScene.Remove(obstacle);
                player.Damage(player.Hp.value);
            });
            obstaclesInScene.Add(obstacle);
            TaskTools.Wait(10, () =>
            {
                obstaclesInScene.Remove(obstacle);
                if (obstacle != null) Destroy(obstacle.gameObject);
            });
        }), config.obstacleChance);
    }
    public MusicObject SpawnObject(MusicObject prefab, Action<MusicObject> onSpawned = null)
    {
        if (prefab == null) return null;

        var frame = Screen.width / 7;
        var randomSpawnPoint = currentCamera.ScreenToWorldPoint(new Vector3(UnityEngine.Random.Range(0 + frame, Screen.width - frame), spawnHeight, cameraDistance));
        var newObj = Instantiate(prefab, randomSpawnPoint, Quaternion.identity);
        onSpawned?.Invoke(newObj);
        return newObj;
    }
}
