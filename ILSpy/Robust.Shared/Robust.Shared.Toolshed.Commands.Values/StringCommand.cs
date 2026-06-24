namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand(Name = "s")]
public sealed class StringCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public string Impl(string value)
	{
		return value;
	}
}
