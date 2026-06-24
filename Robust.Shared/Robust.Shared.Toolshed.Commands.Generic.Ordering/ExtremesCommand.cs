using System.Collections.Generic;
using System.Linq;

namespace Robust.Shared.Toolshed.Commands.Generic.Ordering;

[ToolshedCommand]
public sealed class ExtremesCommand : ToolshedCommand
{
	[TakesPipedTypeAsGeneric]
	[CommandImplementation(null)]
	public IEnumerable<T> Extremes<T>([PipedArgument] IEnumerable<T> input)
	{
		IList<T> collection = input as IList<T>;
		if (collection == null)
		{
			collection = input.ToArray();
		}
		int len = collection.Count;
		for (int i = 0; i < len / 2; i++)
		{
			yield return collection[i];
			IList<T> list = collection;
			yield return list[list.Count - i];
		}
		if (collection.Count % 2 != 0)
		{
			yield return collection[collection.Count / 2];
		}
	}
}
