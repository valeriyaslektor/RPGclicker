// Assets/Scripts/ProfileSelector.cs
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ProfileData
{
    public string profileName;
    public int    minutesPerDay;
    public bool   watchAd;
    public bool   donate;
    public Button button;
}

public class ProfileSelector : MonoBehaviour
{
    [Header("GameManager reference")]
    public GameManager gameManager;

    [Header("Profile buttons setup")]
    public ProfileData[] profiles;

    void Start()
    {
        foreach (var p in profiles)
        {
            p.button.onClick.AddListener(() => OnSelectProfile(p));
        }
    }

    void OnSelectProfile(ProfileData p)
    {
        Debug.Log($"Simulating {p.profileName}");
        gameManager.SimulateProfile(p.minutesPerDay, p.watchAd, p.donate);
    }
}
