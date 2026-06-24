using System.Globalization;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers.Math;

public sealed class AngleTypeParser : TypeParser<Angle>
{
	public override bool TryParse(ParserContext ctx, out Angle result)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		result = default(Angle);
		string text = ctx.GetWord(ParserContext.IsNumeric)?.ToLowerInvariant();
		if (text == null)
		{
			if (!ctx.PeekRune().HasValue)
			{
				ctx.Error = new OutOfInputError();
				return false;
			}
			ctx.Error = new InvalidAngle(ctx.GetWord());
			return false;
		}
		if (text.EndsWith("deg"))
		{
			string text2 = text;
			if (!float.TryParse(text2.Substring(0, text2.Length - 3), CultureInfo.InvariantCulture, out var result2))
			{
				ctx.Error = new InvalidAngle(text);
				return false;
			}
			result = Angle.FromDegrees((double)result2);
			return true;
		}
		if (!float.TryParse(text, CultureInfo.InvariantCulture, out var result3))
		{
			ctx.Error = new InvalidAngle(text);
			result = default(Angle);
			return false;
		}
		result = new Angle((double)result3);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHint(GetArgHint(arg) + "\nAppend \"deg\" for degrees");
	}
}
