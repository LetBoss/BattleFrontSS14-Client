using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public abstract class BaseParser<T> : ITypeParser, IPostInjectInit where T : notnull
{
	[Dependency]
	protected readonly ILocalizationManager Loc;

	[Dependency]
	private readonly ILogManager _log;

	[Dependency]
	protected readonly ToolshedManager Toolshed;

	protected ISawmill Log;

	public virtual bool EnableValueRef => true;

	public virtual bool ShowTypeArgSignature => true;

	public Type Parses => typeof(T);

	public virtual void PostInject()
	{
		Log = _log.GetSawmill(GetType().PrettyName());
	}

	public abstract bool TryParse(ParserContext ctx, [NotNullWhen(true)] out T? result);

	public abstract CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg);

	protected string GetArgHint(CommandArgument? arg)
	{
		return ToolshedCommand.GetArgHint(arg, typeof(T));
	}

	bool ITypeParser.TryParse(ParserContext ctx, [NotNullWhen(true)] out object? result)
	{
		if (!TryParse(ctx, out T result2))
		{
			result = null;
			return false;
		}
		result = result2;
		return true;
	}
}
