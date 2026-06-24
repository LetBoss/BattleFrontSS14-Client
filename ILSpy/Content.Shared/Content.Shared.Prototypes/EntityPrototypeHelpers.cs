using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Prototypes;

public static class EntityPrototypeHelpers
{
	public static bool HasComponent<T>(this EntityPrototype prototype, IComponentFactory? componentFactory = null) where T : IComponent
	{
		return prototype.HasComponent(typeof(T), componentFactory);
	}

	public static bool HasComponent(this EntityPrototype prototype, Type component, IComponentFactory? componentFactory = null)
	{
		if (componentFactory == null)
		{
			componentFactory = IoCManager.Resolve<IComponentFactory>();
		}
		ComponentRegistration registration = componentFactory.GetRegistration(component);
		return ((Dictionary<string, ComponentRegistryEntry>)(object)prototype.Components).ContainsKey(registration.Name);
	}

	public static bool HasComponent<T>(string prototype, IPrototypeManager? prototypeManager = null, IComponentFactory? componentFactory = null) where T : IComponent
	{
		return HasComponent(prototype, typeof(T), prototypeManager, componentFactory);
	}

	public static bool HasComponent(string prototype, Type component, IPrototypeManager? prototypeManager = null, IComponentFactory? componentFactory = null)
	{
		if (prototypeManager == null)
		{
			prototypeManager = IoCManager.Resolve<IPrototypeManager>();
		}
		EntityPrototype proto = default(EntityPrototype);
		if (prototypeManager.TryIndex<EntityPrototype>(prototype, ref proto))
		{
			return proto.HasComponent(component, componentFactory);
		}
		return false;
	}
}
