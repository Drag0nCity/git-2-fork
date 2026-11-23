using System;
using UnityEngine;
public static class GameEvents
{
    public static event Action<int> OnMoveCountChanged;
    public static event Action OnLevelCompleted;
    public static event Action OnLevelFailed;

    public static event Action<Vector3, Vector3> OnPlayerMoved;

    public static void RaiseMoveCountChanged(int moveCount)
    {
        OnMoveCountChanged?.Invoke(moveCount);
    }

    public static void RaiseLevelCompleted()
    {
        OnLevelCompleted?.Invoke();
    }

    public static void RaiseLevelFailed()
    {
        OnLevelFailed?.Invoke();
    }

    public static void RaisePlayerMoved(
        Vector3 previousPosition,
        Vector3 currentPosition)
    {
        OnPlayerMoved?.Invoke(previousPosition, currentPosition);
    }
}
