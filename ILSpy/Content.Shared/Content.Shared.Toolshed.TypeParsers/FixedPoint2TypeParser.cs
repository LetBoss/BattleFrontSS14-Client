using Content.Shared.FixedPoint;
using Robust.Shared.Console;
using Robust.Shared.Toolshed;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

namespace Content.Shared.Toolshed.TypeParsers;

public sealed class FixedPoint2TypeParser : TypeParser<FixedPoint2>
{
	public override bool TryParse(ParserContext ctx, out FixedPoint2 result)
	{
		int? value = default(int?);
		if (((BaseParser<FixedPoint2>)(object)this).Toolshed.TryParse<int?>(ctx, ref value))
		{
			result = FixedPoint2.New(value.Value);
			return true;
		}
		float? fValue = default(float?);
		if (((BaseParser<FixedPoint2>)(object)this).Toolshed.TryParse<float?>(ctx, ref fValue))
		{
			result = FixedPoint2.New(fValue.Value);
			return true;
		}
		result = FixedPoint2.Zero;
		return false;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHint(((BaseParser<FixedPoint2>)(object)this).GetArgHint(arg));
	}
}
