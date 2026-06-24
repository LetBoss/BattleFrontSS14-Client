using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.TypeParsers.Math;

namespace Robust.Shared.Toolshed.Syntax;

public sealed class CommandRun
{
	public readonly string OriginalExpr;

	public readonly List<(ParsedCommand, Vector2i)> Commands;

	public readonly Type? ReturnType;

	public readonly Type? PipedType;

	public readonly int StartIndex;

	public readonly int EndIndex;

	public string SubExpr
	{
		get
		{
			string originalExpr = OriginalExpr;
			int startIndex = StartIndex;
			return originalExpr.Substring(startIndex, EndIndex - startIndex);
		}
	}

	public CommandRun(List<(ParsedCommand, Vector2i)> commands, string originalExpr, Type? returnType, Type? pipedType)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		OriginalExpr = originalExpr;
		Commands = commands;
		ReturnType = returnType;
		PipedType = pipedType;
		StartIndex = commands[0].Item2.X;
		EndIndex = commands[commands.Count - 1].Item2.Y;
	}

	public static bool TryParse(ParserContext ctx, Type? pipedType, Type? targetOutput, [NotNullWhen(true)] out CommandRun? expr)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		expr = null;
		List<(ParsedCommand, Vector2i)> list = new List<(ParsedCommand, Vector2i)>();
		ctx.ConsumeWhitespace();
		if (pipedType == typeof(void))
		{
			throw new ArgumentException("Piped type cannot be void");
		}
		int index = ctx.Index;
		if (ctx.PeekBlockTerminator())
		{
			ctx.Error = new EmptyCommandRun();
			ctx.Error.Contextualize(ctx.Input, new Vector2i(index, ctx.Index + 1));
			return false;
		}
		while (true)
		{
			if (!ParsedCommand.TryParse(ctx, pipedType, out ParsedCommand result))
			{
				if (ctx.Error is NotValidCommandError notValidCommandError)
				{
					notValidCommandError.TargetType = targetOutput;
				}
				return false;
			}
			pipedType = result.ReturnType;
			list.Add((result, Vector2i.op_Implicit((index, ctx.Index))));
			ctx.ConsumeWhitespace();
			ctx.EatCommandTerminators(ref pipedType, out var commandExpected);
			if (ctx.PeekBlockTerminator())
			{
				if (!commandExpected)
				{
					break;
				}
				if (!ctx.GenerateCompletions)
				{
					ctx.Error = new UnexpectedCloseBrace();
					ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
				}
				return false;
			}
			if (ctx.OutOfInput)
			{
				if (!commandExpected)
				{
					break;
				}
				if (ctx.GenerateCompletions)
				{
					ParsedCommand.TryParse(ctx, pipedType, out ParsedCommand _);
				}
				else
				{
					ctx.Error = new OutOfInputError();
				}
				return false;
			}
			index = ctx.Index;
			if (!(pipedType != typeof(void)))
			{
				if (ctx.GenerateCompletions)
				{
					return false;
				}
				ctx.Error = new EndOfCommandError();
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Index, ctx.Index + 1)));
				return false;
			}
		}
		if (ctx.Error != null || list.Count == 0)
		{
			expr = null;
			return false;
		}
		Type type;
		if (!(pipedType != null))
		{
			type = typeof(void);
		}
		else
		{
			type = list[list.Count - 1].Item1.ReturnType;
		}
		Type type2 = type;
		if (targetOutput != null && !type2.IsAssignableTo(targetOutput))
		{
			ctx.Error = new WrongCommandReturn(targetOutput, type2);
			expr = null;
			return false;
		}
		expr = new CommandRun(list, ctx.Input, type2, pipedType);
		return true;
	}

	public object? Invoke(object? input, IInvocationContext ctx, bool reportErrors = true)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (ctx.HasErrors)
		{
			throw new Exception("Improperly handled Toolshed errors");
		}
		object obj = input;
		foreach (var command in Commands)
		{
			ParsedCommand item = command.Item1;
			Vector2i item2 = command.Item2;
			obj = item.Invoke(obj, ctx);
			if (!ctx.HasErrors)
			{
				continue;
			}
			if (!reportErrors)
			{
				return null;
			}
			foreach (IConError error in ctx.GetErrors())
			{
				error.Contextualize(OriginalExpr, item2);
				ctx.WriteLine(error.Describe());
			}
			return null;
		}
		return obj;
	}

	public override string ToString()
	{
		return SubExpr;
	}
}
public sealed class CommandRun<TIn, TOut>
{
	internal readonly CommandRun InnerCommandRun;

	public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out CommandRun<TIn, TOut>? expr)
	{
		if (!CommandRun.TryParse(ctx, typeof(TIn), typeof(TOut), out CommandRun expr2))
		{
			expr = null;
			return false;
		}
		expr = new CommandRun<TIn, TOut>(expr2);
		return true;
	}

	public TOut? Invoke(object? input, IInvocationContext ctx)
	{
		object obj = InnerCommandRun.Invoke(input, ctx);
		if (obj == null)
		{
			return default(TOut);
		}
		return (TOut)obj;
	}

	internal CommandRun(CommandRun commandRun)
	{
		InnerCommandRun = commandRun;
	}

	public override string ToString()
	{
		return InnerCommandRun.ToString();
	}
}
public sealed class CommandRun<TRes>
{
	internal readonly CommandRun InnerCommandRun;

	public static bool TryParse(ParserContext ctx, Type? pipedType, [NotNullWhen(true)] out CommandRun<TRes>? expr)
	{
		if (!CommandRun.TryParse(ctx, pipedType, typeof(TRes), out CommandRun expr2))
		{
			expr = null;
			return false;
		}
		expr = new CommandRun<TRes>(expr2);
		return true;
	}

	public TRes? Invoke(object? input, IInvocationContext ctx)
	{
		object obj = InnerCommandRun.Invoke(input, ctx);
		if (obj == null)
		{
			return default(TRes);
		}
		return (TRes)obj;
	}

	internal CommandRun(CommandRun commandRun)
	{
		InnerCommandRun = commandRun;
	}

	public override string ToString()
	{
		return InnerCommandRun.ToString();
	}
}
