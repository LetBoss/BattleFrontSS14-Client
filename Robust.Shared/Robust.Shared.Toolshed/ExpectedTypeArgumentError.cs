using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public sealed class ExpectedTypeArgumentError : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Expected type argument, but ran out of input");
	}
}
