using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Robust.Shared.GameObjects;

internal sealed class PrototypeReloadSystem : EntitySystem
{
	[Dependency]
	private readonly IPrototypeManager _prototypes;

	[Dependency]
	private readonly IComponentFactory _componentFactory;

	public override void Initialize()
	{
		SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);
	}

	private void OnPrototypesReloaded(PrototypesReloadedEventArgs eventArgs)
	{
		if (!eventArgs.ByType.TryGetValue(typeof(EntityPrototype), out PrototypesReloadedEventArgs.PrototypeChangeSet value))
		{
			return;
		}
		AllEntityQueryEnumerator<MetaDataComponent> allEntityQueryEnumerator = AllEntityQuery<MetaDataComponent>();
		EntityUid uid;
		MetaDataComponent comp;
		while (allEntityQueryEnumerator.MoveNext(out uid, out comp))
		{
			string text = comp.EntityPrototype?.ID;
			if (text != null && value.Modified.ContainsKey(text))
			{
				EntityPrototype newPrototype = _prototypes.Index<EntityPrototype>(text);
				UpdateEntity(uid, comp, newPrototype);
			}
		}
	}

	private void UpdateEntity(EntityUid entity, MetaDataComponent metaData, EntityPrototype newPrototype)
	{
		List<(string, Type)> list = (from name in metaData.EntityPrototype?.Components.Keys
			where name != "Transform" && name != "MetaData"
			select (name: name, Type: _componentFactory.GetRegistration(name).Type)).ToList() ?? new List<(string, Type)>();
		List<(string, Type)> list2 = (from name in newPrototype.Components.Keys
			where name != "Transform" && name != "MetaData"
			select (name: name, Type: _componentFactory.GetRegistration(name).Type)).ToList();
		List<string> ignoredComponents = new List<string>();
		foreach (var (text, type) in list.Except(list2))
		{
			if (newPrototype.Components.ContainsKey(text))
			{
				ignoredComponents.Add(text);
			}
			else
			{
				RemComp(entity, type);
			}
		}
		EntityManager.CullRemovedComponents();
		foreach (var item2 in list2.Where<(string, Type)>(((string name, Type Type) t) => !ignoredComponents.Contains(t.name)).Except(list))
		{
			string item = item2.Item1;
			_ = newPrototype.Components[item];
			IComponent component = _componentFactory.GetComponent(item);
			if (!HasComp(entity, component.GetType()))
			{
				AddComp(entity, component);
			}
		}
		metaData.EntityPrototype = newPrototype;
	}
}
