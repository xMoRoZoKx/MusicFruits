using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTools
{
    public static float MillisecondsToSeconds(this long milliseconds) => milliseconds / 1000;
    public static long SecondsToMilliseconds(this float seconds) => (long)(seconds * 1000);
}
