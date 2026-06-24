// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ExpressionUtils
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Utility;

public static class ExpressionUtils
{
  public static MethodCallExpression ToStringExpression(Expression expression)
  {
    return Expression.Call((Expression) Expression.Convert(expression, typeof (object)), "ToString", Type.EmptyTypes);
  }

  public static Expression WriteLine(object value)
  {
    switch (value)
    {
      case Expression expression:
        if (expression.Type != typeof (string))
        {
          value = (object) ExpressionUtils.ToStringExpression(expression);
          goto label_4;
        }
        goto label_4;
      case string _:
label_4:
        return (Expression) Expression.Call(typeof (Console).GetMethod(nameof (WriteLine), new Type[1]
        {
          typeof (string)
        }), ExpressionUtils.ExpressionOrConstant(value));
      default:
        throw new InvalidOperationException();
    }
  }

  public static Expression[] WriteLineBefore(object value, Expression expression)
  {
    return new Expression[2]
    {
      ExpressionUtils.WriteLine(value),
      expression
    };
  }

  public static Expression[] WriteLineAfter(object value, Expression expression)
  {
    return new Expression[2]
    {
      expression,
      ExpressionUtils.WriteLine(value)
    };
  }

  public static BlockExpression ToBlock(this Expression[] arr) => Expression.Block(arr);

  public static System.Linq.Expressions.NewExpression NewExpression<T>(params object[] parameters)
  {
    return ExpressionUtils.NewExpression(typeof (T), parameters);
  }

  public static System.Linq.Expressions.NewExpression NewExpression(
    Type type,
    params object[] parameters)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return Expression.New(type.GetConstructor(((IEnumerable<object>) parameters).Select<object, Type>(ExpressionUtils.\u003C\u003EO.\u003C0\u003E__ExpressionTypeOrType ?? (ExpressionUtils.\u003C\u003EO.\u003C0\u003E__ExpressionTypeOrType = new Func<object, Type>(ExpressionUtils.ExpressionTypeOrType))).ToArray<Type>()), ((IEnumerable<object>) parameters).Select<object, Expression>(ExpressionUtils.\u003C\u003EO.\u003C1\u003E__ExpressionOrConstant ?? (ExpressionUtils.\u003C\u003EO.\u003C1\u003E__ExpressionOrConstant = new Func<object, Expression>(ExpressionUtils.ExpressionOrConstant))));
  }

  public static UnaryExpression ThrowExpression<T>(params object[] args) where T : Exception
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return Expression.Throw((Expression) Expression.New(typeof (T).GetConstructor(((IEnumerable<object>) args).Select<object, Type>(ExpressionUtils.\u003C\u003EO.\u003C0\u003E__ExpressionTypeOrType ?? (ExpressionUtils.\u003C\u003EO.\u003C0\u003E__ExpressionTypeOrType = new Func<object, Type>(ExpressionUtils.ExpressionTypeOrType))).ToArray<Type>()), ((IEnumerable<object>) args).Select<object, Expression>(ExpressionUtils.\u003C\u003EO.\u003C1\u003E__ExpressionOrConstant ?? (ExpressionUtils.\u003C\u003EO.\u003C1\u003E__ExpressionOrConstant = new Func<object, Expression>(ExpressionUtils.ExpressionOrConstant)))));
  }

  public static Type ExpressionTypeOrType(object x)
  {
    return !(x is Expression expression) ? x.GetType() : expression.Type;
  }

  public static Expression ExpressionOrConstant(object x)
  {
    return !(x is Expression expression) ? (Expression) Expression.Constant(x) : expression;
  }

  public static MethodCallExpression GetTypeExpression(Expression obj)
  {
    return Expression.Call(obj, "GetType", Type.EmptyTypes);
  }

  public static Expression DefaultValueOrTypeDefault(ParameterInfo x)
  {
    return !x.HasDefaultValue ? (Expression) Expression.Default(x.ParameterType) : (Expression) Expression.Constant(x.DefaultValue, x.ParameterType);
  }

  public static Expression EqualExpression(Expression left, Expression right)
  {
    if (left.Type != right.Type)
      throw new InvalidOperationException($"Left & Right Expression Types dont match ({left.Type}, {right.Type})");
    if (left.Type.IsPrimitive || left.Type == typeof (string))
      return (Expression) Expression.Equal(left, right);
    Type type = typeof (EqualityComparer<>).MakeGenericType(left.Type);
    return (Expression) Expression.Call((Expression) Expression.Constant(type.GetProperty("Default").GetMethod.Invoke((object) null, (object[]) null), type), "Equals", Type.EmptyTypes, left, right);
  }
}
