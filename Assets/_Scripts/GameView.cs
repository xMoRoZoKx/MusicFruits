using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.CodeTools;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameView : MonoBehaviour
{
    [SerializeField] private Camera currentCamera;
    private float configSpeed => currentConfig != null ? currentConfig.fallSpeed : 10;
    private Vector3 spawnPoint;
    private Vector3 downPoint;
    private List<MusicObject> musicObjectsInScene = new List<MusicObject>();
    private List<MusicObject> obstaclesInScene = new List<MusicObject>();
    private float startOffset = 0;
    private float speed = 1;
    private LevelConfig currentConfig;
    private List<long> timeKeys;
    private float cameraDistance = 12;
    public float spawnHeight => Screen.height * 2;
    public float GetOffsetInSeconds(Vector3 positionTo)
    {
        return Mathf.Abs(spawnPoint.y - positionTo.y) / configSpeed;
    }
    private IEnumerator Start()
    {
        var config = GameSession.Instance.currentLevelConfig;

        spawnPoint = currentCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, spawnHeight, cameraDistance));
        downPoint = currentCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, cameraDistance));
        currentConfig = config;
        startOffset = GetOffsetInSeconds(downPoint);
        speed = configSpeed;
        timeKeys = config.GetTimes().timeKeysMilliseconds;
        SpawnPoints(config);
        yield return new WaitForSeconds(startOffset);
        var source = this.PlayAudio(config.clip);

        //TODO temp
        yield return new WaitForSeconds((timeKeys[timeKeys.Count - 1] / 1000));
        GameSession.Instance.ComliteLevel(config, true);
        source.Stop();
    }
    private async void SpawnPoints(LevelConfig config)
    {
        for (int i = 1; i < config.GetTimes().timeKeysMilliseconds.Count; i++)
        {
            long timeToNextKey = timeKeys[i] - timeKeys[i - 1];
            var task = TaskTools.WaitForMilliseconds(timeToNextKey, false);
            if (task == null) return;

            var currentTimeKey = timeKeys[i];
            var prefab = config.randomObjects.GetRandom();
            if (prefab == null)
            {
                Debug.LogError(config.name + " havent music objects");
                return;
            }
            musicObjectsInScene.Add(SpawnObject(prefab, newObj =>
            {
                newObj.AddEvent(EventTriggerType.PointerEnter, eventData =>
                {
                    DestroyMusicObject(newObj);
                });
                newObj.timeKey = currentTimeKey;
            }));

            RandomTools.InvokWithChance(() => SpawnObject(config.randomObstacles.GetRandom(), obstacle =>
            {
                //TODO code
                obstaclesInScene.Add(obstacle);
                TaskTools.Wait(10, () =>
                {
                    obstaclesInScene.Remove(obstacle);
                    Destroy(obstacle?.gameObject);
                });
            }), config.obstacleChance);

            await task;
        }
    }
    public MusicObject SpawnObject(MusicObject prefab, Action<MusicObject> onSpawned = null)
    {
        if (prefab == null) return null;

        var frame = Screen.width / 10;
        var randomSpawnPoint = currentCamera.ScreenToWorldPoint(new Vector3(UnityEngine.Random.Range(0 + frame, Screen.width - frame), spawnHeight, cameraDistance));
        var newObj = Instantiate(prefab, randomSpawnPoint, Quaternion.identity);
        onSpawned?.Invoke(newObj);
        return newObj;
    }
    private void DestroyMusicObject(MusicObject musicObject)
    {
        musicObjectsInScene.Remove(musicObject);
        Destroy(musicObject.gameObject);
    }
    private void FixedUpdate()
    {
        CalculateSpeed();
        for (int i = 0; i < musicObjectsInScene.Count; i++)
        {
            musicObjectsInScene[i].transform.Move(0, -speed * Time.fixedDeltaTime, 0);
            if (musicObjectsInScene[i].transform.position.y < downPoint.y)
            {
                DestroyMusicObject(musicObjectsInScene[i]);
                i--;
            }
        }
        obstaclesInScene.ForEach(o => o.transform.Move(0, -5 * Time.fixedDeltaTime, 0));
    }
    public Vector3 GetCurrentTouchPosition()
    {
        var position = Vector3.zero;
#if UNITY_EDITOR
        position = currentCamera.ScreenToWorldPoint(Input.mousePosition);
#else
                if (Input.touchCount != 0) position = currentCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
                else position = downPoint;
#endif
        return position;
    }
    public void CalculateSpeed()
    {
        speed = configSpeed * (GetOffsetInSeconds(GetCurrentTouchPosition()) / startOffset);
    }
}
