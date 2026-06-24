// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.ReflectionExtensions
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Exceptions;
using Robust.Shared.Log;
using Robust.Shared.Toolshed.Syntax;
using Robust.Shared.Toolshed.TypeParsers;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Toolshed;

internal static class ReflectionExtensions
{
  public static bool CanBeNull(this Type t)
  {
    return !t.IsValueType || t.IsGenericType(typeof (Nullable<>));
  }

  public static bool CanBeEmpty(this Type t)
  {
    return t.CanBeNull() || t.IsGenericType(typeof (IEnumerable<>));
  }

  public static bool IsGenericType(this Type t, Type genericType)
  {
    return t.IsGenericType && t.GetGenericTypeDefinition() == genericType;
  }

  public static IEnumerable<Type> GetVariants(this Type t, ToolshedManager toolshed)
  {
    Type[] args = t.GetGenericArguments();
    Type generic = t.GetGenericTypeDefinition();
    Type[] genericArgs = generic.GetGenericArguments();
    int variantCount = ((IEnumerable<Type>) genericArgs).Count<Type>((Func<Type, bool>) (x => (x.GenericParameterAttributes & GenericParameterAttributes.VarianceMask) != 0));
    if (variantCount > 1)
      throw new NotImplementedException("I swear to god I am NOT supporting more than one variant type parameter. Absolutely no combinatorial explosions in this house, factorials can go home.");
    yield return t;
    if (variantCount >= 1)
    {
      int variant = 0;
      for (int index = 0; index < args.Length; ++index)
      {
        if ((genericArgs[index].GenericParameterAttributes & GenericParameterAttributes.VarianceMask) != GenericParameterAttributes.None)
        {
          variant = index;
          break;
        }
      }
      Type[] newArgs = (Type[]) args.Clone();
      foreach (Type allSteppedType in toolshed.AllSteppedTypes(args[variant], false))
      {
        newArgs[variant] = allSteppedType;
        yield return generic.MakeGenericType(newArgs);
      }
    }
  }

  public static Type StepDownConstraints(this Type t)
  {
    if (!t.IsGenericType || t.IsGenericTypeDefinition)
      return t;
    Type[] genericTypeArguments = t.GenericTypeArguments;
    Type[] typeArray = new Type[genericTypeArguments.Length];
    for (int index = 0; index < genericTypeArguments.Length; ++index)
      typeArray[index] = !genericTypeArguments[index].IsGenericType ? genericTypeArguments[index] : genericTypeArguments[index].GetGenericTypeDefinition();
    return t.GetGenericTypeDefinition().MakeGenericType(typeArray);
  }

  public static bool HasGenericParent(this Type type, Type parent)
  {
    for (Type t = type; t != (Type) null; t = t.BaseType)
    {
      if (t.IsGenericType(parent))
        return true;
    }
    return false;
  }

  public static bool IsValueRef(this Type type) => type.HasGenericParent(typeof (ValueRef<>));

  public static bool IsCustomParser(this Type type)
  {
    return type.HasGenericParent(typeof (CustomTypeParser<>));
  }

  public static bool IsParser(this Type type) => type.HasGenericParent(typeof (TypeParser<>));

  public static bool IsCommandArgument(this ParameterInfo param)
  {
    if (param.HasCustomAttribute<CommandArgumentAttribute>())
      return true;
    return !param.HasCustomAttribute<CommandInvertedAttribute>() && !param.HasCustomAttribute<PipedArgumentAttribute>() && !param.HasCustomAttribute<CommandInvocationContextAttribute>() && param.ParameterType != typeof (IInvocationContext);
  }

  public static string PrettyName(this Type type)
  {
    string str = type.Name;
    if (type.IsGenericParameter)
      return type.ToString();
    if ((object) type.DeclaringType != null)
      str = $"{type.DeclaringType.PrettyName()}.{type.Name}";
    if (type.GetGenericArguments().Length == 0)
      return str;
    if (!str.Contains('`'))
      return str + "<>";
    Type[] genericArguments = type.GetGenericArguments();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return $"{str.Substring(0, str.IndexOf('`', StringComparison.InvariantCulture))}<{string.Join(",", ((IEnumerable<Type>) genericArguments).Select<Type, string>(ReflectionExtensions.\u003C\u003EO.\u003C0\u003E__PrettyName ?? (ReflectionExtensions.\u003C\u003EO.\u003C0\u003E__PrettyName = new Func<Type, string>(ReflectionExtensions.PrettyName))))}>";
  }

  public static ParameterInfo? ConsoleGetPipedArgument(this MethodInfo method)
  {
    return ((IEnumerable<ParameterInfo>) method.GetParameters()).SingleOrDefault<ParameterInfo>((Func<ParameterInfo, bool>) (x => x.HasCustomAttribute<PipedArgumentAttribute>()));
  }

  public static bool ConsoleHasInvertedArgument(this MethodInfo method)
  {
    return ((IEnumerable<ParameterInfo>) method.GetParameters()).Any<ParameterInfo>((Func<ParameterInfo, bool>) (x => x.HasCustomAttribute<CommandInvertedAttribute>()));
  }

  public static Expression CreateEmptyExpr(this Type t)
  {
    if (!t.CanBeEmpty())
      throw new TypeArgumentException();
    if (t.IsGenericType(typeof (IEnumerable<>)))
      return (Expression) Expression.Constant((object) Array.CreateInstance(((IEnumerable<Type>) t.GetGenericArguments()).First<Type>(), 0), t);
    if (!t.CanBeNull())
      throw new NotImplementedException();
    return (object) Nullable.GetUnderlyingType(t) != null ? (Expression) Expression.Constant(t.GetConstructor(BindingFlags.CreateInstance, Array.Empty<Type>()).Invoke((object) null, (object[]) null), t) : (Expression) Expression.Constant((object) null, t);
  }

  public static Type Intersect(this Type left, Type right)
  {
    if (left.IsArray)
      return typeof (List<>).MakeGenericType(left.GetElementType()).Intersect(right);
    if (right.IsArray)
      return left.Intersect(typeof (List<>).MakeGenericType(right.GetElementType()));
    if (!left.IsGenericType || !right.IsGenericType)
      return left;
    Type genericTypeDefinition1 = left.GetGenericTypeDefinition();
    Type genericTypeDefinition2 = right.GetGenericTypeDefinition();
    Type[] genericArguments = left.GetGenericArguments();
    Type type = genericTypeDefinition2;
    return genericTypeDefinition1 == type ? ((IEnumerable<Type>) genericArguments).First<Type>().Intersect(((IEnumerable<Type>) right.GenericTypeArguments).First<Type>()) : ((IEnumerable<Type>) genericArguments).First<Type>().Intersect(right);
  }

  public static void DumpGenericInfo(this Type t)
  {
    Logger.Debug("Info for " + t.PrettyName());
    Logger.Debug($"GP {t.IsGenericParameter} | MP {t.IsGenericMethodParameter} | TP {t.IsGenericTypeParameter} | DEF {t.IsGenericTypeDefinition} | TY {t.IsGenericType} | CON {t.IsConstructedGenericType}");
    if (t.IsGenericParameter)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      Logger.Debug("CONSTRAINTS: " + string.Join(", ", ((IEnumerable<Type>) t.GetGenericParameterConstraints()).Select<Type, string>(ReflectionExtensions.\u003C\u003EO.\u003C0\u003E__PrettyName ?? (ReflectionExtensions.\u003C\u003EO.\u003C0\u003E__PrettyName = new Func<Type, string>(ReflectionExtensions.PrettyName)))));
    }
    if (!t.IsGenericTypeDefinition && t.IsGenericRelated() && t.IsGenericType)
      t.GetGenericTypeDefinition().DumpGenericInfo();
    foreach (Type genericArgument in t.GetGenericArguments())
      genericArgument.DumpGenericInfo();
  }

  public static bool IsAssignableToGeneric(
    this Type left,
    Type right,
    ToolshedManager toolshed,
    bool recursiveDescent = true)
  {
    return left.IntersectWithGeneric(right, toolshed, recursiveDescent) != null;
  }

  public static Type[]? IntersectWithGeneric(
    this Type left,
    Type right,
    ToolshedManager toolshed,
    bool recursiveDescent)
  {
    if (left.IsAssignableTo(right))
      return new Type[1]{ left };
    if (right.IsInterface && !left.IsInterface)
    {
      foreach (Type type in left.GetInterfaces())
      {
        if (!(right.GetMostGenericPossible() != type.GetMostGenericPossible()))
        {
          Type[] typeArray = right.IntersectWithGeneric(type, toolshed, recursiveDescent);
          if (typeArray != null)
            return typeArray;
        }
      }
    }
    if (left.Constructable() && right.IsGenericParameter)
      return new Type[1]{ left };
    if (left.IsGenericType && right.IsGenericType && left.GenericTypeArguments.Length == right.GenericTypeArguments.Length && left.GetGenericTypeDefinition() == right.GetGenericTypeDefinition())
    {
      Type[] typeArray1 = (Type[]) null;
      foreach ((Type First, Type Second) tuple in ((IEnumerable<Type>) left.GenericTypeArguments).Zip<Type, Type>((IEnumerable<Type>) right.GenericTypeArguments))
      {
        Type[] typeArray2 = tuple.First.IntersectWithGeneric(tuple.Second, toolshed, false);
        if (typeArray2 != null)
        {
          Type[] array1 = typeArray1 ?? Array.Empty<Type>();
          Type[] array2 = typeArray2;
          int start1 = 0;
          Type[] array3 = new Type[array1.Length + array2.Length];
          ReadOnlySpan<Type> readOnlySpan1 = new ReadOnlySpan<Type>(array1);
          readOnlySpan1.CopyTo(new Span<Type>(array3).Slice(start1, readOnlySpan1.Length));
          int start2 = start1 + readOnlySpan1.Length;
          ReadOnlySpan<Type> readOnlySpan2 = new ReadOnlySpan<Type>(array2);
          readOnlySpan2.CopyTo(new Span<Type>(array3).Slice(start2, readOnlySpan2.Length));
          int num = start2 + readOnlySpan2.Length;
          typeArray1 = array3;
        }
      }
      return typeArray1;
    }
    if (recursiveDescent)
    {
      foreach (Type allSteppedType in toolshed.AllSteppedTypes(left))
      {
        Type[] typeArray = allSteppedType.IntersectWithGeneric(right, toolshed, false);
        if (typeArray != null)
          return typeArray;
      }
    }
    return (Type[]) null;
  }

  public static bool IsGenericRelated(this Type t)
  {
    return t.IsGenericParameter | t.IsGenericType | t.IsGenericMethodParameter | t.IsGenericTypeDefinition | t.IsConstructedGenericType | t.IsGenericTypeParameter;
  }

  public static bool Constructable(this Type t)
  {
    if (!t.IsGenericRelated())
      return true;
    if (!t.IsGenericType || !t.IsConstructedGenericType)
      return false;
    bool flag = true;
    foreach (Type genericArgument in t.GetGenericArguments())
      flag &= genericArgument.Constructable();
    return flag;
  }

  public static PropertyInfo? FindIndexerProperty(this Type type)
  {
    DefaultMemberAttribute defaultPropertyAttribute = type.GetCustomAttributes<DefaultMemberAttribute>().FirstOrDefault<DefaultMemberAttribute>();
    return defaultPropertyAttribute != null ? type.GetRuntimeProperties().FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (pi =>
    {
      if (pi.Name == defaultPropertyAttribute.MemberName && pi.IsIndexerProperty())
      {
        ParameterInfo[] parameters = pi.SetMethod?.GetParameters();
        if (parameters != null && parameters.Length == 2)
          return parameters[0].ParameterType == typeof (string);
      }
      return false;
    })) : (PropertyInfo) null;
  }

  public static bool IsIndexerProperty(this PropertyInfo propertyInfo)
  {
    ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
    return indexParameters.Length == 1 && indexParameters[0].ParameterType == typeof (string);
  }

  public static Type GetMostGenericPossible(this Type t)
  {
    return !t.IsGenericType ? t : t.GetGenericTypeDefinition();
  }
}
