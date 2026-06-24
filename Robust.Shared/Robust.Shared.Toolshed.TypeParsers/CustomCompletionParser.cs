using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public abstract class CustomCompletionParser<T> : CustomTypeParser<T> where T : notnull
{
	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result)
	{
		return Toolshed.TryParse<T>(ctx, out result);
	}
}
