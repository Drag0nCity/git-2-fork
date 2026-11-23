using System.Collections.Generic;

public class CommandHistory
{
    private readonly Stack<ICommand> commands = new Stack<ICommand>();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        commands.Push(command);
    }

    public void UndoLastCommand()
    {
        if (commands.Count == 0)
            return;

        ICommand command = commands.Pop();
        command.Undo();
    }

    public void Clear()
    {
        commands.Clear();
    }

    public int Count => commands.Count;
}
