using System.Linq;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class CommandSpecTypeParser : TypeParser<CommandSpec>
{
	public override bool TryParse(ParserContext ctx, out CommandSpec result)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		string word = ctx.GetWord(ParserContext.IsCommandToken);
		int index = ctx.Index;
		string subCommand = null;
		if (word == null)
		{
			if (!ctx.PeekRune().HasValue)
			{
				ctx.Error = new OutOfInputError();
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
				result = default(CommandSpec);
				return false;
			}
			ctx.Error = new NotValidCommandError();
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
			result = default(CommandSpec);
			return false;
		}
		if (!ctx.Environment.TryGetCommand(word, out ToolshedCommand command))
		{
			ctx.Error = new UnknownCommandError(word);
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
			result = default(CommandSpec);
			return false;
		}
		if (command.HasSubCommands)
		{
			if (!ctx.EatMatch(':'))
			{
				IConError error;
				if (!ctx.OutOfInput)
				{
					IConError conError = new ExpectedSubCommand();
					error = conError;
				}
				else
				{
					IConError conError = new OutOfInputError();
					error = conError;
				}
				ctx.Error = error;
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
				result = default(CommandSpec);
				return false;
			}
			int index2 = ctx.Index;
			string word2 = ctx.GetWord(ParserContext.IsToken);
			if (word2 == null)
			{
				ctx.Error = new ExpectedSubCommand();
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
				result = default(CommandSpec);
				return false;
			}
			if (!command.Subcommands.Contains(word2))
			{
				ctx.Error = new UnknownSubcommandError(word2, command);
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index2, ctx.Index)));
				result = default(CommandSpec);
				return false;
			}
			subCommand = word2;
		}
		result = new CommandSpec(command, subCommand);
		return true;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		return CompletionResult.FromHintOptions(from x in parserContext.Environment.AllCommands()
			select x.AsCompletion(), "<command name>");
	}
}
