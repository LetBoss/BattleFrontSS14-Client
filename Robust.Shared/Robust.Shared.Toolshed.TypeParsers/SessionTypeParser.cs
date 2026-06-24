using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed.TypeParsers;

internal sealed class SessionTypeParser : TypeParser<ICommonSession>
{
	public record InvalidUsername(ILocalizationManager Loc, string Username) : IConError
	{
		public string? Expression { get; set; }

		public Vector2i? IssueSpan { get; set; }

		public StackTrace? Trace { get; set; }

		public FormattedMessage DescribeInner()
		{
			return FormattedMessage.FromUnformatted(Loc.GetString("cmd-parse-failure-session", ("username", Username)));
		}
	}

	[Dependency]
	private ISharedPlayerManager _player;

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out ICommonSession? result)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		int index = ctx.Index;
		string word = ctx.GetWord();
		result = null;
		if (word == null)
		{
			ctx.Error = new OutOfInputError();
			return false;
		}
		if (_player.TryGetSessionByUsername(word, out ICommonSession session))
		{
			result = session;
			return true;
		}
		ctx.Error = new InvalidUsername(Loc, word);
		ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
		return false;
	}

	public override CompletionResult TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(sorted: true, _player), GetArgHint(arg));
	}
}
