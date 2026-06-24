using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Toolshed.Syntax;

namespace Robust.Shared.Toolshed.TypeParsers;

public sealed class TypeTypeParser : TypeParser<Type>
{
	[Dependency]
	private readonly IModLoader _modLoader;

	public Dictionary<string, Type> Types = new Dictionary<string, Type>
	{
		{
			"object",
			typeof(object)
		},
		{
			"int",
			typeof(int)
		},
		{
			"uint",
			typeof(uint)
		},
		{
			"char",
			typeof(char)
		},
		{
			"byte",
			typeof(byte)
		},
		{
			"sbyte",
			typeof(sbyte)
		},
		{
			"short",
			typeof(short)
		},
		{
			"ushort",
			typeof(ushort)
		},
		{
			"long",
			typeof(ulong)
		},
		{
			"ulong",
			typeof(ulong)
		},
		{
			"string",
			typeof(string)
		},
		{
			"bool",
			typeof(bool)
		},
		{
			"nint",
			typeof(nint)
		},
		{
			"nuint",
			typeof(nuint)
		},
		{
			"float",
			typeof(float)
		},
		{
			"double",
			typeof(double)
		},
		{
			"decimal",
			typeof(decimal)
		},
		{
			"Vector2",
			typeof(Vector2)
		},
		{
			"TimeSpan",
			typeof(TimeSpan)
		},
		{
			"DateTime",
			typeof(DateTime)
		},
		{
			"IEnumerable",
			typeof(IEnumerable<>)
		},
		{
			"List",
			typeof(List<>)
		},
		{
			"HashSet",
			typeof(HashSet<>)
		},
		{
			"Task",
			typeof(Task<>)
		},
		{
			"ValueTask",
			typeof(ValueTask<>)
		},
		{
			"Dictionary",
			typeof(Dictionary<, >)
		}
	};

	private readonly HashSet<string> _ambiguousTypes = new HashSet<string>();

	private CompletionResult? _optionsCache;

	public override void PostInject()
	{
		foreach (Assembly item in _modLoader.LoadedModules.Append(Assembly.GetExecutingAssembly()).Append<Assembly>(Assembly.GetAssembly(typeof(Box2))))
		{
			foreach (Type exportedType in item.ExportedTypes)
			{
				string name = exportedType.Name;
				if (!_ambiguousTypes.Contains(name) && !Types.TryAdd(name, exportedType))
				{
					Types.Remove(name);
					_ambiguousTypes.Add(name);
				}
			}
		}
		_optionsCache = CompletionResult.FromHintOptions(Types.Select<KeyValuePair<string, Type>, CompletionOption>((KeyValuePair<string, Type> x) => new CompletionOption(x.Key)), "C# level type");
	}

	public override bool TryParse(ParserContext ctx, [NotNullWhen(true)] out Type? result)
	{
		string word = ctx.GetWord(Rune.IsLetterOrDigit);
		if (word == null)
		{
			ctx.Error = new OutOfInputError();
			result = null;
			return false;
		}
		Type type = ParseBase(word);
		if ((object)type == null)
		{
			ctx.Error = new UnknownType(word);
			result = null;
			return false;
		}
		if (type.IsGenericTypeDefinition)
		{
			if (!ctx.EatMatch('<'))
			{
				ctx.Error = new ExpectedGeneric();
				result = null;
				return false;
			}
			int num = type.GetGenericArguments().Length;
			Type[] array = new Type[type.GetGenericArguments().Length];
			for (int i = 0; i < num; i++)
			{
				if (!TryParse(ctx, out Type result2))
				{
					result = null;
					return false;
				}
				array[i] = result2;
				if (i != num - 1 && !ctx.EatMatch(','))
				{
					ctx.Error = new ExpectedNextType();
					result = null;
					return false;
				}
			}
			if (!ctx.EatMatch('>'))
			{
				ctx.Error = new ExpectedGeneric();
				result = null;
				return false;
			}
			type = type.MakeGenericType(array);
		}
		if (ctx.EatMatch('['))
		{
			if (!ctx.EatMatch(']'))
			{
				ctx.Error = new UnknownType(word);
				result = null;
				return false;
			}
			type = type.MakeArrayType();
		}
		if (ctx.EatMatch('?') && (type.IsValueType || type.IsPrimitive))
		{
			type = typeof(Nullable<>).MakeGenericType(type);
		}
		result = type;
		return true;
	}

	private Type? ParseBase(string word)
	{
		Types.TryGetValue(word, out Type value);
		return value;
	}

	public override CompletionResult? TryAutocomplete(ParserContext parserContext, CommandArgument? arg)
	{
		if (_optionsCache != null)
		{
			_optionsCache.Hint = GetArgHint(arg);
		}
		return _optionsCache;
	}
}
