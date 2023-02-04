using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/LevelConfig", fileName = "NewLevelConfig")]
public class LevelConfig : ScriptableObject
{
    public string levelName => name;
    public string timeKeysJson;
    public AudioClip clip;
    public Sprite icon;
    public int fallSpeed = 10;
    public List<MusicObject> randomObjects;
    public List<MusicObject> randomObstacles;
    public ParticleSystem particle;
    public int obstacleChance = 10;

    public TimeKeys GetTimes() => JsonUtility.FromJson<TimeKeys>(timeKeysJson);
}