using System;
using System.Collections.Generic;

namespace Robust.Shared.Toolshed.Commands.Generic.Stats;

[ToolshedCommand]
public sealed class BinCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IDictionary<T, int> Bin<T>([PipedArgument] IEnumerable<T> input) where T : IComparable<T>
	{
		Dictionary<T, int> dictionary = new Dictionary<T, int>();
		foreach (T item in input)
		{
			dictionary.TryAdd(item, 0);
			dictionary[item]++;
		}
		return dictionary;
	}
}
