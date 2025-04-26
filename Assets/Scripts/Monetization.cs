// Assets/Scripts/Monetization.cs
using UnityEngine;

public class Monetization : MonoBehaviour
{
    public GameManager gameManager;
    public void WatchAd() => gameManager.WatchAd();
    public void Donate()  => gameManager.Donate();
}








