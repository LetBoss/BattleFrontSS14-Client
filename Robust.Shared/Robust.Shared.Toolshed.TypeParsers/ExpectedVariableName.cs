using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class ExpectedVariableName : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Expected a valid variable name.");
	}
}
