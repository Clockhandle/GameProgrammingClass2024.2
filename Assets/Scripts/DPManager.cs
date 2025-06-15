using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DPManager : MonoBehaviour
{
    public static DPManager Instance { get; private set; }

    [Header("DP Settings")]
    public int startingDP = 20;
    public TMP_Text dpText;

    public float dpGainInterval = 1f; 
    public int dpGainAmount = 1;

    private int currentDP;
    public int maxDP = 50;

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

    private float dpTimer = 0f;

    void Update()
    {
        dpTimer += Time.deltaTime;
        if (dpTimer >= dpGainInterval)
        {
            dpTimer = 0f;
            GainDP(dpGainAmount);
        }
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
        currentDP = Mathf.Min(currentDP + amount, maxDP);
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
