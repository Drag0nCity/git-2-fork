using UnityEngine;
public class MoveCommand : ICommand
{
    private readonly Transform player;
    private readonly Vector3 playerOldPosition;
    private readonly Vector3 playerNewPosition;

    private readonly Transform block;
    private readonly Vector3 blockOldPosition;
    private readonly Vector3 blockNewPosition;

    public MoveCommand(
        Transform player,
        Vector3 playerNewPosition,
        Transform block = null,
        Vector3 blockNewPosition = default)
    {
        this.player = player;
        this.playerOldPosition = player.position;
        this.playerNewPosition = playerNewPosition;

        this.block = block;

        if (block != null)
        {
            blockOldPosition = block.position;
            this.blockNewPosition = blockNewPosition;
        }
    }

    public void Execute()
    {
        if (block != null)
            block.position = blockNewPosition;

        player.position = playerNewPosition;
    }

    public void Undo()
    {
        player.position = playerOldPosition;

        if (block != null)
            block.position = blockOldPosition;
    }
}
