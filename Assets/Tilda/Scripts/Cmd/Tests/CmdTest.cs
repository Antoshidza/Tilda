using NUnit.Framework;
using Tilda;

public class CmdTest
{
    [Test]
    public void InputHandleTest()
    {
        var cmd = new Cmd();
        var commandRan = false;
        cmd.AddCommand("test-command", input => commandRan = true);
        cmd.TryHandle("test-command");
        Assert.True(commandRan);
    }
}
