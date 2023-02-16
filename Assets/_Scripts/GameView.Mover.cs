using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEngine;

public partial class GameView
{

    private const float coinsSpeed = 7, obstaclesSpeed = 5;
    private float configSpeed => currentConfig != null ? currentConfig.fallSpeed : 10;
    private float speed = 1;
    public void MoveCursor(Vector3 touchPos)
    {
        if (Input.touchCount != 0)
        {
            particle.SetActive(true);
            particle.transform.Teleportation(touchPos.WithZ(2f));
        }
        else particle.SetActive(false);
#if UNITY_EDITOR
        particle.transform.Teleportation(touchPos.WithZ(2f));
#endif
    }
    public void MoveObstacles()
    {
        obstaclesInScene.ForEach(o => o.transform.Move(0, -obstaclesSpeed * Time.fixedDeltaTime, 0));
    }
    public void MoveMusicObjs()
    {
        for (int i = 0; i < musicObjectsInScene.Count; i++)
        {
            musicObjectsInScene[i].transform.Move(0, -speed * Time.fixedDeltaTime, 0);
            if (musicObjectsInScene[i].transform.position.y < downPoint.y)
            {
                Destroy(musicObjectsInScene[i].gameObject);
                musicObjectsInScene.RemoveAt(i);
                player.Damage(1);
                i--;
            }
        }
    }
    public void MoveCoins()
    {
        for (int i = 0; i < coinsInScene.Count; i++)
        {
            coinsInScene[i].transform.Move(0, -coinsSpeed * Time.fixedDeltaTime, 0);
            if (coinsInScene[i].transform.position.y < downPoint.y)
            {
                Destroy(coinsInScene[i].gameObject);
                coinsInScene.RemoveAt(i);
                i--;
            }
        }
    }
    public void CalculateSpeed(Vector3 touchPos)
    {
        speed = configSpeed * (GetOffsetInSeconds(touchPos) / startOffset);
    }
}
