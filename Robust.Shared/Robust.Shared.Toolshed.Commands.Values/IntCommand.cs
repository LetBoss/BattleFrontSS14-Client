namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand(Name = "i")]
public sealed class IntCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public int Impl(int value)
	{
		return value;
	}
}
