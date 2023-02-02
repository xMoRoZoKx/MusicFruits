using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.CodeTools;
using UnityEngine;

public class GameConfigs : Singleton<GameConfigs>
{
    public GameConfigs()
    {
        AllLevels = ResourceLoader.GetAllLevelConfigs();
    }
    public List<LevelConfig> AllLevels;
}