using System;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "?")]
public sealed class DefaultIfNullCommand : ToolshedCommand
{
	private static Type[] _parsers = new Type[1] { typeof(MapBlockOutputParser) };

	public override Type[] TypeParameterParsers => _parsers;

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public TOut? DefaultIfNull<TOut, TIn>(IInvocationContext ctx, [PipedArgument] TIn? value, Block<TIn, TOut> follower) where TIn : unmanaged
	{
		if (!value.HasValue)
		{
			return default(TOut);
		}
		return follower.Invoke(value.Value, ctx);
	}
}
