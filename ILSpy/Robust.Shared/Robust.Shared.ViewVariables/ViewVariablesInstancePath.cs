using System;

namespace Robust.Shared.ViewVariables;

public sealed class ViewVariablesInstancePath : ViewVariablesPath
{
	private readonly object? _object;

	public override Type Type => _object?.GetType() ?? typeof(void);

	public ViewVariablesInstancePath(object? obj)
	{
		_object = obj;
	}

	public override object? Get()
	{
		return _object;
	}

	public override void Set(object? value)
	{
	}

	public override object? Invoke(object?[]? parameters)
	{
		return null;
	}
}
