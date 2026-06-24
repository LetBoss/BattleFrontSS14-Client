using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Toolshed.Errors;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;

namespace Robust.Shared.Toolshed.Commands.Generic;

[ToolshedCommand]
public sealed class EmplaceCommand : ToolshedCommand
{
	private record EmplaceContext<T> : IInvocationContext
	{
		public ICommonSession? Session => _inner.Session;

		public ToolshedManager Toolshed => _inner.Toolshed;

		public NetUserId? User => _inner.User;

		public ToolshedEnvironment Environment => _inner.Environment;

		public bool HasErrors => _inner.HasErrors;

		public T? Value;

		private readonly IInvocationContext _inner;

		private readonly IEntityManager _entMan;

		private readonly HashSet<string> _localVars;

		public EmplaceContext(IInvocationContext inner, IEntityManager entMan, T? value = default(T?))
		{
			_inner = inner;
			_entMan = entMan;
			Value = value;
			_localVars.Add("value");
			if (typeof(T) == typeof(EntityUid))
			{
				_localVars.Add("wx");
				_localVars.Add("wy");
				_localVars.Add("proto");
				_localVars.Add("name");
				_localVars.Add("desc");
				_localVars.Add("paused");
			}
			else if (typeof(T).IsAssignableTo(typeof(ICommonSession)))
			{
				_localVars.Add("ent");
				_localVars.Add("name");
				_localVars.Add("userid");
			}
		}

		public bool CheckInvokable(CommandSpec command, out IConError? error)
		{
			return _inner.CheckInvokable(command, out error);
		}

		public void WriteLine(string line)
		{
			_inner.WriteLine(line);
		}

		public void ReportError(IConError err)
		{
			_inner.ReportError(err);
		}

		public IEnumerable<IConError> GetErrors()
		{
			return _inner.GetErrors();
		}

		public void ClearErrors()
		{
			_inner.ClearErrors();
		}

		public IEnumerable<string> GetVars()
		{
			foreach (string localVar in _localVars)
			{
				yield return localVar;
			}
			foreach (string var in _inner.GetVars())
			{
				if (!_localVars.Contains(var))
				{
					yield return var;
				}
			}
		}

		public object? ReadVar(string name)
		{
			if (name == "value")
			{
				return Value;
			}
			T value = Value;
			if (!(value is EntityUid uid))
			{
				if (value is ICommonSession commonSession)
				{
					return name switch
					{
						"ent" => commonSession.AttachedEntity, 
						"name" => commonSession.Name, 
						"userid" => commonSession.UserId, 
						_ => _inner.ReadVar(name), 
					};
				}
				return _inner.ReadVar(name);
			}
			return name switch
			{
				"wx" => _entMan.System<SharedTransformSystem>().GetWorldPosition(uid).X, 
				"wy" => _entMan.System<SharedTransformSystem>().GetWorldPosition(uid).Y, 
				"proto" => _entMan.GetComponent<MetaDataComponent>(uid).EntityPrototype?.ID ?? "", 
				"desc" => _entMan.GetComponent<MetaDataComponent>(uid).EntityDescription, 
				"name" => _entMan.GetComponent<MetaDataComponent>(uid).EntityName, 
				"paused" => _entMan.GetComponent<MetaDataComponent>(uid).EntityPaused, 
				_ => _inner.ReadVar(name), 
			};
		}

		public void WriteVar(string name, object? value)
		{
			if (_localVars.Contains(name))
			{
				ReportError(new ReadonlyVariableError(name));
			}
			else
			{
				_inner.WriteVar(name, value);
			}
		}

		public bool IsReadonlyVar(string name)
		{
			return _localVars.Contains(name);
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Value = ");
			builder.Append(Value);
			builder.Append(", Session = ");
			builder.Append(Session);
			builder.Append(", Toolshed = ");
			builder.Append(Toolshed);
			builder.Append(", User = ");
			builder.Append(User.ToString());
			builder.Append(", Environment = ");
			builder.Append(Environment);
			builder.Append(", HasErrors = ");
			builder.Append(HasErrors.ToString());
			return true;
		}
	}

	private sealed class EmplaceBlockParser : CustomTypeParser<Block>
	{
		public static bool TryParse(ParserContext ctx, [NotNullWhen(true)] out CommandRun? result)
		{
			if (ctx.Bundle.PipedType == null)
			{
				result = null;
				return false;
			}
			Type type = ctx.Bundle.PipedType;
			if (type.IsGenericType(typeof(IEnumerable<>)))
			{
				type = type.GetGenericArguments()[0];
			}
			LocalVarParser localVarParser = SetupVarParser(ctx, type);
			bool result2 = Block.TryParseBlock(ctx, null, null, out result);
			ctx.VariableParser = localVarParser.Inner;
			return result2;
		}

		private static LocalVarParser SetupVarParser(ParserContext ctx, Type input)
		{
			LocalVarParser localVarParser = (LocalVarParser)(ctx.VariableParser = new LocalVarParser(ctx.VariableParser));
			localVarParser.SetLocalType("value", input, @readonly: true);
			if (input == typeof(EntityUid))
			{
				localVarParser.SetLocalType("wx", typeof(float), @readonly: true);
				localVarParser.SetLocalType("wy", typeof(float), @readonly: true);
				localVarParser.SetLocalType("proto", typeof(string), @readonly: true);
				localVarParser.SetLocalType("desc", typeof(string), @readonly: true);
				localVarParser.SetLocalType("name", typeof(string), @readonly: true);
				localVarParser.SetLocalType("paused", typeof(bool), @readonly: true);
			}
			else if (input.IsAssignableTo(typeof(ICommonSession)))
			{
				localVarParser.SetLocalType("ent", typeof(EntityUid), @readonly: true);
				localVarParser.SetLocalType("name", typeof(string), @readonly: true);
				localVarParser.SetLocalType("userid", typeof(NetUserId), @readonly: true);
			}
			return localVarParser;
		}

		public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Block? result)
		{
			result = null;
			if (!TryParse(ctx, out CommandRun result2))
			{
				return false;
			}
			result = new Block(result2);
			return true;
		}

		public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
		{
			TryParse(ctx, out CommandRun _);
			return ctx.Completions;
		}
	}

	private sealed class EmplaceBlockOutputParser : CustomTypeParser<Type>
	{
		public override bool ShowTypeArgSignature => false;

		public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
		{
			result = null;
			ParserRestorePoint point = ctx.Save();
			if (!EmplaceBlockParser.TryParse(ctx, out CommandRun result2))
			{
				return false;
			}
			if (result2.ReturnType == null)
			{
				return false;
			}
			ctx.Restore(point);
			result = result2.ReturnType;
			return true;
		}

		public override CompletionResult? TryAutocomplete(ParserContext ctx, CommandArgument? arg)
		{
			EmplaceBlockParser.TryParse(ctx, out CommandRun _);
			return ctx.Completions;
		}
	}

	private static Type[] _parsers = new Type[1] { typeof(EmplaceBlockOutputParser) };

	public override Type[] TypeParameterParsers => _parsers;

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	private TOut Emplace<TOut, TIn>(IInvocationContext ctx, [PipedArgument] TIn value, [CommandArgument(typeof(EmplaceBlockParser), false)] Block block)
	{
		EmplaceContext<TIn> emplaceContext = new EmplaceContext<TIn>(ctx, EntityManager);
		emplaceContext.Value = value;
		return (TOut)block.Invoke(null, emplaceContext);
	}

	[CommandImplementation(null)]
	[TakesPipedTypeAsGeneric]
	private IEnumerable<TOut> Emplace<TOut, TIn>(IInvocationContext ctx, [PipedArgument] IEnumerable<TIn> value, [CommandArgument(typeof(EmplaceBlockParser), false)] Block block)
	{
		EmplaceContext<TIn> emplaceCtx = new EmplaceContext<TIn>(ctx, EntityManager);
		foreach (TIn item in value)
		{
			if (!ctx.HasErrors)
			{
				emplaceCtx.Value = item;
				yield return (TOut)block.Invoke(null, emplaceCtx);
				continue;
			}
			yield break;
		}
	}
}
