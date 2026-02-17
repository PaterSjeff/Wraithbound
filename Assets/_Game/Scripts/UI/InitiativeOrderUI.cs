using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InitiativeOrderUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform entryContainer;
    [SerializeField] private GameObject unitEntryPrefab;
    [SerializeField] private GameObject roundSeparatorPrefab;
    
    [Header("Settings")]
    [SerializeField] private float entrySpacing = 10f;
    [SerializeField] private float entryHeight = 60f;
    [SerializeField] private float slideAnimationDuration = 0.3f;
    [SerializeField] private Ease slideEase = Ease.OutQuad;

    private List<GameObject> activeEntries = new List<GameObject>();
    private GameObject roundSeparator;

    private void Start()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnActiveUnitChanged += OnActiveUnitChanged;
            TurnManager.Instance.OnRoundStarted += OnRoundStarted;
        }
        
        RefreshDisplay();
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnActiveUnitChanged -= OnActiveUnitChanged;
            TurnManager.Instance.OnRoundStarted -= OnRoundStarted;
        }
    }

    private void OnActiveUnitChanged(Unit activeUnit)
    {
        RefreshDisplay();
    }

    private void OnRoundStarted()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        StartCoroutine(RefreshDisplayCoroutine());
    }

    private IEnumerator RefreshDisplayCoroutine()
    {
        if (TurnManager.Instance == null || entryContainer == null || unitEntryPrefab == null)
            yield break;

        var initiativeOrder = TurnManager.Instance.GetInitiativeOrder();
        Unit activeUnit = TurnManager.Instance.GetActiveUnit();

        if (initiativeOrder == null || initiativeOrder.Count == 0)
            yield break;

        // Find the active unit's index in the initiative order
        int activeIndex = -1;
        for (int i = 0; i < initiativeOrder.Count; i++)
        {
            if (initiativeOrder[i] == activeUnit)
            {
                activeIndex = i;
                break;
            }
        }

        // Reorder so active unit is at the top
        List<Unit> reorderedList = new List<Unit>();
        if (activeIndex >= 0)
        {
            // Add from active unit to end
            for (int i = activeIndex; i < initiativeOrder.Count; i++)
            {
                reorderedList.Add(initiativeOrder[i]);
            }

            int roundBreakIndex = reorderedList.Count;

            // Add from start to active unit (next round)
            for (int i = 0; i < activeIndex; i++)
            {
                reorderedList.Add(initiativeOrder[i]);
            }

            // If we already have entries, slide them up/down
            if (activeEntries.Count > 0)
            {
                // Slide all entries up by one position height
                float slideDistance = -(entryHeight + entrySpacing);
                
                foreach (GameObject entry in activeEntries)
                {
                    if (entry != null)
                    {
                        RectTransform rt = entry.GetComponent<RectTransform>();
                        if (rt != null)
                        {
                            rt.DOAnchorPosY(rt.anchoredPosition.y - slideDistance, slideAnimationDuration)
                                .SetEase(slideEase);
                        }
                    }
                }
                
                yield return new WaitForSeconds(slideAnimationDuration);
                
                // Clear old entries after animation
                foreach (GameObject entry in activeEntries)
                {
                    if (entry != null)
                        Destroy(entry);
                }
                activeEntries.Clear();
                
                if (roundSeparator != null)
                {
                    Destroy(roundSeparator);
                    roundSeparator = null;
                }
            }

            // Create UI entries
            for (int i = 0; i < reorderedList.Count; i++)
            {
                Unit unit = reorderedList[i];
                if (unit == null) continue;

                // Insert round separator before next round units
                if (i == roundBreakIndex && roundSeparatorPrefab != null && activeIndex > 0)
                {
                    roundSeparator = Instantiate(roundSeparatorPrefab, entryContainer);
                    activeEntries.Add(roundSeparator);
                }

                GameObject entry = Instantiate(unitEntryPrefab, entryContainer);
                activeEntries.Add(entry);

                // Configure the entry
                InitiativeEntryUI entryUI = entry.GetComponent<InitiativeEntryUI>();
                if (entryUI != null)
                {
                    bool isActive = (unit == activeUnit);
                    entryUI.Setup(unit, isActive);
                }
            }

            // Force layout rebuild
            if (entryContainer is RectTransform rectTransform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }
    }
}
