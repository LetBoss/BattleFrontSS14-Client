using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class SearchCommand : ToolshedCommand
{
	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	public IEnumerable<FormattedMessage> Search<T>([PipedArgument] IEnumerable<T> input, string term)
	{
		return (from x in input.Select((T x) => Toolshed.PrettyPrintType(x, out IEnumerable _)).ToList()
			where x.Contains(term, StringComparison.InvariantCultureIgnoreCase)
			select x).Select(delegate(string x)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			int num = x.IndexOf(term, StringComparison.InvariantCultureIgnoreCase);
			return ConHelpers.HighlightSpan(x, Vector2i.op_Implicit((num, num + term.Length)), Color.Aqua);
		});
	}
}
