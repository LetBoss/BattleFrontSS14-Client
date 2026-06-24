namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand(Name = "b")]
public sealed class BoolCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public bool Impl(bool value)
	{
		return value;
	}
}
