
using UnityEngine;

public class LevelStrategyManager : MonoBehaviour
{
    private ILevelStrategy currentStrategy;
    private bool isConfigured;

    public ILevelStrategy CurrentStrategy => currentStrategy;

    public bool HasRunOutOfMoves =>
        currentStrategy is LimitedMovesStrategy limited &&
        limited.RemainingMoves <= 0;

    public void ConfigureFromLevel(LevelData levelData)
    {
        if (levelData == null)
        {
            Debug.LogError("LevelStrategyManager: LevelData is null!");
            return;
        }

        if (levelData.useMoveLimit)
        {
            currentStrategy = LevelStrategyFactory.CreateLimitedMovesStrategy(
                levelData.maxMoves
            );
        }
        else
        {
            currentStrategy = LevelStrategyFactory.CreateBasicStrategy();
        }

        isConfigured = true;

        Debug.Log("Strategy configured for: " + levelData.levelName);
    }

    public bool CanMove(int x, int z)
    {
        return isConfigured &&
               currentStrategy != null &&
               currentStrategy.CanMove(x, z);
    }

    public void RegisterMove()
    {
        currentStrategy?.RegisterMove();
    }

    public void UndoMove()
    {
        currentStrategy?.UndoMove();
    }

    public void CompleteLevel()
    {
        currentStrategy?.OnLevelCompleted();
    }
}
