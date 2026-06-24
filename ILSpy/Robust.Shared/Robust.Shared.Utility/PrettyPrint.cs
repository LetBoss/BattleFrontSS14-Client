using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Utility;

public static class PrettyPrint
{
	private static readonly FrozenDictionary<Type, string> TypeShortHand = new Dictionary<Type, string>
	{
		{
			typeof(void),
			"void"
		},
		{
			typeof(object),
			"object"
		},
		{
			typeof(bool),
			"bool"
		},
		{
			typeof(byte),
			"byte"
		},
		{
			typeof(char),
			"char"
		},
		{
			typeof(decimal),
			"decimal"
		},
		{
			typeof(double),
			"double"
		},
		{
			typeof(float),
			"float"
		},
		{
			typeof(int),
			"int"
		},
		{
			typeof(long),
			"long"
		},
		{
			typeof(sbyte),
			"sbyte"
		},
		{
			typeof(short),
			"short"
		},
		{
			typeof(string),
			"string"
		},
		{
			typeof(uint),
			"uint"
		},
		{
			typeof(ulong),
			"ulong"
		},
		{
			typeof(ushort),
			"ushort"
		}
	}.ToFrozenDictionary();

	public static string? PrintUserFacing(object? value)
	{
		string typeRep;
		return PrintUserFacingWithType(value, out typeRep);
	}

	public static string PrintUserFacingWithType(object? value, out string typeRep)
	{
		if (value == null)
		{
			typeRep = string.Empty;
			return "null";
		}
		string result;
		if (value.GetType().GetMethod("ToString", new Type[0], new ParameterModifier[0]).DeclaringType == typeof(object))
		{
			result = TypeAbbreviation.Abbreviate(value.GetType());
			typeRep = string.Empty;
		}
		else if (value is EntityUid entityUid)
		{
			result = IoCManager.Resolve<IEntityManager>().ToPrettyString(entityUid);
			typeRep = TypeAbbreviation.Abbreviate(value.GetType());
		}
		else
		{
			result = value.ToString();
			typeRep = TypeAbbreviation.Abbreviate(value.GetType());
		}
		return result;
	}

	public static string PrintUserFacingTypeShort(Type type, int maxSections)
	{
		string text = TypeAbbreviation.Abbreviate(type);
		List<string> list = text.Split('.').ToList();
		int num = list.Count - maxSections - 1;
		if (num <= 0)
		{
			return text;
		}
		list.RemoveRange(maxSections, num);
		return string.Join('.', list);
	}

	public static string PrintTypeSignature(this Type type)
	{
		if (TypeShortHand.TryGetValue(type, out string value))
		{
			return value;
		}
		bool flag = false;
		if (type.IsNullable(out Type underlyingType))
		{
			type = underlyingType;
			flag = true;
		}
		if (type.IsGenericType)
		{
			return $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(PrintTypeSignature))}>{(flag ? "?" : string.Empty)}";
		}
		return type.Name;
	}

	public static string PrintParameterSignature(this ParameterInfo parameter)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (parameter.IsIn)
		{
			stringBuilder.Append("in ");
		}
		if (parameter.IsOut)
		{
			stringBuilder.Append("out ");
		}
		else if (parameter.ParameterType.IsByRef)
		{
			stringBuilder.Append("ref ");
		}
		stringBuilder.Append(parameter.ParameterType.PrintTypeSignature());
		string name = parameter.Name;
		if (name != null)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
			handler.AppendLiteral(" ");
			handler.AppendFormatted(name);
			stringBuilder3.Append(ref handler);
		}
		if (parameter.HasDefaultValue)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(3, 1, stringBuilder2);
			handler.AppendLiteral(" = ");
			handler.AppendFormatted(PrintUserFacing(parameter.DefaultValue));
			stringBuilder4.Append(ref handler);
		}
		return stringBuilder.ToString();
	}

	public static string PrintMethodSignature(this MethodInfo method, bool modifiers = false, bool arguments = true, bool returnType = true, bool name = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (modifiers)
		{
			if (method.IsPublic)
			{
				stringBuilder.Append("public ");
			}
			if (method.IsPrivate)
			{
				stringBuilder.Append("private ");
			}
			if (method.IsFamilyAndAssembly)
			{
				stringBuilder.Append("private protected ");
			}
			if (method.IsFamily)
			{
				stringBuilder.Append("protected ");
			}
			if (method.IsFamilyOrAssembly)
			{
				stringBuilder.Append("protected internal ");
			}
			if (method.IsAssembly)
			{
				stringBuilder.Append("internal ");
			}
			if (method.IsStatic)
			{
				stringBuilder.Append("static ");
			}
			Type declaringType;
			if (method.IsAbstract)
			{
				declaringType = method.DeclaringType;
				if ((object)declaringType != null && declaringType.IsAbstract && !declaringType.IsInterface)
				{
					stringBuilder.Append("abstract ");
					goto IL_0119;
				}
			}
			declaringType = method.DeclaringType;
			if ((object)declaringType != null && !declaringType.IsInterface)
			{
				if (method.IsFinal)
				{
					stringBuilder.Append("sealed override ");
				}
				else if (method.IsVirtual)
				{
					stringBuilder.Append(method.Equals(method.GetBaseDefinition()) ? "virtual " : "override ");
				}
			}
		}
		goto IL_0119;
		IL_0119:
		StringBuilder stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler;
		if (returnType && !method.IsConstructor)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(1, 1, stringBuilder2);
			handler.AppendFormatted(method.ReturnType.PrintTypeSignature());
			handler.AppendLiteral(" ");
			stringBuilder3.Append(ref handler);
		}
		if (name)
		{
			stringBuilder.Append(method.Name);
		}
		if (!arguments)
		{
			return stringBuilder.ToString();
		}
		if (method.IsGenericMethod)
		{
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
			handler.AppendLiteral("<");
			handler.AppendFormatted(string.Join(", ", method.GetGenericArguments().Select(PrintTypeSignature)));
			handler.AppendLiteral(">");
			stringBuilder4.Append(ref handler);
		}
		stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder5 = stringBuilder2;
		handler = new StringBuilder.AppendInterpolatedStringHandler(2, 1, stringBuilder2);
		handler.AppendLiteral("(");
		handler.AppendFormatted(string.Join(", ", method.GetParameters().Select(PrintParameterSignature)));
		handler.AppendLiteral(")");
		stringBuilder5.Append(ref handler);
		return stringBuilder.ToString();
	}

	public static string PrintPropertySignature(this PropertyInfo property, bool modifiers = false, bool accessors = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder stringBuilder3 = stringBuilder2;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
		handler.AppendFormatted(property.PropertyType.PrintTypeSignature());
		handler.AppendLiteral(" ");
		handler.AppendFormatted(property.Name);
		stringBuilder3.Append(ref handler);
		if (accessors)
		{
			stringBuilder.Append(" { ");
			if (property.CanRead)
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
				handler.AppendFormatted(property.GetMethod.PrintMethodSignature(modifiers, arguments: false, returnType: false, name: false));
				handler.AppendLiteral("get; ");
				stringBuilder4.Append(ref handler);
			}
			if (property.CanWrite)
			{
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
				handler.AppendFormatted(property.SetMethod.PrintMethodSignature(modifiers, arguments: false, returnType: false, name: false));
				handler.AppendLiteral("set; ");
				stringBuilder5.Append(ref handler);
			}
			stringBuilder.Append('}');
		}
		return stringBuilder.ToString();
	}

	public static string PrintFieldSignature(this FieldInfo field, bool modifiers = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (modifiers)
		{
			if (field.IsPublic)
			{
				stringBuilder.Append("public ");
			}
			if (field.IsPrivate)
			{
				stringBuilder.Append("private ");
			}
			if (field.IsFamilyAndAssembly)
			{
				stringBuilder.Append("private protected ");
			}
			if (field.IsFamily)
			{
				stringBuilder.Append("protected ");
			}
			if (field.IsFamilyOrAssembly)
			{
				stringBuilder.Append("protected internal ");
			}
			if (field.IsAssembly)
			{
				stringBuilder.Append("internal ");
			}
			if (field.IsStatic)
			{
				stringBuilder.Append("static ");
			}
		}
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
		handler.AppendFormatted(field.FieldType.PrintTypeSignature());
		handler.AppendLiteral(" ");
		handler.AppendFormatted(field.Name);
		stringBuilder2.Append(ref handler);
		return stringBuilder.ToString();
	}
}
