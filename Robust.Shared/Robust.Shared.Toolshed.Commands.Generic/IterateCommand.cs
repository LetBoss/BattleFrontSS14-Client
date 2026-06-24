using System.Collections.Generic;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class IterateCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T>? Iterate<T>(IInvocationContext ctx, [PipedArgument] T value, Block<T, T> block, int times)
	{
		for (int i = 0; i < times; i++)
		{
			T val = block.Invoke(value, ctx);
			if (val != null && !ctx.HasErrors)
			{
				value = val;
				yield return value;
				continue;
			}
			break;
		}
	}
}
