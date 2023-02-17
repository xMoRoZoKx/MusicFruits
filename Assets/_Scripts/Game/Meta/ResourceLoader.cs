using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ResourceLoader
{
    public static Sprite GetDefaultIcon() => Resources.Load<Sprite>("Icons/Default");
    public static List<LevelConfig> GetAllLevelConfigs() => Resources.LoadAll<LevelConfig>("LevelConfigs/").ToList();
}
