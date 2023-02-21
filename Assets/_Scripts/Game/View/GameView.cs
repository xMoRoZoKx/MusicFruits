using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.CodeTools;
using Game.UI;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class GameView : MonoBehaviour
{
    [SerializeField] private Camera currentCamera;
    [SerializeField] private BaseAnimationBG animationBG;
    private GamePlayModel model => GameSession.Instance.gamePlayModel;
    private LevelConfig currentConfig => model.levelConfig;
    private Reactive<float> progress => model.progress;
    private List<long> timeKeys => model.timeKeys;
    private PlayerInstance player => model.player;
    private AudioSource source;
    private ParticleSystem particle;
    public Vector2 defaultCursorPosition => Vector2.zero;
    private float cameraDistance = 15;
    private bool isPause;
    private bool isLevelComplete;
    public float GetOffsetInSeconds(Vector3 positionTo)
    {
        return Mathf.Abs(spawnPoint.y - positionTo.y) / configSpeed;
    }
    private void Start()
    {
        if (model == null) GameSession.Instance.LoadMenu();
        Init();

        StartCoroutine(StartGame());
    }
    public IEnumerator StartGame()
    {
        SpawnWithRhythm(currentConfig);
        yield return new WaitForSeconds(startOffset);
        source = this.PlayAudio(currentConfig.clip);
        progress.Subscribe(value =>
        {
            if (value - 1 <= 0.01f || isLevelComplete) return;

            if (!player.IsDead) SpawnCoins();
            else GameSession.Instance.ComliteLevel(false);

            source.Pause();
            isLevelComplete = true;
        });
    }
    public void Init()
    {
        player.OnDead = () =>
        {
            WindowManager.instance.Show<LosingGameScreen>().Show(
            onContinue: () =>
            {
                player.Resurrect();
                Pause(false);
            },
            onTimerEnd: () =>
            {
                GameSession.Instance.ComliteLevel(false);
            });
            Pause(true);
        };

        spawnPoint = currentCamera.ScreenToWorldPoint(spawnScreenPoint);
        downPoint = currentCamera.ScreenToWorldPoint(downScreenPoint);

        startOffset = GetOffsetInSeconds(downPoint);
        particle = Instantiate(currentConfig.particle, downPoint, Quaternion.identity);

        WindowManager.instance.Show<GameHUDScreen>().Show(player.Hp, progress);
    }

    private void FixedUpdate()
    {
        if (isPause || model == null) return;
        Vector3 touchPos = GetTouchWorldPosition();
        CalculateProgress();
        CalculateSpeed(touchPos);
        MoveCursor(touchPos);
        MoveMusicObjs();
        MoveObstacles();
        MoveCoins();
    }
    public void Pause(bool value)
    {
        if (value)
        {
            source.Pause();
            spawnTaskAwaiter.Pause();
        }
        else
        {
            source.Play();
            spawnTaskAwaiter.Resume();
        }
        isPause = value;
    }
    public Vector2 GetTouchScreenPosition()
    {
        var position = defaultCursorPosition;
#if UNITY_EDITOR
        position = Input.mousePosition;
#else
        if (Input.touchCount != 0) position = Input.GetTouch(0).position;
        else position = defaultCursorPosition;
#endif
        return position;
    }
    public Vector3 GetTouchWorldPosition() => currentCamera.ScreenToWorldPoint(GetTouchScreenPosition());
    public void CalculateProgress()
    {
        if (source == null) return;
        progress.value = source.time / timeKeys[timeKeys.Count - 1].MillisecondsToSeconds();
    }
}