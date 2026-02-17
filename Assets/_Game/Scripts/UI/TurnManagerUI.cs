using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManagerUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI initiativeOrderText;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnManager.Instance.RequestEndTurn();
        });

        TurnManager.Instance.OnTurnChanged += UpdateDisplay;
        TurnManager.Instance.OnActiveUnitChanged += OnActiveUnitChanged;
        TurnManager.Instance.OnRoundStarted += OnRoundStarted;
        UpdateDisplay();
    }

    private void OnActiveUnitChanged(Unit unit)
    {
        UpdateDisplay();
    }

    private void OnRoundStarted()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        TurnManager tm = TurnManager.Instance;
        bool playerTurn = tm.IsPlayerTurn();
        endTurnButton.gameObject.SetActive(playerTurn);
        endTurnButton.interactable = playerTurn;
        turnText.text = playerTurn
            ? $"Round {tm.GetRoundNumber()} — Your Turn"
            : $"Round {tm.GetRoundNumber()} — Enemy Turn";

        if (initiativeOrderText != null)
        {
            var order = tm.GetInitiativeOrder();
            var names = new System.Collections.Generic.List<string>();
            foreach (Unit u in order)
            {
                if (u != null && u.gameObject.activeInHierarchy)
                    names.Add(u.UnitData != null ? u.UnitData.unitName : u.gameObject.name);
            }
            initiativeOrderText.text = "Order: " + (names.Count > 0 ? string.Join(", ", names) : "—");
        }
    }
}
