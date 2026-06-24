using System;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand]
public sealed class VarCommand : ToolshedCommand
{
	private static Type[] _parsers = new Type[1] { typeof(VarTypeParser) };

	public override Type[] TypeParameterParsers => _parsers;

	[CommandImplementation(null)]
	public T Var<T>(IInvocationContext ctx, VarRef<T> var)
	{
		return var.Evaluate(ctx);
	}
}
