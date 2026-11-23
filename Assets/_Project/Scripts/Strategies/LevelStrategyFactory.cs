public static class LevelStrategyFactory
{
    public static ILevelStrategy CreateBasicStrategy()
    {
        return new BasicLevelStrategy();
    }

    public static ILevelStrategy CreateLimitedMovesStrategy(int maxMoves)
    {
        return new LimitedMovesStrategy(maxMoves);
    }
}