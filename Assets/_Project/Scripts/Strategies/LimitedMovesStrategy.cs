using UnityEngine;

public class LimitedMovesStrategy : ILevelStrategy
{
    private readonly int maxMoves;
    private int currentMoves;

    public LimitedMovesStrategy(int maxMoves)
    {
        this.maxMoves = Mathf.Max(1, maxMoves);
        currentMoves = 0;
    }

    public bool CanMove(int x, int z)
    {
        return currentMoves < maxMoves;
    }

    public void RegisterMove()
    {
        currentMoves++;
    }

    public void UndoMove()
    {
        currentMoves = Mathf.Max(0, currentMoves - 1);
    }

    public void OnLevelCompleted()
    {
        Debug.Log("Level completed in " + currentMoves + " moves!");
    }

    public int RemainingMoves => Mathf.Max(0, maxMoves - currentMoves);
}