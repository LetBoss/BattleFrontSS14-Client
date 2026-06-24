using System;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public sealed class TypeArgumentParseError(Type parser) : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Failed to parse type argument using parser " + parser.PrettyName());
	}
}
