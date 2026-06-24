using System.Collections.Generic;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class WhereCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> Where<T>(IInvocationContext ctx, [PipedArgument] IEnumerable<T> input, Block<T, bool> check)
	{
		foreach (T item in input)
		{
			bool flag = check.Invoke(item, ctx);
			if (!ctx.HasErrors)
			{
				if (flag)
				{
					yield return item;
				}
				continue;
			}
			yield break;
		}
	}
}
