using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld instance;

    [SerializeField] private LayerMask mousePlaneLayerMask;

    private void Awake()
    {
        instance = this;
    }

    public static Vector3 GetPosition()
    {
        // FIX: Use InputManager instead of Input.mousePosition
        Vector2 mouseScreenPosition = InputManager.Instance.GetMouseScreenPosition();
        
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
}