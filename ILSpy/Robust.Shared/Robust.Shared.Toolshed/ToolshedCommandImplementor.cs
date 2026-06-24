using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Robust.Shared.Exceptions;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

internal sealed class ToolshedCommandImplementor
{
	public readonly ToolshedCommand Owner;

	public readonly string? SubCommand;

	public readonly string FullName;

	public readonly string LocName;

	private readonly ToolshedManager _toolshed;

	private readonly ILocalizationManager _loc;

	public readonly Dictionary<CommandDiscriminator, Func<CommandInvocationArguments, object?>> Implementations = new Dictionary<CommandDiscriminator, Func<CommandInvocationArguments, object>>();

	private readonly Dictionary<CommandDiscriminator, ConcreteCommandMethod?> _methodCache = new Dictionary<CommandDiscriminator, ConcreteCommandMethod?>();

	internal readonly CommandMethod[] Methods;

	public CommandSpec Spec => new CommandSpec(Owner, SubCommand);

	public ToolshedCommandImplementor(string? subCommand, ToolshedCommand owner, ToolshedManager toolshed, ILocalizationManager loc)
	{
		Owner = owner;
		_loc = loc;
		SubCommand = subCommand;
		FullName = ((SubCommand == null) ? Owner.Name : (Owner.Name + ":" + SubCommand));
		_toolshed = toolshed;
		Methods = (from x in Owner.GetMethods(SubCommand)
			select new CommandMethod(x, this)).ToArray();
		LocName = (Owner.Name.All(char.IsAsciiLetterOrDigit) ? Owner.Name : Owner.GetType().PrettyName());
		if (SubCommand != null)
		{
			LocName = LocName + "-" + SubCommand;
		}
	}

	public bool TryParse(ParserContext ctx, out Func<CommandInvocationArguments, object?>? invocable, [NotNullWhen(true)] out ConcreteCommandMethod? method)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		ctx.ConsumeWhitespace();
		method = null;
		invocable = null;
		if (!TryParseTypeArguments(ctx))
		{
			return false;
		}
		if (!TryGetConcreteMethod(ctx.Bundle.PipedType, ctx.Bundle.TypeArguments, out method))
		{
			if (ctx.GenerateCompletions)
			{
				return false;
			}
			ctx.Error = new NoImplementationError(ctx);
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((ctx.Bundle.NameStart, ctx.Bundle.NameEnd)));
			return false;
		}
		int index = ctx.Index;
		if (!TryParseArguments(ctx, method.Value))
		{
			ctx.Error?.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
			return false;
		}
		invocable = GetImplementation(ctx.Bundle, method.Value);
		return true;
	}

	public bool TryParseArguments(ParserContext ctx, ConcreteCommandMethod method)
	{
		CommandArgument[] args = method.Args;
		for (int i = 0; i < args.Length; i++)
		{
			CommandArgument arg = args[i];
			object parsed;
			if (arg.IsParamsCollection)
			{
				if (!ParseParamsCollection(ctx, arg, out parsed))
				{
					return false;
				}
			}
			else if (!TryParseArgument(ctx, arg, out parsed))
			{
				return false;
			}
			ref Dictionary<string, object> arguments = ref ctx.Bundle.Arguments;
			if (arguments == null)
			{
				arguments = new Dictionary<string, object>();
			}
			ctx.Bundle.Arguments[arg.Name] = parsed;
		}
		return true;
	}

	private bool ParseParamsCollection(ParserContext ctx, CommandArgument arg, out object? collection)
	{
		List<object> list = (List<object>)(collection = new List<object>());
		while (true)
		{
			ctx.ConsumeWhitespace();
			if (ctx.PeekCommandOrBlockTerminated() || (ctx != null && ctx.OutOfInput && !ctx.GenerateCompletions))
			{
				break;
			}
			if (!TryParseArgument(ctx, arg, out object parsed))
			{
				return false;
			}
			list.Add(parsed);
		}
		return true;
	}

	private bool TryParseArgument(ParserContext ctx, CommandArgument arg, out object? parsed)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		int index = ctx.Index;
		ParserRestorePoint point = ctx.Save();
		ctx.ConsumeWhitespace();
		parsed = null;
		ParserContext parserContext;
		if (ctx.PeekCommandOrBlockTerminated() || (ctx != null && ctx.OutOfInput && !ctx.GenerateCompletions))
		{
			if (!arg.IsOptional)
			{
				ctx.Error = new ExpectedArgumentError(arg.Type);
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
				return false;
			}
			parsed = arg.DefaultValue;
		}
		else if (!arg.Parser.TryParse(ctx, out parsed))
		{
			if (ctx.GenerateCompletions)
			{
				if (!ctx.OutOfInput)
				{
					return false;
				}
				if (ctx.Completions != null)
				{
					return false;
				}
				ctx.Restore(point);
				ctx.Error = null;
				parserContext = ctx;
				if ((object)parserContext.Completions == null)
				{
					parserContext.Completions = arg.Parser.TryAutocomplete(ctx, arg);
				}
				TrySetArgHint(ctx, arg);
				return false;
			}
			int item = Math.Max(index + 1, ctx.Index);
			parserContext = ctx;
			if (parserContext.Error == null)
			{
				parserContext.Error = new ArgumentParseError(arg.Type, arg.Parser.GetType());
			}
			ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, item)));
			return false;
		}
		if (!ctx.GenerateCompletions || !ctx.OutOfInput)
		{
			return true;
		}
		ctx.Restore(point);
		ctx.Error = null;
		parserContext = ctx;
		if ((object)parserContext.Completions == null)
		{
			parserContext.Completions = arg.Parser.TryAutocomplete(ctx, arg);
		}
		TrySetArgHint(ctx, arg);
		return false;
	}

	private void TrySetArgHint(ParserContext ctx, CommandArgument arg)
	{
		if (!(ctx.Completions == null) && _loc.TryGetString("command-arg-hint-" + LocName + "-" + arg.Name, out string value))
		{
			ctx.Completions.Hint = value;
		}
	}

	internal bool TryParseTypeArguments(ParserContext ctx)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		if (Owner.TypeParameterParsers.Length == 0)
		{
			return true;
		}
		ref Type[] typeArguments = ref ctx.Bundle.TypeArguments;
		typeArguments = new Type[Owner.TypeParameterParsers.Length];
		for (int i = 0; i < Owner.TypeParameterParsers.Length; i++)
		{
			Type type = Owner.TypeParameterParsers[i];
			int index = ctx.Index;
			ctx.ConsumeWhitespace();
			ParserRestorePoint point = ctx.Save();
			if ((ctx != null && ctx.OutOfInput && !ctx.GenerateCompletions) || ctx.PeekCommandOrBlockTerminated())
			{
				ctx.Error = new ExpectedTypeArgumentError();
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index + 1)));
				return false;
			}
			BaseParser<Type> baseParser = (BaseParser<Type>)((type == typeof(TypeTypeParser)) ? _toolshed.GetParserForType(typeof(Type)) : _toolshed.GetCustomParser(type));
			if (!baseParser.TryParse(ctx, out Type result))
			{
				typeArguments = null;
				ParserContext parserContext;
				if (ctx.GenerateCompletions)
				{
					if (!ctx.OutOfInput)
					{
						return false;
					}
					ctx.Restore(point);
					ctx.Error = null;
					parserContext = ctx;
					if ((object)parserContext.Completions == null)
					{
						parserContext.Completions = baseParser.TryAutocomplete(ctx, null);
					}
					return false;
				}
				parserContext = ctx;
				if (parserContext.Error == null)
				{
					parserContext.Error = new TypeArgumentParseError(type);
				}
				ctx.Error.Contextualize(ctx.Input, Vector2i.op_Implicit((index, ctx.Index)));
				return false;
			}
			typeArguments[i] = result;
			if (ctx.GenerateCompletions && ctx.OutOfInput)
			{
				ctx.Restore(point);
				ctx.Error = null;
				ctx.Completions = baseParser.TryAutocomplete(ctx, null);
				return false;
			}
		}
		return true;
	}

	internal bool TryGetConcreteMethod(Type? pipedType, Type[]? typeArguments, [NotNullWhen(true)] out ConcreteCommandMethod? method)
	{
		CommandDiscriminator key = new CommandDiscriminator(pipedType, typeArguments);
		if (_methodCache.TryGetValue(key, out method))
		{
			return method.HasValue;
		}
		(CommandMethod, MethodInfo)? concreteMethodInternal = GetConcreteMethodInternal(pipedType, typeArguments);
		if (!concreteMethodInternal.HasValue)
		{
			_methodCache[key] = (method = null);
			return false;
		}
		var (commandMethod, methodInfo) = concreteMethodInternal.Value;
		if (((object)pipedType != null && pipedType.ContainsGenericParameters) || (typeArguments != null && typeArguments.Any((Type x) => x.ContainsGenericParameters)))
		{
			_methodCache[key] = (method = new ConcreteCommandMethod(methodInfo, null, commandMethod));
			return true;
		}
		CommandArgument[] args = (from x in methodInfo.GetParameters()
			where x.IsCommandArgument()
			select x).Select(GetCommandArgument).ToArray();
		_methodCache[key] = (method = new ConcreteCommandMethod(methodInfo, args, commandMethod));
		return true;
	}

	internal CommandArgument GetCommandArgument(ParameterInfo arg)
	{
		Type type = arg.ParameterType;
		bool flag = arg.HasCustomAttribute<ParamArrayAttribute>();
		if (flag)
		{
			if (!type.IsArray)
			{
				throw new NotSupportedException(".net 9 params collections are not yet supported");
			}
			type = type.GetElementType();
		}
		return new CommandArgument(arg.Name, type, GetArgumentParser(arg, type), arg.IsOptional, arg.DefaultValue, flag);
	}

	private ITypeParser? GetArgumentParser(ParameterInfo param, Type type)
	{
		if (type.ContainsGenericParameters)
		{
			return null;
		}
		Type type2 = param.GetCustomAttribute<CommandArgumentAttribute>()?.CustomParser;
		return (((object)type2 == null) ? _toolshed.GetArgumentParser(type) : _toolshed.GetArgumentParser(_toolshed.GetCustomParser(type2))) ?? throw new Exception($"No parser for type: {param.ParameterType}");
	}

	private (CommandMethod, MethodInfo)? GetConcreteMethodInternal(Type? pipedType, Type[]? typeArguments)
	{
		return Methods.Where(delegate(CommandMethod x)
		{
			ParameterInfo pipeArg = x.PipeArg;
			if (pipeArg == null)
			{
				return (object)pipedType == null;
			}
			if (pipedType == null)
			{
				return false;
			}
			return x.Generic || _toolshed.IsTransformableTo(pipedType, pipeArg.ParameterType);
		}).OrderByDescending(delegate(CommandMethod x)
		{
			ParameterInfo pipeArg = x.PipeArg;
			return (pipeArg != null) ? GetMethodRating(pipedType, pipeArg.ParameterType) : 0;
		}).Select(delegate(CommandMethod x)
		{
			if (!x.Generic)
			{
				return (x, x.Info);
			}
			try
			{
				if (!x.PipeGeneric)
				{
					return (x, x.Info.MakeGenericMethod(typeArguments));
				}
				Type genericTypeFromPiped = GetGenericTypeFromPiped(pipedType, x.PipeArg.ParameterType);
				return (x, x.Info.MakeGenericMethod(typeArguments?.Append(genericTypeFromPiped).ToArray() ?? new Type[1] { genericTypeFromPiped }));
			}
			catch (ArgumentException)
			{
				return ((CommandMethod, MethodInfo)?)null;
			}
		})
			.FirstOrDefault(((CommandMethod, MethodInfo)? x) => x.HasValue);
	}

	private int GetMethodRating(Type? pipedType, Type paramType)
	{
		if (pipedType.IsAssignableTo(paramType))
		{
			return 1000;
		}
		if (!paramType.ContainsGenericParameters)
		{
			if (paramType.GetMostGenericPossible() == pipedType.GetMostGenericPossible())
			{
				return 500;
			}
			return 400;
		}
		if (paramType.GetMostGenericPossible() == pipedType.GetMostGenericPossible())
		{
			return 300;
		}
		if (paramType.IsGenericParameter)
		{
			return Math.Min(100 + paramType.GetGenericParameterConstraints().Length, 299);
		}
		return 0;
	}

	public static Type GetGenericTypeFromPiped(Type inputType, Type parameterType)
	{
		return inputType.Intersect(parameterType);
	}

	public Func<CommandInvocationArguments, object?> GetImplementation(CommandArgumentBundle args, ConcreteCommandMethod method)
	{
		CommandDiscriminator key = new CommandDiscriminator(args.PipedType, args.TypeArguments);
		if (!Implementations.TryGetValue(key, out Func<CommandInvocationArguments, object> value))
		{
			value = (Implementations[key] = GetImplementationInternal(args, method));
		}
		return value;
	}

	internal Func<CommandInvocationArguments, object?> GetImplementationInternal(CommandArgumentBundle cmdArgs, ConcreteCommandMethod method)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(CommandInvocationArguments));
		List<Expression> list = new List<Expression>();
		ParameterInfo[] parameters = method.Info.GetParameters();
		foreach (ParameterInfo param in parameters)
		{
			list.Add(GetParamExpr(param, cmdArgs.PipedType, parameterExpression));
		}
		Expression expression = Expression.Call(Expression.Constant(Owner), method.Info, list);
		Type returnType = method.Info.ReturnType;
		if (returnType == typeof(void))
		{
			expression = Expression.Block(expression, Expression.Constant(null));
		}
		else if (returnType.IsValueType)
		{
			expression = Expression.Convert(expression, typeof(object));
		}
		return Expression.Lambda<Func<CommandInvocationArguments, object>>(expression, new ParameterExpression[1] { parameterExpression }).Compile();
	}

	private Expression GetParamExpr(ParameterInfo param, Type? pipedType, ParameterExpression args)
	{
		if (param.HasCustomAttribute<PipedArgumentAttribute>())
		{
			if ((object)pipedType == null)
			{
				throw new TypeArgumentException();
			}
			return _toolshed.GetTransformer(pipedType, param.ParameterType, Expression.Field(args, "PipedArgument"));
		}
		if (param.HasCustomAttribute<CommandInvertedAttribute>())
		{
			return Expression.Property(args, "Inverted");
		}
		if (param.HasCustomAttribute<CommandArgumentAttribute>())
		{
			return GetArgExpr(param, args);
		}
		if (param.HasCustomAttribute<CommandInvocationContextAttribute>() || param.ParameterType == typeof(IInvocationContext))
		{
			return Expression.Property(args, "Context");
		}
		return GetArgExpr(param, args);
	}

	private Expression GetArgExpr(ParameterInfo param, ParameterExpression args)
	{
		IndexExpression arg = Expression.MakeIndex(Expression.Property(args, "Arguments"), typeof(Dictionary<string, object>).FindIndexerProperty(), new ConstantExpression[1] { Expression.Constant(param.Name) });
		MemberExpression arg2 = Expression.Property(args, "Context");
		MethodInfo method = ((!param.HasCustomAttribute<ParamArrayAttribute>()) ? typeof(ValueRef<>).MakeGenericType(param.ParameterType).GetMethod("EvaluateParameter", BindingFlags.Static | BindingFlags.NonPublic) : typeof(ValueRef<>).MakeGenericType(param.ParameterType.GetElementType()).GetMethod("EvaluateParamsCollection", BindingFlags.Static | BindingFlags.NonPublic));
		return Expression.Call(method, arg, arg2);
	}

	public override string ToString()
	{
		return FullName;
	}

	public string GetHelp()
	{
		if (_loc.TryGetString("command-help-" + LocName, out string value))
		{
			return value;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (Methods.Any((CommandMethod x) => x.Invertible))
		{
			stringBuilder.AppendLine(_loc.GetString("command-help-invertible"));
		}
		stringBuilder.Append(_loc.GetString("command-help-usage"));
		CommandMethod[] methods = Methods;
		foreach (CommandMethod commandMethod in methods)
		{
			stringBuilder.Append(Environment.NewLine + "  ");
			ParameterInfo pipeArg = commandMethod.PipeArg;
			if (pipeArg != null)
			{
				string messageId = "command-arg-sig-" + LocName + "-" + pipeArg.Name;
				if (!_loc.TryGetString(messageId, out string value2))
				{
					value2 = ToolshedCommand.GetArgHint(pipeArg.Name, optional: false, isParams: false, pipeArg.ParameterType);
				}
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
				handler.AppendFormatted(value2);
				handler.AppendLiteral(" → ");
				stringBuilder3.Append(ref handler);
			}
			if (commandMethod.Invertible)
			{
				stringBuilder.Append("[not] ");
			}
			AddMethodSignature(stringBuilder, commandMethod.Arguments);
			if (commandMethod.Info.ReturnType != typeof(void))
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
				handler.AppendLiteral(" → ");
				handler.AppendFormatted(commandMethod.Info.ReturnType.PrettyName());
				stringBuilder4.Append(ref handler);
			}
		}
		return stringBuilder.ToString();
	}

	internal void AddMethodSignature(StringBuilder builder, CommandArgument[] args, Type[]? typeArgs = null)
	{
		builder.Append(FullName);
		Type[] typeParameterParsers = Owner.TypeParameterParsers;
		int num = 0;
		Type[] array = typeParameterParsers;
		foreach (Type type in array)
		{
			if (!(type == typeof(TypeTypeParser)) && _toolshed.GetCustomParser(type).ShowTypeArgSignature)
			{
				num++;
			}
		}
		if (num > 0)
		{
			builder.Append('<');
			for (int j = 0; j < num; j++)
			{
				if (j > 0)
				{
					builder.Append(", ");
				}
				if (typeArgs != null)
				{
					builder.Append(typeArgs[j].PrettyName());
					continue;
				}
				builder.Append('T');
				if (num > 1)
				{
					builder.Append(j + 1);
				}
			}
			builder.Append('>');
		}
		for (int i = 0; i < args.Length; i++)
		{
			CommandArgument value = args[i];
			builder.Append(' ');
			if (_loc.TryGetString("command-arg-sig-" + LocName + "-" + value.Name, out string value2))
			{
				builder.Append(value2);
			}
			else
			{
				builder.Append(ToolshedCommand.GetArgHint(value, value.Type));
			}
		}
	}

	public string DescriptionLocKey()
	{
		return "command-description-" + LocName;
	}

	public string Description()
	{
		return _loc.GetString(DescriptionLocKey());
	}
}
