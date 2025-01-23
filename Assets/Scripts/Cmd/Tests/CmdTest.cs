using NUnit.Framework;

public class CmdTest
{
    [Test]
    public void InputHandleTest()
    {
        var cmd = new Cmd.Cmd();
        var commandRan = false;
        cmd.AddCommand("test-command", input => commandRan = true);
        cmd.TryHandle("test-command");
        Assert.True(commandRan);
    }
}
