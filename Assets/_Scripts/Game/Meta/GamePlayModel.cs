using System;
using System.Collections;
using System.Collections.Generic;
using Game.CodeTools;
using UnityEngine;

public class GamePlayModel
{
    public GamePlayModel(LevelConfig config)
    {
        levelConfig = config;
        player = new PlayerInstance();
        timeKeys = levelConfig.GetTimes().timeKeysMilliseconds;
        currentFallSpeed = coinsSpeed;
    }
    public PlayerInstance player { get; private set; }
    public LevelConfig levelConfig { get; private set; }
    public List<long> timeKeys { get; private set; }
    public float coinsSpeed => 7;
    public float obstaclesSpeed => 5;
    public float configSpeed => levelConfig != null ? levelConfig.fallSpeed : 10;
    public float currentFallSpeed = 1;
    public int CountCatchedCoins;
}
public class PlayerInstance
{
    public Reactive<int> Hp { get; private set; } = new Reactive<int>(3);
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

