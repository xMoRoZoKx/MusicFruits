using System.Collections;
using System.Collections.Generic;
using Game.CodeTools;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class StarsProgressBar : MonoBehaviour
{
    [SerializeField] private Image fieldImg;
    [SerializeField] private Image starPrefab;
    [SerializeField] private Color starOnColor, starOffColor;
    private List<Image> stars = new List<Image>();
    private float[] starPointsInPrecent = { 20, 50, 80 };
    public void Show(Reactive<float> progress)
    {
        var rectTransform = GetComponent<RectTransform>();
        foreach (var precent in starPointsInPrecent)
        {
            var star = Instantiate(starPrefab, transform);
            stars.Add(star);
            star.color = starOffColor;
            var startPos = rectTransform.position.WithX(rectTransform.position.x+ rectTransform.sizeDelta.x / 2);// 
            star.rectTransform.position = startPos.WithX(startPos.x - rectTransform.sizeDelta.x / 100 * precent);//+
        }
        progress.SubscribeAndInvoke(value =>
        {
            fieldImg.fillAmount = value;
            for (int i = 0; i < starPointsInPrecent.Length; i++)
            {
                if (value * 100 >= starPointsInPrecent[i] && stars[i].color != starOnColor)
                {
                    stars[i].color = starOnColor;
                }
            }
        });
    }
}
