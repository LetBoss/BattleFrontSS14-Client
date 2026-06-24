using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Robust.Shared.Exceptions;
using Robust.Shared.Log;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;

namespace Robust.Shared.Toolshed;

internal static class ReflectionExtensions
{
	public static bool CanBeNull(this Type t)
	{
		if (t.IsValueType)
		{
			return t.IsGenericType(typeof(Nullable<>));
		}
		return true;
	}

	public static bool CanBeEmpty(this Type t)
	{
		if (!t.CanBeNull())
		{
			return t.IsGenericType(typeof(IEnumerable<>));
		}
		return true;
	}

	public static bool IsGenericType(this Type t, Type genericType)
	{
		if (t.IsGenericType)
		{
			return t.GetGenericTypeDefinition() == genericType;
		}
		return false;
	}

	public static IEnumerable<Type> GetVariants(this Type t, ToolshedManager toolshed)
	{
		Type[] args = t.GetGenericArguments();
		Type generic = t.GetGenericTypeDefinition();
		Type[] genericArgs = generic.GetGenericArguments();
		int variantCount = genericArgs.Count((Type x) => (x.GenericParameterAttributes & GenericParameterAttributes.VarianceMask) != 0);
		if (variantCount > 1)
		{
			throw new NotImplementedException("I swear to god I am NOT supporting more than one variant type parameter. Absolutely no combinatorial explosions in this house, factorials can go home.");
		}
		yield return t;
		if (variantCount < 1)
		{
			yield break;
		}
		int variant = 0;
		for (int num = 0; num < args.Length; num++)
		{
			if ((genericArgs[num].GenericParameterAttributes & GenericParameterAttributes.VarianceMask) != GenericParameterAttributes.None)
			{
				variant = num;
				break;
			}
		}
		Type[] newArgs = (Type[])args.Clone();
		foreach (Type item in toolshed.AllSteppedTypes(args[variant], allowVariants: false))
		{
			newArgs[variant] = item;
			yield return generic.MakeGenericType(newArgs);
		}
	}

	public static Type StepDownConstraints(this Type t)
	{
		if (!t.IsGenericType || t.IsGenericTypeDefinition)
		{
			return t;
		}
		Type[] genericTypeArguments = t.GenericTypeArguments;
		Type[] array = new Type[genericTypeArguments.Length];
		for (int i = 0; i < genericTypeArguments.Length; i++)
		{
			if (genericTypeArguments[i].IsGenericType)
			{
				array[i] = genericTypeArguments[i].GetGenericTypeDefinition();
			}
			else
			{
				array[i] = genericTypeArguments[i];
			}
		}
		return t.GetGenericTypeDefinition().MakeGenericType(array);
	}

	public static bool HasGenericParent(this Type type, Type parent)
	{
		Type type2 = type;
		while (type2 != null)
		{
			if (type2.IsGenericType(parent))
			{
				return true;
			}
			type2 = type2.BaseType;
		}
		return false;
	}

	public static bool IsValueRef(this Type type)
	{
		return type.HasGenericParent(typeof(ValueRef<>));
	}

	public static bool IsCustomParser(this Type type)
	{
		return type.HasGenericParent(typeof(CustomTypeParser<>));
	}

	public static bool IsParser(this Type type)
	{
		return type.HasGenericParent(typeof(TypeParser<>));
	}

	public static bool IsCommandArgument(this ParameterInfo param)
	{
		if (param.HasCustomAttribute<CommandArgumentAttribute>())
		{
			return true;
		}
		if (param.HasCustomAttribute<CommandInvertedAttribute>())
		{
			return false;
		}
		if (param.HasCustomAttribute<PipedArgumentAttribute>())
		{
			return false;
		}
		if (param.HasCustomAttribute<CommandInvocationContextAttribute>())
		{
			return false;
		}
		return param.ParameterType != typeof(IInvocationContext);
	}

	public static string PrettyName(this Type type)
	{
		string text = type.Name;
		if (type.IsGenericParameter)
		{
			return type.ToString();
		}
		if ((object)type.DeclaringType != null)
		{
			text = type.DeclaringType.PrettyName() + "." + type.Name;
		}
		if (type.GetGenericArguments().Length == 0)
		{
			return text;
		}
		if (!text.Contains('`'))
		{
			return text + "<>";
		}
		Type[] genericArguments = type.GetGenericArguments();
		return text.Substring(0, text.IndexOf('`', StringComparison.InvariantCulture)) + "<" + string.Join(",", genericArguments.Select(PrettyName)) + ">";
	}

	public static ParameterInfo? ConsoleGetPipedArgument(this MethodInfo method)
	{
		return method.GetParameters().SingleOrDefault((ParameterInfo x) => x.HasCustomAttribute<PipedArgumentAttribute>());
	}

	public static bool ConsoleHasInvertedArgument(this MethodInfo method)
	{
		return method.GetParameters().Any((ParameterInfo x) => x.HasCustomAttribute<CommandInvertedAttribute>());
	}

	public static Expression CreateEmptyExpr(this Type t)
	{
		if (!t.CanBeEmpty())
		{
			throw new TypeArgumentException();
		}
		if (t.IsGenericType(typeof(IEnumerable<>)))
		{
			return Expression.Constant(Array.CreateInstance(t.GetGenericArguments().First(), 0), t);
		}
		if (t.CanBeNull())
		{
			if ((object)Nullable.GetUnderlyingType(t) != null)
			{
				return Expression.Constant(t.GetConstructor(BindingFlags.CreateInstance, Array.Empty<Type>()).Invoke(null, null), t);
			}
			return Expression.Constant(null, t);
		}
		throw new NotImplementedException();
	}

	public static Type Intersect(this Type left, Type right)
	{
		if (left.IsArray)
		{
			return typeof(List<>).MakeGenericType(left.GetElementType()).Intersect(right);
		}
		if (right.IsArray)
		{
			return left.Intersect(typeof(List<>).MakeGenericType(right.GetElementType()));
		}
		if (!left.IsGenericType)
		{
			return left;
		}
		if (!right.IsGenericType)
		{
			return left;
		}
		Type genericTypeDefinition = left.GetGenericTypeDefinition();
		Type genericTypeDefinition2 = right.GetGenericTypeDefinition();
		Type[] genericArguments = left.GetGenericArguments();
		if (genericTypeDefinition == genericTypeDefinition2)
		{
			return genericArguments.First().Intersect(right.GenericTypeArguments.First());
		}
		return genericArguments.First().Intersect(right);
	}

	public static void DumpGenericInfo(this Type t)
	{
		Logger.Debug("Info for " + t.PrettyName());
		Logger.Debug($"GP {t.IsGenericParameter} | MP {t.IsGenericMethodParameter} | TP {t.IsGenericTypeParameter} | DEF {t.IsGenericTypeDefinition} | TY {t.IsGenericType} | CON {t.IsConstructedGenericType}");
		if (t.IsGenericParameter)
		{
			Logger.Debug("CONSTRAINTS: " + string.Join(", ", t.GetGenericParameterConstraints().Select(PrettyName)));
		}
		if (!t.IsGenericTypeDefinition && t.IsGenericRelated() && t.IsGenericType)
		{
			t.GetGenericTypeDefinition().DumpGenericInfo();
		}
		Type[] genericArguments = t.GetGenericArguments();
		for (int i = 0; i < genericArguments.Length; i++)
		{
			genericArguments[i].DumpGenericInfo();
		}
	}

	public static bool IsAssignableToGeneric(this Type left, Type right, ToolshedManager toolshed, bool recursiveDescent = true)
	{
		return left.IntersectWithGeneric(right, toolshed, recursiveDescent) != null;
	}

	public static Type[]? IntersectWithGeneric(this Type left, Type right, ToolshedManager toolshed, bool recursiveDescent)
	{
		if (left.IsAssignableTo(right))
		{
			return new Type[1] { left };
		}
		if (right.IsInterface && !left.IsInterface)
		{
			Type[] interfaces = left.GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (!(right.GetMostGenericPossible() != type.GetMostGenericPossible()))
				{
					Type[] array = right.IntersectWithGeneric(type, toolshed, recursiveDescent);
					if (array != null)
					{
						return array;
					}
				}
			}
		}
		if (left.Constructable() && right.IsGenericParameter)
		{
			return new Type[1] { left };
		}
		if (left.IsGenericType && right.IsGenericType && left.GenericTypeArguments.Length == right.GenericTypeArguments.Length && left.GetGenericTypeDefinition() == right.GetGenericTypeDefinition())
		{
			Type[] array2 = null;
			{
				foreach (var item3 in left.GenericTypeArguments.Zip(right.GenericTypeArguments))
				{
					Type item = item3.First;
					Type item2 = item3.Second;
					Type[] array3 = item.IntersectWithGeneric(item2, toolshed, recursiveDescent: false);
					if (array3 != null)
					{
						Type[] interfaces = array2 ?? Array.Empty<Type>();
						Type[] array4 = array3;
						int i = 0;
						Type[] array5 = new Type[interfaces.Length + array4.Length];
						ReadOnlySpan<Type> readOnlySpan = new ReadOnlySpan<Type>(interfaces);
						readOnlySpan.CopyTo(new Span<Type>(array5).Slice(i, readOnlySpan.Length));
						i += readOnlySpan.Length;
						ReadOnlySpan<Type> readOnlySpan2 = new ReadOnlySpan<Type>(array4);
						readOnlySpan2.CopyTo(new Span<Type>(array5).Slice(i, readOnlySpan2.Length));
						i += readOnlySpan2.Length;
						array2 = array5;
					}
				}
				return array2;
			}
		}
		if (recursiveDescent)
		{
			foreach (Type item4 in toolshed.AllSteppedTypes(left))
			{
				Type[] array6 = item4.IntersectWithGeneric(right, toolshed, recursiveDescent: false);
				if (array6 != null)
				{
					return array6;
				}
			}
		}
		return null;
	}

	public static bool IsGenericRelated(this Type t)
	{
		return t.IsGenericParameter | t.IsGenericType | t.IsGenericMethodParameter | t.IsGenericTypeDefinition | t.IsConstructedGenericType | t.IsGenericTypeParameter;
	}

	public static bool Constructable(this Type t)
	{
		if (!t.IsGenericRelated())
		{
			return true;
		}
		if (!t.IsGenericType || !t.IsConstructedGenericType)
		{
			return false;
		}
		bool flag = true;
		Type[] genericArguments = t.GetGenericArguments();
		foreach (Type t2 in genericArguments)
		{
			flag &= t2.Constructable();
		}
		return flag;
	}

	public static PropertyInfo? FindIndexerProperty(this Type type)
	{
		DefaultMemberAttribute defaultPropertyAttribute = type.GetCustomAttributes<DefaultMemberAttribute>().FirstOrDefault();
		if (defaultPropertyAttribute != null)
		{
			return type.GetRuntimeProperties().FirstOrDefault(delegate(PropertyInfo pi)
			{
				if (pi.Name == defaultPropertyAttribute.MemberName && pi.IsIndexerProperty())
				{
					ParameterInfo[] array = pi.SetMethod?.GetParameters();
					if (array != null && array.Length == 2)
					{
						return array[0].ParameterType == typeof(string);
					}
				}
				return false;
			});
		}
		return null;
	}

	public static bool IsIndexerProperty(this PropertyInfo propertyInfo)
	{
		ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
		if (indexParameters.Length == 1)
		{
			return indexParameters[0].ParameterType == typeof(string);
		}
		return false;
	}

	public static Type GetMostGenericPossible(this Type t)
	{
		if (!t.IsGenericType)
		{
			return t;
		}
		return t.GetGenericTypeDefinition();
	}
}
