using System.Text;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.Errors;

public static class ConHelpers
{
	public static FormattedMessage HighlightSpan(string input, Vector2i span, Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage formattedMessage = FormattedMessage.FromUnformatted(input.Substring(0, span.X));
		formattedMessage.PushColor(color);
		string text;
		int x;
		if (span.Y >= input.Length)
		{
			text = input;
			x = span.X;
			formattedMessage.AddText(text.Substring(x, text.Length - x));
			formattedMessage.Pop();
			return formattedMessage;
		}
		x = span.X;
		formattedMessage.AddText(input.Substring(x, span.Y - x));
		formattedMessage.Pop();
		text = input;
		x = span.Y;
		formattedMessage.AddText(text.Substring(x, text.Length - x));
		return formattedMessage;
	}

	public static FormattedMessage ArrowSpan(Vector2i span)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(' ', span.X);
		stringBuilder.Append('^', span.Y - span.X);
		return FormattedMessage.FromUnformatted(stringBuilder.ToString());
	}
}
