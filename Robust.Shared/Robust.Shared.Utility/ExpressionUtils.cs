using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Robust.Shared.Utility;

public static class ExpressionUtils
{
	public static MethodCallExpression ToStringExpression(Expression expression)
	{
		return Expression.Call(Expression.Convert(expression, typeof(object)), "ToString", Type.EmptyTypes);
	}

	public static Expression WriteLine(object value)
	{
		if (value is Expression expression)
		{
			if (expression.Type != typeof(string))
			{
				value = ToStringExpression(expression);
			}
		}
		else if (!(value is string))
		{
			throw new InvalidOperationException();
		}
		return Expression.Call(typeof(System.Console).GetMethod("WriteLine", new Type[1] { typeof(string) }), ExpressionOrConstant(value));
	}

	public static Expression[] WriteLineBefore(object value, Expression expression)
	{
		return new Expression[2]
		{
			WriteLine(value),
			expression
		};
	}

	public static Expression[] WriteLineAfter(object value, Expression expression)
	{
		return new Expression[2]
		{
			expression,
			WriteLine(value)
		};
	}

	public static BlockExpression ToBlock(this Expression[] arr)
	{
		return Expression.Block(arr);
	}

	public static NewExpression NewExpression<T>(params object[] parameters)
	{
		return NewExpression(typeof(T), parameters);
	}

	public static NewExpression NewExpression(Type type, params object[] parameters)
	{
		return Expression.New(type.GetConstructor(parameters.Select(ExpressionTypeOrType).ToArray()), parameters.Select(ExpressionOrConstant));
	}

	public static UnaryExpression ThrowExpression<T>(params object[] args) where T : Exception
	{
		return Expression.Throw(Expression.New(typeof(T).GetConstructor(args.Select(ExpressionTypeOrType).ToArray()), args.Select(ExpressionOrConstant)));
	}

	public static Type ExpressionTypeOrType(object x)
	{
		if (!(x is Expression expression))
		{
			return x.GetType();
		}
		return expression.Type;
	}

	public static Expression ExpressionOrConstant(object x)
	{
		if (!(x is Expression result))
		{
			return Expression.Constant(x);
		}
		return result;
	}

	public static MethodCallExpression GetTypeExpression(Expression obj)
	{
		return Expression.Call(obj, "GetType", Type.EmptyTypes);
	}

	public static Expression DefaultValueOrTypeDefault(ParameterInfo x)
	{
		if (!x.HasDefaultValue)
		{
			return Expression.Default(x.ParameterType);
		}
		return Expression.Constant(x.DefaultValue, x.ParameterType);
	}

	public static Expression EqualExpression(Expression left, Expression right)
	{
		if (left.Type != right.Type)
		{
			throw new InvalidOperationException($"Left & Right Expression Types dont match ({left.Type}, {right.Type})");
		}
		if (left.Type.IsPrimitive || left.Type == typeof(string))
		{
			return Expression.Equal(left, right);
		}
		Type type = typeof(EqualityComparer<>).MakeGenericType(left.Type);
		return Expression.Call(Expression.Constant(type.GetProperty("Default").GetMethod.Invoke(null, null), type), "Equals", Type.EmptyTypes, left, right);
	}
}
