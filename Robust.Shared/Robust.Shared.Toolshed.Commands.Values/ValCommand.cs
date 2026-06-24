using System;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand]
public sealed class ValCommand : ToolshedCommand
{
	private static Type[] _parsers = new Type[1] { typeof(TypeTypeParser) };

	public override Type[] TypeParameterParsers => _parsers;

	[CommandImplementation(null)]
	public T Val<T>(T value)
	{
		return value;
	}
}
