using System;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class NotValidCommandError : ConError
{
	public Type? TargetType;

	public override FormattedMessage DescribeInner()
	{
		FormattedMessage formattedMessage = new FormattedMessage();
		formattedMessage.AddText("Ran into an invalid command, could not parse.");
		if ((object)TargetType != null && TargetType != typeof(void))
		{
			formattedMessage.PushNewline();
			formattedMessage.AddText("The parser was trying to obtain a run of type " + TargetType.PrettyName() + ", make sure your run actually returns that value.");
		}
		return formattedMessage;
	}
}
