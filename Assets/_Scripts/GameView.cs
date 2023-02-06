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
    [SerializeField] private BaseAnimationBG animationBG;
    private AudioSource source;
    private float configSpeed => currentConfig != null ? currentConfig.fallSpeed : 10;
    private Vector3 spawnPoint;
    private Vector3 downPoint;
    private List<MusicObject> musicObjectsInScene = new List<MusicObject>();
    private List<MusicObject> obstaclesInScene = new List<MusicObject>();
    private float startOffset = 0;
    private float speed = 1;
    private LevelConfig currentConfig;
    private List<long> timeKeys;
    private float cameraDistance = 15;
    private ParticleSystem particle;
    public float spawnHeight => Screen.height * 2;
    public Vector2 defaultCursorPosition => Vector2.zero;
    public float GetOffsetInSeconds(Vector3 positionTo)
    {
        return Mathf.Abs(spawnPoint.y - positionTo.y) / configSpeed;
    }
    private IEnumerator Start()
    {
        var config = GameSession.Instance.currentLevelConfig;
        currentConfig = config;

        spawnPoint = currentCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, spawnHeight, cameraDistance));
        downPoint = currentCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, 0, cameraDistance));

        startOffset = GetOffsetInSeconds(downPoint);
        speed = configSpeed;
        timeKeys = config.GetTimes().timeKeysMilliseconds;
        particle = Instantiate(config.particle, downPoint, Quaternion.identity);


        SpawnPoints(config);
        yield return new WaitForSeconds(startOffset);
        source = this.PlayAudio(config.clip);

        //TODO temp
        yield return new WaitForSeconds((timeKeys[timeKeys.Count - 1] / 1000));
        GameSession.Instance.ComliteLevel(config, true);
        source.Stop();
    }
    private async void SpawnPoints(LevelConfig config)
    {
        for (int i = 1; i < config.GetTimes().timeKeysMilliseconds.Count - 1; i++)
        {
            long timeToNextKey = timeKeys[i] - timeKeys[i - 1];
            var task = TaskTools.WaitForMilliseconds(timeToNextKey, false);
            if (task == null) return;

            SpawnMusicObject(config, timeKeys[i]);
            SpawnObstacle(config);

            await task;
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
            newObj.OnCatch(eventData =>
            {
                animationBG.StartDanceAnimation(0.3f);
                musicObjectsInScene.Remove(newObj);
                newObj.Catch();
            });
            newObj.timeKey = currentTimeKey;
        }));
    }
    public void SpawnObstacle(LevelConfig config)
    {
        RandomTools.InvokWithChance(() => SpawnObject(config.randomObstacles.GetRandom(), obstacle =>
        {
            //TODO code
            obstaclesInScene.Add(obstacle);
            TaskTools.Wait(10, () =>
            {
                obstaclesInScene.Remove(obstacle);
                Destroy(obstacle.gameObject);
            });
        }), config.obstacleChance);
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
    private void FixedUpdate()
    {
        Vector3 touchPos = GetTouchWorldPosition();

        CalculateSpeed(touchPos);
        MoveCursor(touchPos);
        MoveMusicObjs();
        MoveObstacles();
    }
    public void MoveCursor(Vector3 touchPos)
    {
        if (Input.touchCount == 0) particle.SetActive(false);
        else
        {
            particle.transform.Teleportation(touchPos.WithZ(2f));
            particle.SetActive(true);
        }
    }
    public void MoveObstacles()
    {
        obstaclesInScene.ForEach(o => o.transform.Move(0, -5 * Time.fixedDeltaTime, 0));
    }
    public void MoveMusicObjs()
    {
        for (int i = 0; i < musicObjectsInScene.Count; i++)
        {
            musicObjectsInScene[i].transform.Move(0, -speed * Time.fixedDeltaTime, 0);
            if (musicObjectsInScene[i].transform.position.y < downPoint.y)
            {
                Destroy(musicObjectsInScene[i].gameObject);
                musicObjectsInScene.Remove(musicObjectsInScene[i]);
                i--;
            }
        }
    }
    public Vector2 GetTouchScreenPosition()
    {
        var position = defaultCursorPosition;
#if UNITY_EDITOR
        position = Input.mousePosition;
#else
        if (Input.touchCount != 0) position = Input.GetTouch(0).position;
        else position = defaultCursorPosition;//.WithY(-10);
#endif
        return position;
    }
    public Vector3 GetTouchWorldPosition() => currentCamera.ScreenToWorldPoint(GetTouchScreenPosition());
    public void CalculateSpeed(Vector3 touchPos)
    {
        speed = configSpeed * (GetOffsetInSeconds(touchPos) / startOffset);
    }
}
