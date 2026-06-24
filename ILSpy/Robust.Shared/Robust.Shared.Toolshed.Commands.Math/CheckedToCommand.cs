using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class CheckedToCommand : ToolshedCommand
{
	private static Type[] _parsers = new Type[1] { typeof(TypeTypeParser) };

	public override Type[] TypeParameterParsers => _parsers;

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public TOut Operation<TOut, T>([PipedArgument] T x) where TOut : INumberBase<TOut> where T : INumberBase<T>
	{
		return TOut.CreateChecked(x);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<TOut> Operation<TOut, T>([PipedArgument] IEnumerable<T> x) where TOut : INumberBase<TOut> where T : INumberBase<T>
	{
		return x.Select(Operation<TOut, T>);
	}
}
