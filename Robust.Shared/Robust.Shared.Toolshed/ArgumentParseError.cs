using System;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public sealed class ArgumentParseError(Type type, Type parser) : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Failed to parse command argument of type " + type.PrettyName() + " using parser " + parser.PrettyName());
	}
}
