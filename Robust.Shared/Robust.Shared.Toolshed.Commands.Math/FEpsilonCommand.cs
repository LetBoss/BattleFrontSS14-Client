namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class FEpsilonCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public float Const()
	{
		return float.Epsilon;
	}
}
