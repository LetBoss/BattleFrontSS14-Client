namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand(Name = "f")]
public sealed class FloatCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public float Impl(float value)
	{
		return value;
	}
}
