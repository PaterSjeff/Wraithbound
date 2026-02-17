using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InitiativeEntryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private Image unitIcon;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject activeIndicator;
    
    [Header("Colors")]
    [SerializeField] private Color playerColor = new Color(0.3f, 0.6f, 1f, 0.8f);
    [SerializeField] private Color enemyColor = new Color(1f, 0.3f, 0.3f, 0.8f);
    [SerializeField] private Color activeColor = new Color(1f, 1f, 0.5f, 1f);
    [SerializeField] private float inactiveAlpha = 0.6f;

    private Unit unit;
    private bool isActive;

    public void Setup(Unit unit, bool isActive)
    {
        this.unit = unit;
        this.isActive = isActive;

        if (unit == null) return;

        // Set unit name
        if (unitNameText != null)
        {
            string displayName = unit.UnitData != null ? unit.UnitData.unitName : unit.name;
            unitNameText.text = displayName;
        }

        // Set background color based on team
        if (backgroundImage != null)
        {
            Color baseColor = unit.IsEnemy ? enemyColor : playerColor;
            
            if (isActive)
            {
                backgroundImage.color = activeColor;
            }
            else
            {
                Color fadedColor = baseColor;
                fadedColor.a = inactiveAlpha;
                backgroundImage.color = fadedColor;
            }
        }

        // Show active indicator
        if (activeIndicator != null)
        {
            activeIndicator.SetActive(isActive);
        }

        // Set icon (if you have unit portraits)
        if (unitIcon != null)
        {
            // You can set unit.UnitData.icon here if you add an icon field to UnitData_SO
            // For now, we'll just use a colored square
            unitIcon.color = unit.IsEnemy ? Color.red : Color.blue;
        }
    }
}
