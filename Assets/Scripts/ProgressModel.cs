// Assets/Scripts/ProgressModel.cs
using UnityEngine;

[System.Serializable]
public class ProgressModel
{
    [Header("Коэффициенты прогресса")]
    public float a = 0.02705f;
    public float b = 0.01705f;
    public float bonusAd = 10f;
    public float bonusDon = 40.333333f;
    public float g = 0.10f;
}


