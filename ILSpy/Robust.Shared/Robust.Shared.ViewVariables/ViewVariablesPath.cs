using System;
using Robust.Shared.Analyzers;

namespace Robust.Shared.ViewVariables;

[Virtual]
public abstract class ViewVariablesPath
{
	public ViewVariablesComponentPath? ParentComponent;

	public abstract Type Type { get; }

	public virtual Type[] InvokeParameterTypes { get; } = Array.Empty<Type>();

	public virtual uint InvokeOptionalParameters { get; }

	public virtual Type InvokeReturnType { get; } = typeof(void);

	public abstract object? Get();

	public abstract void Set(object? value);

	public abstract object? Invoke(object?[]? parameters);

	public static ViewVariablesFakePath FromObject(object obj)
	{
		return new ViewVariablesFakePath((Func<object?>?)(() => obj), (Action<object?>?)null, (Func<object?, object?>?)null, obj.GetType(), (Type[]?)null, 0u, (Type?)null);
	}

	public static ViewVariablesFakePath FromGetter(Func<object?> getter, Type type)
	{
		return new ViewVariablesFakePath(getter, (Action<object?>?)null, (Func<object?, object?>?)null, type, (Type[]?)null, 0u, (Type?)null);
	}

	public static ViewVariablesFakePath FromSetter(Action<object?> setter, Type type)
	{
		return new ViewVariablesFakePath((Func<object?>?)null, setter, (Func<object?, object?>?)null, type, (Type[]?)null, 0u, (Type?)null);
	}

	public static ViewVariablesFakePath FromInvoker(Func<object?, object?> invoker, Type[]? invokeParameterTypes = null, uint invokeOptionalParameters = 0u, Type? invokeReturnType = null)
	{
		return new ViewVariablesFakePath(null, null, invoker, null, invokeParameterTypes, invokeOptionalParameters, invokeReturnType);
	}

	public static ViewVariablesFakePath FromInvoker(Action<object?> invoker, Type[]? invokeParameterTypes = null, uint invokeOptionalParameters = 0u, Type? invokeReturnType = null)
	{
		return new ViewVariablesFakePath(null, null, invoker, null, invokeParameterTypes, invokeOptionalParameters, invokeReturnType);
	}
}
