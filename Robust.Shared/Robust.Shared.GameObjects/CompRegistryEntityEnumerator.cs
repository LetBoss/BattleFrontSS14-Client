using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;

namespace Robust.Shared.GameObjects;

public struct CompRegistryEntityEnumerator(IEntityManager entManager, Dictionary<EntityUid, IComponent> traitDict, ComponentRegistry registry) : IDisposable
{
	private IEntityManager _entManager = entManager;

	private Dictionary<EntityUid, IComponent>.Enumerator _traitDict = traitDict.GetEnumerator();

	private ComponentRegistry _registry = registry;

	public bool MoveNext(out EntityUid uid)
	{
		KeyValuePair<EntityUid, IComponent> current;
		while (true)
		{
			if (!_traitDict.MoveNext())
			{
				uid = default(EntityUid);
				return false;
			}
			current = _traitDict.Current;
			if (current.Value.Deleted)
			{
				continue;
			}
			int num = -1;
			bool flag = true;
			foreach (KeyValuePair<string, EntityPrototype.ComponentRegistryEntry> item in _registry)
			{
				num++;
				if (num != 0 && (!_entManager.TryGetComponent(current.Key, item.Value.Component.GetType(), out IComponent component) || component.Deleted))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		uid = current.Key;
		return true;
	}

	public void Dispose()
	{
		_traitDict.Dispose();
	}
}
