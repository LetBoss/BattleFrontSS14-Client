using System;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Console;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class ParsedCommand
{
	internal readonly ToolshedCommandImplementor Implementor;

	internal readonly ConcreteCommandMethod Method;

	private bool _passedInvokeTest;

	public ToolshedCommand Command => Implementor.Owner;

	public Type ReturnType => Method.Info.ReturnType;

	public Type? PipedType => Bundle.PipedType;

	public string? SubCommand => Bundle.SubCommand;

	internal Func<CommandInvocationArguments, object?> Invocable { get; }

	internal CommandArgumentBundle Bundle { get; }

	public static bool TryParse(ParserContext ctx, Type? piped, [NotNullWhen(true)] out ParsedCommand? result)
	{
		ParserRestorePoint point = ctx.Save();
		CommandArgumentBundle bundle = ctx.Bundle;
		ctx.Bundle = new CommandArgumentBundle
		{
			Inverted = false,
			PipedType = piped
		};
		ctx.ConsumeWhitespace();
		if (!TryDigestModifiers(ctx))
		{
			result = null;
			ctx.Restore(point);
			return false;
		}
		if (!TryParseCommand(ctx, out Func<CommandInvocationArguments, object> invocable, out ConcreteCommandMethod? method, out ToolshedCommandImplementor implementor))
		{
			result = null;
			ctx.Restore(point);
			return false;
		}
		result = new ParsedCommand(ctx.Bundle, invocable, method.Value, implementor);
		ctx.Bundle = bundle;
		return true;
	}

	private ParsedCommand(CommandArgumentBundle bundle, Func<CommandInvocationArguments, object?> invocable, ConcreteCommandMethod method, ToolshedCommandImplementor implementor)
	{
		Invocable = invocable;
		Bundle = bundle;
		Implementor = implementor;
		Method = method;
	}

	private static bool TryDigestModifiers(ParserContext ctx)
	{
		if (ctx.EatMatch("not"))
		{
			ctx.ConsumeWhitespace();
			ctx.Bundle.Inverted = true;
		}
		return true;
	}

	private static bool TryParseCommand(ParserContext ctx, [NotNullWhen(true)] out Func<CommandInvocationArguments, object?>? invocable, [NotNullWhen(true)] out ConcreteCommandMethod? method, [NotNullWhen(true)] out ToolshedCommandImplementor? implementor)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		invocable = null;
		implementor = null;
		method = null;
		int index = ctx.Index;
		if (!TryParseCommandName(ctx, out string name))
		{
			return false;
		}
		if (!ctx.Environment.TryGetCommand(name, out ToolshedCommand command))
		{
			if (ctx.GenerateCompletions)
			{
				if (ctx.OutOfInput)
				{
					ctx.Completions = ctx.Environment.CommandCompletionsForType(ctx.Bundle.PipedType);
				}
				return false;
			}
			if (ctx.Error == null)
			{
				ctx.Error = new UnknownCommandError(name);
			}
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
			return false;
		}
		if (!TryParseImplementor(ctx, command, out implementor))
		{
			return false;
		}
		if (!ctx.CheckInvokable(implementor.Spec))
		{
			if (ctx.GenerateCompletions)
			{
				ctx.Completions = CompletionResult.FromHint("Insufficient permissions for command: " + implementor.FullName);
			}
			return false;
		}
		if (ctx.GenerateCompletions && ctx.OutOfInput)
		{
			ctx.Completions = ((ctx.Bundle.SubCommand == null) ? ctx.Environment.CommandCompletionsForType(ctx.Bundle.PipedType) : ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, command));
			return false;
		}
		return implementor.TryParse(ctx, out invocable, out method);
	}

	private static bool TryParseCommandName(ParserContext ctx, [NotNullWhen(true)] out string? name)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		ctx.Bundle.NameStart = ctx.Index;
		name = ctx.GetWord(ParserContext.IsCommandToken);
		if (name != null)
		{
			ctx.Bundle.Command = name;
			ctx.Bundle.NameEnd = ctx.Index;
			return true;
		}
		if (ctx.OutOfInput)
		{
			if (ctx.GenerateCompletions)
			{
				ctx.Completions = ctx.Environment.CommandCompletionsForType(ctx.Bundle.PipedType);
			}
			else
			{
				ctx.Error = new OutOfInputError();
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
			}
			return false;
		}
		if (ctx.GenerateCompletions)
		{
			return false;
		}
		ctx.Error = new NotValidCommandError();
		ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Bundle.NameStart, ctx.Index + 1)));
		return false;
	}

	private static bool TryParseImplementor(ParserContext ctx, ToolshedCommand cmd, [NotNullWhen(true)] out ToolshedCommandImplementor? impl)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (!cmd.HasSubCommands)
		{
			impl = cmd.CommandImplementors[string.Empty];
			return true;
		}
		impl = null;
		if (!ctx.EatMatch(':'))
		{
			if (ctx.GenerateCompletions)
			{
				ctx.Completions = ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, cmd);
				return false;
			}
			ctx.Error = new OutOfInputError();
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
			return false;
		}
		int index = ctx.Index;
		string word = ctx.GetWord(ParserContext.IsToken);
		if (word == null)
		{
			if (ctx.GenerateCompletions)
			{
				ctx.Completions = ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, cmd);
				return false;
			}
			ctx.Error = new OutOfInputError();
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index)));
			return false;
		}
		if (!cmd.CommandImplementors.TryGetValue(word, out impl))
		{
			if (ctx.GenerateCompletions)
			{
				ctx.Completions = ctx.Environment.SubCommandCompletionsForType(ctx.Bundle.PipedType, cmd);
				return false;
			}
			ctx.Error = new UnknownSubcommandError(word, cmd);
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
			return false;
		}
		ctx.Bundle.NameEnd = ctx.Index;
		ctx.Bundle.SubCommand = word;
		return true;
	}

	public object? Invoke(object? pipedIn, IInvocationContext ctx)
	{
		if (!_passedInvokeTest && !ctx.CheckInvokable(new CommandSpec(Command, SubCommand), out IConError error))
		{
			if (error != null)
			{
				ctx.ReportError(error);
			}
			return null;
		}
		_passedInvokeTest = true;
		try
		{
			return Invocable(new CommandInvocationArguments
			{
				Bundle = Bundle,
				PipedArgument = pipedIn,
				Context = ctx
			});
		}
		catch (Exception exception)
		{
			ctx.ReportError(new UnhandledExceptionError(exception));
			return null;
		}
	}
}
