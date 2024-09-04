using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : CustomSingleton<ScoreManager>
{
    public System.Action ComboScoreChangedEvent;

    [Header("References")]
    [SerializeField] int longestComboCount;
    [SerializeField] int currentComboCount;

    private void Start()
    {
        MatchController.instance.MatchFoundEvent += OnMatchFound;
    }

    private void OnMatchFound(bool isFound)
    {
        if (!isFound) ResetCurrentCombo();
        else
        {
            IncreaseCurrentComboCount(1);
            UpdateLongestComboCount();
        }

        ComboScoreChangedEvent?.Invoke();
    }
    private void IncreaseCurrentComboCount(int increaseAmount)
    {
        currentComboCount += increaseAmount;
    }
    private void UpdateLongestComboCount()
    {
        if (currentComboCount <= longestComboCount) return;
        longestComboCount = currentComboCount;
    }
    private void ResetCurrentCombo()
    {
        currentComboCount = 0;
    }
    public int GetLongestComboCount()
    {
        return longestComboCount;
    }
    public int GetCurrentComboCount()
    {
        return currentComboCount; 
    }
}
