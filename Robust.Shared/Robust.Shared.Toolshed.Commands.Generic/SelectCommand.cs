using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class SelectCommand : ToolshedCommand
{
	[Dependency]
	private readonly IRobustRandom _random;

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<TR> Select<TR>([PipedArgument] IEnumerable<TR> enumerable, Quantity quantity, [CommandInverted] bool inverted)
	{
		TR[] array = enumerable.ToArray();
		_random.Shuffle(array);
		float? amount = quantity.Amount;
		if (amount.HasValue)
		{
			float valueOrDefault = amount.GetValueOrDefault();
			int num = (int)System.Math.Ceiling(valueOrDefault);
			if (inverted)
			{
				num = System.Math.Max(0, array.Length - num);
			}
			return array.Take(num);
		}
		int count = (inverted ? ((int)System.Math.Floor((double)array.Length * System.Math.Clamp(1.0 - (double)quantity.Percentage.Value, 0.0, 1.0))) : ((int)System.Math.Floor((double)array.Length * System.Math.Clamp(quantity.Percentage.Value, 0.0, 1.0))));
		return array.Take(count);
	}
}
