using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DPManager : MonoBehaviour
{
    public static DPManager Instance { get; private set; }

    [Header("DP Settings")]
    public int startingDP = 20;
    public TMP_Text dpText;

    private int currentDP;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        currentDP = startingDP;
        UpdateDPText();
    }

    public bool CanSpendDP(int amount)
    {
        return currentDP >= amount;
    }

    public void SpendDP(int amount)
    {
        currentDP -= amount;
        UpdateDPText();
    }

    public void GainDP(int amount)
    {
        currentDP += amount;
        UpdateDPText();
    }

    private void UpdateDPText()
    {
        if (dpText != null)
            dpText.text = $"DP: {currentDP}";
    }

    public int GetCurrentDP()
    {
        return currentDP;
    }
}
