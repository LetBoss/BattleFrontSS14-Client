using System.Diagnostics;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Errors;

public interface IConError
{
	string? Expression { get; protected set; }

	Vector2i? IssueSpan { get; protected set; }

	StackTrace? Trace { get; protected set; }

	FormattedMessage Describe()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage formattedMessage = new FormattedMessage();
		string expression = Expression;
		if (expression != null)
		{
			Vector2i? issueSpan = IssueSpan;
			if (issueSpan.HasValue)
			{
				Vector2i valueOrDefault = issueSpan.GetValueOrDefault();
				formattedMessage.AddMessage(ConHelpers.HighlightSpan(expression, valueOrDefault, Color.Red));
				formattedMessage.PushNewline();
				formattedMessage.AddMessage(ConHelpers.ArrowSpan(valueOrDefault));
				formattedMessage.PushNewline();
			}
		}
		formattedMessage.AddMessage(DescribeInner());
		return formattedMessage;
	}

	protected FormattedMessage DescribeInner();

	void Contextualize(string expression, Vector2i issueSpan)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (Expression == null || !IssueSpan.HasValue)
		{
			Expression = expression;
			IssueSpan = issueSpan;
		}
	}
}
