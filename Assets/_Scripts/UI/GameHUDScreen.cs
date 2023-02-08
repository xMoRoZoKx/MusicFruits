using System.Collections;
using System.Collections.Generic;
using Game.CodeTools;
using Game.UI;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class GameHUDScreen : WindowBase
{
    [SerializeField] private Image fieldImg;
    [SerializeField] private RectTransform heathPrefab;
    [SerializeField] private RectTransform container;
    private List<Transform> views = new List<Transform>();
    
    public void Show(Reactive<int> hp, Reactive<float> progress)
    {
        hp.SubscribeAndInvoke(value =>
        {
            views.ForEach(v => v.SetActive(false));
            for (int i = 0; i < value; i++)
            {
                if (views.Count <= i)
                    views.Add(Instantiate(heathPrefab, container));
                views[i].SetActive(true);
            }
        });
        progress.SubscribeAndInvoke(value =>
        {
            fieldImg.fillAmount = value;
        });
    }
}
