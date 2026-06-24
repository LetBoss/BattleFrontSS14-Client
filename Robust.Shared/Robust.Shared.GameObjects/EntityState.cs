using System;
using System.Collections.Generic;
using NetSerializer;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public sealed class EntityState
{
	public NetEntity NetEntity;

	public readonly GameTick EntityLastModified;

	public HashSet<ushort>? NetComponents;

	public NetListAsArray<ComponentChange> ComponentChanges { get; }

	public bool Empty
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			IReadOnlyCollection<ComponentChange> value = ComponentChanges.Value;
			if ((value == null || value.Count == 0) ? true : false)
			{
				return NetComponents == null;
			}
			return false;
		}
	}

	public EntityState(NetEntity netEntity, NetListAsArray<ComponentChange> changedComponents, GameTick lastModified, HashSet<ushort>? netComps = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		NetEntity = netEntity;
		ComponentChanges = changedComponents;
		EntityLastModified = lastModified;
		NetComponents = netComps;
	}
}
