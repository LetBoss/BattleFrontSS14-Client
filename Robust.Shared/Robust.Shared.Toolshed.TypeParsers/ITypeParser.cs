using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public interface ITypeParser
{
	Type Parses { get; }

	bool EnableValueRef { get; }

	bool ShowTypeArgSignature => true;

	bool TryParse(ParserContext ctx, [NotNullWhen(true)] out object? result);

	CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg);
}
