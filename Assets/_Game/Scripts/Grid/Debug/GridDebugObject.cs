using UnityEngine;
using TMPro; // Ensure you have the TMP Essentials imported

public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMeshPro;
    private GridObject gridObject;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    private void Update()
    {
        // Performance Note: In a real build, use Events. 
        // For this Prototype, Update() is acceptable and "Snappy" to code.
        textMeshPro.text = gridObject.ToString();
    }
}