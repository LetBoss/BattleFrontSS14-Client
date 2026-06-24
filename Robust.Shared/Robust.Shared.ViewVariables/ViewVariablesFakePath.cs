using System;

namespace Robust.Shared.ViewVariables;

public sealed class ViewVariablesFakePath : ViewVariablesPath
{
	private readonly Func<object?>? _getter;

	private readonly Action<object?>? _setter;

	private readonly Func<object?, object?>? _invoker;

	public override Type Type { get; }

	public override Type[] InvokeParameterTypes { get; }

	public override uint InvokeOptionalParameters { get; }

	public override Type InvokeReturnType { get; }

	public ViewVariablesFakePath(Func<object?>? getter, Action<object?>? setter, Func<object?, object?>? invoker = null, Type? type = null, Type[]? invokeParameterTypes = null, uint invokeOptionalParameters = 0u, Type? invokeReturnType = null)
	{
		_getter = getter;
		_setter = setter;
		_invoker = invoker;
		Type = type ?? typeof(void);
		InvokeParameterTypes = invokeParameterTypes ?? Array.Empty<Type>();
		InvokeOptionalParameters = invokeOptionalParameters;
		InvokeReturnType = invokeReturnType ?? typeof(void);
	}

	public ViewVariablesFakePath(Func<object?>? getter, Action<object?>? setter, Action<object?> invoker, Type? type = null, Type[]? invokeParameterTypes = null, uint invokeOptionalParameters = 0u, Type? invokeReturnType = null)
		: this(getter, setter, (Func<object?, object?>?)null, type, invokeParameterTypes, invokeOptionalParameters, invokeReturnType)
	{
		_invoker = delegate(object? p)
		{
			invoker(p);
			return (object?)null;
		};
	}

	public override object? Get()
	{
		return _getter?.Invoke();
	}

	public override void Set(object? value)
	{
		_setter?.Invoke(value);
	}

	public override object? Invoke(object?[]? parameters)
	{
		return _invoker?.Invoke(parameters);
	}

	public ViewVariablesFakePath WithGetter(Func<object?> getter, Type? type = null)
	{
		return new ViewVariablesFakePath(getter, _setter, _invoker, type ?? Type, InvokeParameterTypes, InvokeOptionalParameters, InvokeReturnType);
	}

	public ViewVariablesFakePath WithSetter(Action<object?> setter, Type? type = null)
	{
		return new ViewVariablesFakePath(_getter, setter, _invoker, type ?? Type, InvokeParameterTypes, InvokeOptionalParameters, InvokeReturnType);
	}

	public ViewVariablesFakePath WithInvoker(Func<object?, object?> invoker, Type[]? invokeParameterTypes = null, uint invokeOptionalParameters = 0u, Type? invokeReturnType = null)
	{
		return new ViewVariablesFakePath(_getter, _setter, invoker, Type, invokeParameterTypes, invokeOptionalParameters, invokeReturnType);
	}
}
