using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Robust.Shared.Toolshed.Commands.Math;

[ToolshedCommand]
public sealed class AverageCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public T Average<T>([PipedArgument] IEnumerable<T> input) where T : INumberBase<T>
	{
		T[] array = input.ToArray();
		T zero = T.Zero;
		T[] array2 = array;
		foreach (T val in array2)
		{
			zero += val;
		}
		return zero / T.CreateChecked(array.Length);
	}
}
