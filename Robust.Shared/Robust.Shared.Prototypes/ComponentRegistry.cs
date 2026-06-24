using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Prototypes;

public sealed class ComponentRegistry : Dictionary<string, EntityPrototype.ComponentRegistryEntry>, IEntityLoadContext
{
	public ComponentRegistry()
	{
	}

	public ComponentRegistry(Dictionary<string, EntityPrototype.ComponentRegistryEntry> components)
		: base((IDictionary<string, EntityPrototype.ComponentRegistryEntry>)components)
	{
	}

	public bool TryGetComponent(string componentName, [NotNullWhen(true)] out IComponent? component)
	{
		EntityPrototype.ComponentRegistryEntry value;
		bool result = TryGetValue(componentName, out value);
		component = value?.Component;
		return result;
	}

	public bool TryGetComponent<TComponent>(IComponentFactory componentFactory, [NotNullWhen(true)] out TComponent? component) where TComponent : class, IComponent, new()
	{
		component = null;
		string componentName = componentFactory.GetComponentName<TComponent>();
		if (TryGetComponent(componentName, out IComponent component2))
		{
			component = (TComponent)component2;
			return true;
		}
		return false;
	}

	public IEnumerable<string> GetExtraComponentTypes()
	{
		return base.Keys;
	}

	public bool ShouldSkipComponent(string compName)
	{
		return false;
	}
}
