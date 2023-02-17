using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaffObject : MusicObject
{
    private PlayerInstance player;
    public void Init(PlayerInstance player)
    {
        this.player = player;
    }
    public override void Catch()
    {
        base.Catch();
    }

}
