using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveAction : BaseAction
{
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float hopHeight = 0.3f;
    [SerializeField] private float hopDuration = 0.3f;

    private List<Vector3> positionList;
    private int currentPositionIndex;
    private bool isMoving;

    public override string GetActionName() => "Move";

    public override ActionResourceType GetActionResourceType() => ActionResourceType.Move;

    public override bool CanExecute() => unit != null && unit.CanMove();

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositions = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition);
        
        currentPositionIndex = 0;
        positionList = new List<Vector3>();
        List<GridPosition> gridPath = new List<GridPosition>();

        if (pathGridPositions != null)
        {
            foreach (GridPosition pathGridPos in pathGridPositions)
            {
                positionList.Add(GridSystem.Instance.GetWorldPosition(pathGridPos));
                gridPath.Add(pathGridPos);
            }
        }
        else
        {
            positionList.Add(GridSystem.Instance.GetWorldPosition(gridPosition));
            gridPath.Add(gridPosition);
        }

        BaseActionStart(onActionComplete);
        StartCoroutine(MoveAlongPathCoroutine(gridPath));
    }

    private IEnumerator MoveAlongPathCoroutine(List<GridPosition> gridPath)
    {
        isMoving = true;
        
        for (int i = 0; i < positionList.Count; i++)
        {
            Vector3 targetPos = positionList[i];
            Vector3 startPos = transform.position;
            Vector3 direction = (targetPos - startPos).normalized;
            
            if (direction != Vector3.zero)
                transform.forward = direction;

            Sequence moveSeq = DOTween.Sequence();
            moveSeq.Append(transform.DOMoveY(startPos.y + hopHeight, hopDuration * 0.5f).SetEase(Ease.OutQuad));
            moveSeq.Join(transform.DOMove(new Vector3(targetPos.x, startPos.y + hopHeight, targetPos.z), hopDuration * 0.5f).SetEase(Ease.Linear));
            moveSeq.Append(transform.DOMoveY(targetPos.y, hopDuration * 0.5f).SetEase(Ease.InQuad));

            yield return moveSeq.WaitForCompletion();
            
            if (i < gridPath.Count)
                unit.SetGridPosition(gridPath[i]);
        }

        isMoving = false;
        BaseActionComplete();
    }


    public override bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> gridPositionList = GetValidActionGridPositionList();
        return gridPositionList.Contains(gridPosition);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        int moveRange = unit.GetMoveRange();

        // Check a larger area to ensure we catch all reachable positions
        int searchRange = moveRange * 2;
        for (int x = -searchRange; x <= searchRange; x++)
        {
            for (int z = -searchRange; z <= searchRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!GridSystem.Instance.IsValidGridPosition(testGridPosition)) continue;
                if (unitGridPosition == testGridPosition) continue;
                if (GridSystem.Instance.GetGridObject(testGridPosition).GetUnit() != null) continue;

                // Find path and check actual path length
                List<GridPosition> path = Pathfinding.Instance.FindPath(unitGridPosition, testGridPosition);
                if (path != null)
                {
                    // Path length (excluding starting position)
                    int pathLength = path.Count - 1;
                    if (pathLength <= moveRange)
                    {
                        validGridPositionList.Add(testGridPosition);
                    }
                }
            }
        }
        return validGridPositionList;
    }
}