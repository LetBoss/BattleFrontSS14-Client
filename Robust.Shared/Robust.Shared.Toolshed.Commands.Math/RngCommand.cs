using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class RngCommand : ToolshedCommand
{
	[Dependency]
	private readonly IRobustRandom _random;

	[CommandImplementation("to")]
	public int To([PipedArgument] int from, int to)
	{
		return _random.Next(from, to);
	}

	[CommandImplementation("from")]
	public int From([PipedArgument] int to, int from)
	{
		return _random.Next(from, to);
	}

	[CommandImplementation("to")]
	public float To([PipedArgument] float from, float to)
	{
		return _random.NextFloat(from, to);
	}

	[CommandImplementation("from")]
	public float From([PipedArgument] float to, float from)
	{
		return _random.NextFloat(from, to);
	}

	[CommandImplementation("prob")]
	public bool Prob([PipedArgument] float prob)
	{
		return _random.Prob(prob);
	}
}
