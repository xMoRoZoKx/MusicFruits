using System.Collections;
using System.Collections.Generic;
using Game.CodeTools;
using Tools;
using UITools;
using UnityEngine;
using UnityEngine.UI;

public class StarsProgressBar : MonoBehaviour
{
    [SerializeField] private Image fieldImg;
    [SerializeField] private Image starPrefab;
    [SerializeField] private Color starOnColor, starOffColor;
    [SerializeField] private float[] starPointsInPercent = { 30, 70, 100 };
    private List<Image> stars = new List<Image>();
    public void Show(Reactive<float> progress, Canvas canvas)
    {
        var starsContainer = GetComponent<RectTransform>();
        foreach (var precent in starPointsInPercent)
        {
            var star = Instantiate(starPrefab, transform);
            stars.Add(star);
            star.color = starOffColor;
            float rectWidth = starsContainer.GetWidth(canvas.scaleFactor);
            var startPos = starsContainer.position.WithX(starsContainer.position.x - (rectWidth / 2));
            star.rectTransform.position = startPos.WithX(startPos.x + (rectWidth/ 100 * precent));
        }
        progress.SubscribeAndInvoke(value =>
        {
            fieldImg.fillAmount = value;
            for (int i = 0; i < starPointsInPercent.Length; i++)
            {
                if (value * 100 >= starPointsInPercent[i] && stars[i].color != starOnColor)
                {
                    stars[i].color = starOnColor;
                }
            }
        });
    }
}
