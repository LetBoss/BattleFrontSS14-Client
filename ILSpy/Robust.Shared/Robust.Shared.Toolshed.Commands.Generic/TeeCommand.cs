using System;
using System.Collections.Generic;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class TeeCommand : ToolshedCommand
{
	private static Type[] _parsers = new Type[1] { typeof(MapBlockOutputParser) };

	public override Type[] TypeParameterParsers => _parsers;

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<TIn> Tee<TOut, TIn>(IInvocationContext ctx, [PipedArgument] IEnumerable<TIn> value, Block<TIn, TOut> block)
	{
		foreach (TIn item in value)
		{
			block.Invoke(item, ctx);
			if (!ctx.HasErrors)
			{
				yield return item;
				continue;
			}
			yield break;
		}
	}
}
