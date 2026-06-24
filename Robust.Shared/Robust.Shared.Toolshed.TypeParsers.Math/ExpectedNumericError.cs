using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers.Math;

public sealed class ExpectedNumericError : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Expected a number");
	}
}
