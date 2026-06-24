using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Shared.Player;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class IsImaginaryCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public bool Operation<T>([PipedArgument] T x) where T : INumberBase<T>
	{
		return T.IsImaginaryNumber(x);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<bool> Operation<T>([PipedArgument] IEnumerable<T> x) where T : INumberBase<T>
	{
		return x.Select(Operation);
	}

	[CommandImplementation(null)]
	public bool Operation(IInvocationContext ctx, [PipedArgument] ICommonSession x)
	{
		return ctx.Session != x;
	}
}
