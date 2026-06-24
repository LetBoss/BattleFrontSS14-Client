using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

public sealed class ReadonlyVariableError(string name) : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("$" + name + " is a read-only variable.");
	}
}
