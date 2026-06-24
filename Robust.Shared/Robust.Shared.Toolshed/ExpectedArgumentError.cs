using System;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public sealed class ExpectedArgumentError(Type type) : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Expected command argument of type " + type.PrettyName() + ", but ran out of input");
	}
}
