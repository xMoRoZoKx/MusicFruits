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
    private AudioSource source;
    private LevelConfig currentConfig;
    private List<long> timeKeys;
    private float cameraDistance = 15;
    private ParticleSystem particle;
    public Vector2 defaultCursorPosition => Vector2.zero;
    private PlayerInstance player;
    private Reactive<float> progress = new Reactive<float>();
    private bool isPause;
    private bool isLevelComplete;
    public float GetOffsetInSeconds(Vector3 positionTo)
    {
        return Mathf.Abs(spawnPoint.y - positionTo.y) / configSpeed;
    }
    private void Start()
    {
        Init(GameSession.Instance.currentLevelConfig);

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
    public void Init(LevelConfig config)
    {
        player = new PlayerInstance();
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

        currentConfig = config;

        spawnPoint = currentCamera.ScreenToWorldPoint(spawnScreenPoint);
        downPoint = currentCamera.ScreenToWorldPoint(downScreenPoint);

        startOffset = GetOffsetInSeconds(downPoint);
        speed = configSpeed;
        timeKeys = config.GetTimes().timeKeysMilliseconds;
        particle = Instantiate(config.particle, downPoint, Quaternion.identity);

        WindowManager.instance.Show<GameHUDScreen>().Show(player.Hp, progress);
    }

    private void FixedUpdate()
    {
        if (isPause) return;
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
public class PlayerInstance
{
    public Reactive<int> Hp { get; private set; } = new Reactive<int>(3);
    public int CountCatchedCoins;
    public bool IsDead => Hp.value <= 0;
    public bool HaveShield;
    public void Damage(int damageCount)
    {
        if (HaveShield) return;
        Hp.value -= damageCount;
        if (Hp.value <= 0)
        {
            OnDead?.Invoke();
        }
    }
    public void Resurrect()
    {
        Hp.value = 3;
    }
    public Action OnDead;
}
