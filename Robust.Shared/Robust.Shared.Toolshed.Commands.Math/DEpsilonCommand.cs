namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class DEpsilonCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	public double Const()
	{
		return double.Epsilon;
	}
}
