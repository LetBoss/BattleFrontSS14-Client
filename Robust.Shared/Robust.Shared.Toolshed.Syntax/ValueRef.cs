using System;
using System.Collections.Generic;

namespace Robust.Shared.Toolshed.Syntax;

public abstract class ValueRef<T>
{
	public abstract T? Evaluate(IInvocationContext ctx);

	internal static T? EvaluateParameter(object? obj, IInvocationContext ctx)
	{
		if (obj != null)
		{
			if (!(obj is T result))
			{
				if (obj is ValueRef<T> valueRef)
				{
					return valueRef.Evaluate(ctx);
				}
				throw new Exception($"Failed to parse command parameter. This likely is a toolshed bug and should be reported.\nTarget type: {typeof(T).PrettyName()}.\nInput type: {obj.GetType()}.\nInput: {obj}");
			}
			return result;
		}
		return default(T);
	}

	internal static T[] EvaluateParamsCollection(object? obj, IInvocationContext ctx)
	{
		List<object> obj2 = (obj as List<object>) ?? throw new Exception("Failed to parse command parameter. This likely is a toolshed bug and should be reported.");
		int num = 0;
		T[] array = new T[obj2.Count];
		foreach (object item in obj2)
		{
			array[num++] = EvaluateParameter(item, ctx);
		}
		return array;
	}
}
[Obsolete("Use EntProtoId / ProtoId<T>")]
public sealed class ValueRef<T, TAuto>(ValueRef<T> inner) : ValueRef<T>
{
	public override T? Evaluate(IInvocationContext ctx)
	{
		return inner.Evaluate(ctx);
	}
}
