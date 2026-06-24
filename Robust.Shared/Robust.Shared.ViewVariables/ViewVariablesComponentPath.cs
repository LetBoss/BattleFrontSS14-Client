using System;
using Robust.Shared.GameObjects;

namespace Robust.Shared.ViewVariables;

public sealed class ViewVariablesComponentPath : ViewVariablesPath
{
	public readonly IComponent Component;

	public readonly EntityUid Owner;

	public override Type Type => Component?.GetType() ?? typeof(void);

	public ViewVariablesComponentPath(IComponent component, EntityUid owner)
	{
		Component = component;
		Owner = owner;
	}

	public override object? Get()
	{
		return Component;
	}

	public override void Set(object? value)
	{
	}

	public override object? Invoke(object?[]? parameters)
	{
		return null;
	}
}
