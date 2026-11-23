public interface ILevelStrategy
{
    bool CanMove(int x, int z);
    void RegisterMove();
    void UndoMove();
    void OnLevelCompleted();
}