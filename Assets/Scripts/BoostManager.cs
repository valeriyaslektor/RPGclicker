// Assets/Scripts/BoostManager.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostManager : MonoBehaviour
{
    [Header("Ссылка на GameManager")] 
    public GameManager gameManager;

    [Header("UI буст-панели")] 
    public GameObject boostPanel;
    public GameObject boostButtonPrefab;
    public Transform buttonContainer;
    public Button regenerateButton;
    public Text boostsAvailableText;

    [Header("Набор возможных буст-опций")] 
    public BoostOption[] allOptions;

    private List<BoostOption> currentOptions = new List<BoostOption>();

    void Awake()
    {
        boostPanel.SetActive(false);
        if (regenerateButton != null)
            regenerateButton.onClick.AddListener(GenerateBoostOptions);
    }

    void Update()
    {
        boostsAvailableText.text = $"Available: {(gameManager.boostUsedThisLevel ? 0 : 1)}";
    }

    public void OpenBoostMenu()
    {
        if (gameManager.boostUsedThisLevel) return;
        GenerateBoostOptions();
        boostPanel.SetActive(true);
        boostPanel.transform.SetAsLastSibling();
    }

    public void CloseBoostMenu()
    {
        boostPanel.SetActive(false);
    }

    public void GenerateBoostOptions()
    {
        currentOptions.Clear();
        var pool = new List<BoostOption>(allOptions);

        // Выбираем случайные 3 опции
        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            currentOptions.Add(pool[idx]);
            pool.RemoveAt(idx);
        }

        // Удаляем старые кнопки
        foreach (Transform t in buttonContainer)
            Destroy(t.gameObject);

        // Создаём кнопки для новых опций
        foreach (var opt in currentOptions)
        {
            var btnObj = Instantiate(boostButtonPrefab, buttonContainer);
            var btn    = btnObj.GetComponent<Button>();
            var txt    = btnObj.GetComponentInChildren<Text>();
            txt.text   = opt.description;

            btn.onClick.AddListener(() =>
            {
                gameManager.ApplyBoostBonus(opt.bonusFraction, opt.description);
                CloseBoostMenu();
            });
        }
    }
}














