using System;
using System.Linq;
using System.Reflection;

namespace Robust.Shared.ViewVariables;

internal sealed class ViewVariablesMethodPath : ViewVariablesPath
{
	private readonly object? _object;

	private readonly MethodInfo _method;

	private readonly VVAccess? _access;

	public override Type Type => typeof(void);

	public override Type InvokeReturnType => _method.ReturnType;

	public override Type[] InvokeParameterTypes
	{
		get
		{
			if (_access != VVAccess.ReadWrite)
			{
				return Array.Empty<Type>();
			}
			return (from info in _method.GetParameters()
				select info.ParameterType).ToArray();
		}
	}

	public override uint InvokeOptionalParameters
	{
		get
		{
			if (_access != VVAccess.ReadWrite)
			{
				return 0u;
			}
			return (uint)_method.GetParameters().Count((ParameterInfo info) => info.IsOptional);
		}
	}

	internal ViewVariablesMethodPath(object? obj, MethodInfo method)
	{
		_object = obj;
		_method = method;
		ViewVariablesUtility.TryGetViewVariablesAccess(method, out _access);
	}

	public override object? Get()
	{
		return null;
	}

	public override void Set(object? value)
	{
	}

	public override object? Invoke(object?[]? parameters)
	{
		if (_access != VVAccess.ReadWrite)
		{
			return null;
		}
		if (_object == null)
		{
			return null;
		}
		return _method.Invoke(_object, parameters);
	}
}
