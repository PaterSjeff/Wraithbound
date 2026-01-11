using System;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private Animator unitAnimator; // Optional for now
    
    private Vector3 targetPosition;

    protected override void Awake() 
    {
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isActionInProgress) return;

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float stoppingDistance = .1f;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            
            // Snap rotation to look at target
            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }
        else
        {
            isActionInProgress = false;
            onActionComplete();
        }
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        this.targetPosition = GridSystem.Instance.GetWorldPosition(gridPosition);
        this.isActionInProgress = true;
    }

    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        if (gridPosition == unit.GetGridPosition())
        {
            // Cannot move to the same position
            return false;
        }

        if (GridSystem.Instance.GetGridObject(gridPosition).GetUnit() != null)
        {
            // Cannot move to an occupied position
            return false;
        }

        int distance = gridPosition.Distance(unit.GetGridPosition());
        return distance <= maxMoveDistance;
    }
}