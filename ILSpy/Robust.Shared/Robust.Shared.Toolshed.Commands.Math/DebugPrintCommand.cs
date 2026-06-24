using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand(Name = "??")]
public sealed class DebugPrintCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T DebugPrint<T>(IInvocationContext ctx, [PipedArgument] T value)
	{
		ctx.WriteLine(Toolshed.PrettyPrintType(value, out IEnumerable _));
		return value;
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<T> DebugPrint<T>(IInvocationContext ctx, [PipedArgument] IEnumerable<T> value)
	{
		List<T> list = value.ToList();
		ctx.WriteLine(Toolshed.PrettyPrintType(list, out IEnumerable _));
		return list;
	}
}
