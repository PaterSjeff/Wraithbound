using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    [SerializeField] private GameObject busyScreen;

    private void Start()
    {
        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
        UpdateBusyText(false);
    }

    private void UnitActionSystem_OnBusyChanged(bool isBusy)
    {
        UpdateBusyText(isBusy);
    }

    private void UpdateBusyText(bool isBusy)
    {
        busyScreen.SetActive(isBusy);
    }
}

