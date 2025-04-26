// Assets/Scripts/GameManager.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Коэффициенты модели")]
    public float clickRate = 0.02705f;
    public float idleRate  = 0.01705f;

    [Header("Бонусы за рекламу/донат")]
    public float adBonusXP     = 10f;
    public float donateBonusXP = 40.333333f;

    [Header("Настройки ручных кликов")]
    public float clicksPerMinute = 60f;

    [Header("Состояние прогресса")]
    public float totalXP             = 0f;
    public int   currentDay         = 1;
    public float timeOfDay          = 0f;   // минуты с начала дня
    public float playedTodayMinutes = 0f;   // активные мин. сегодня
    public float totalPlayedMinutes = 0f;   // активные мин. за сессию

    // boost
    private float pendingBoostFraction = 0f;
    public bool  boostUsedThisLevel    = false;

    // ad/donate
    public bool adClaimed = false;
    public bool donated   = false;

    [Header("UI — привяжите в инспекторе")]
    public Text   dayText;
    public Text   timeText;
    public Text   xpText;
    public Text   levelText;
    public Slider progressBar;

    void Start()
    {
        UpdateUI();
    }

    // -------- ручные методы --------

    /// <summary>+1h: начисляет активный XP (+boost), а потом idle при переходе дня</summary>
    public void AddHour()
    {
        const float ONE_HOUR = 60f;
        playedTodayMinutes += ONE_HOUR;
        totalPlayedMinutes += ONE_HOUR;

        // базовый XP за час
        float xpThisHour = clickRate * ONE_HOUR;

        // применяем буст, если был выбран
        if (pendingBoostFraction > 0f)
        {
            float boostXP = clickRate * ONE_HOUR * pendingBoostFraction;
            xpThisHour   += boostXP;
            Debug.Log($"Boost applied ('+1h'): +{boostXP:F2} XP");
            pendingBoostFraction = 0f;
        }

        totalXP += xpThisHour;
        Debug.Log($"AddHour(): +{xpThisHour:F2} XP totalXP={totalXP:F2}");

        // вращаем время
        timeOfDay += ONE_HOUR;
        if (timeOfDay >= 1440f)
        {
            float idleMins = 1440f - playedTodayMinutes;
            if (idleMins > 0f)
            {
                float idleXP = idleRate * idleMins;
                totalXP    += idleXP;
                Debug.Log($"Idle end-of-day: +{idleXP:F2} XP");
            }

            currentDay++;
            timeOfDay           -= 1440f;
            playedTodayMinutes   = 0f;
            boostUsedThisLevel   = false;
        }

        UpdateUI();
    }

    /// <summary>+1d: начислить idle за остаток дня и перейти на новый день</summary>
    public void AdvanceDayOnly()
    {
        float idleMins = 1440f - playedTodayMinutes;
        if (idleMins > 0f)
        {
            float idleXP = idleRate * idleMins;
            totalXP    += idleXP;
            Debug.Log($"Idle AdvanceDay: +{idleXP:F2} XP");
        }

        currentDay++;
        timeOfDay           = 0f;
        playedTodayMinutes  = 0f;
        boostUsedThisLevel  = false;

        UpdateUI();
    }

    /// <summary>Реальный клик по врагу: кликает за долю минуты</summary>
    public void OnEnemyClick()
    {
        float clickMinutes = 1f / clicksPerMinute;
        playedTodayMinutes += clickMinutes;
        totalPlayedMinutes += clickMinutes;

        float xpThisClick = clickRate * clickMinutes;

        if (pendingBoostFraction > 0f)
        {
            float boostXP = clickRate * clickMinutes * pendingBoostFraction;
            xpThisClick += boostXP;
            Debug.Log($"Boost applied ('click'): +{boostXP:F3} XP");
            pendingBoostFraction = 0f;
        }

        totalXP += xpThisClick;
        Debug.Log($"OnEnemyClick(): +{xpThisClick:F3} XP totalXP={totalXP:F3}");

        UpdateUI();
    }

    /// <summary>Watch Ad (ручной)</summary>
    public void WatchAd()
    {
        if (!adClaimed)
        {
            adClaimed = true;
            totalXP  += adBonusXP;
            Debug.Log($"+Ad Bonus: +{adBonusXP:F2} XP");
            UpdateUI();
        }
    }

    /// <summary>Donate (ручной)</summary>
    public void Donate()
    {
        if (!donated)
        {
            donated  = true;
            totalXP  += donateBonusXP;
            Debug.Log($"+Donate Bonus: +{donateBonusXP:F2} XP");
            UpdateUI();
        }
    }

    /// <summary>Выбрать Boost: запоминаем и логируем выбор</summary>
    public void ApplyBoostBonus(float bonusFraction, string description)
    {
        if (boostUsedThisLevel) return;
        boostUsedThisLevel    = true;
        pendingBoostFraction  = bonusFraction;
        Debug.Log($"Boost selected: '{description}' ({bonusFraction:P})");
        UpdateUI();
    }

    /// <summary>Симуляция профиля 2 дня (без влияния ручного режима)</summary>
    public void SimulateProfile(int minutesPerDay, bool watchAd, bool donateFlag)
    {
        totalXP             = 0f;
        currentDay          = 1;
        timeOfDay           = 0f;
        pendingBoostFraction = 0f;
        adClaimed           = false;
        donated             = false;
        boostUsedThisLevel  = false;

        for (int day = 1; day <= 2; day++)
        {
            float playMins = minutesPerDay;
            totalXP += clickRate * playMins;
            totalXP += idleRate * (1440f - playMins);

            if (watchAd && !adClaimed)  { WatchAd();  }
            if (donateFlag && !donated) { Donate();   }

            currentDay++;
        }

        timeOfDay = 0f;
        UpdateUI();
    }

    /// <summary>Перезапустить сцену в рантайме</summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // -------- внутренняя отрисовка --------

    private void UpdateUI()
    {
        int lvl  = Mathf.FloorToInt(totalXP);
        float frac = totalXP - lvl;

        if (dayText)   dayText.text   = $"Day: {currentDay}";
        if (timeText)
        {
            int hh = Mathf.FloorToInt(timeOfDay / 60f);
            int mm = Mathf.FloorToInt(timeOfDay % 60f);
            timeText.text = $"{hh:00}:{mm:00}";
        }
        if (xpText)    xpText.text    = $"XP: {totalXP:F3}";
        if (levelText) levelText.text = $"Level: {lvl}";
        if (progressBar) progressBar.value = frac;
    }
}


























