using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManagerUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnText;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnManager.Instance.NextTurn();
        });

        TurnManager.Instance.OnTurnChanged += UpdateTurnText;
        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        turnText.text = TurnManager.Instance.IsPlayerTurn() ? "Player Turn" : "Enemy Turn";
    }
}
