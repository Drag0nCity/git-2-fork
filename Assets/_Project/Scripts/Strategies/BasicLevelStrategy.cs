using UnityEngine;

public class BasicLevelStrategy : ILevelStrategy
{
    public bool CanMove(int x, int z)
    {
        return true;
    }

    public void RegisterMove()
    {
    }

    public void UndoMove()
    {
    }

    public void OnLevelCompleted()
    {
        Debug.Log("Level completed!");
    }
}