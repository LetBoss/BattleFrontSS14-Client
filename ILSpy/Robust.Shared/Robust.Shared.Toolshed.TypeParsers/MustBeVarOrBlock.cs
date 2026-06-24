using System;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class MustBeVarOrBlock(Type T) : ConError
{
	public override FormattedMessage DescribeInner()
	{
		return FormattedMessage.FromUnformatted("Command expects an argument of type " + T.PrettyName() + ".\nHowever this type has no parser available, and thus cannot be directly parsed.\nInstead, you have to use a variable or command block to provide it.");
	}
}
