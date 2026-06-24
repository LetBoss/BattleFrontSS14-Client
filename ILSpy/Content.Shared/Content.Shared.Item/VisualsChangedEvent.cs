using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Item;

[Serializable]
[NetSerializable]
public sealed class VisualsChangedEvent : EntityEventArgs
{
	public readonly NetEntity Item;

	public readonly string ContainerId;

	public VisualsChangedEvent(NetEntity item, string containerId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Item = item;
		ContainerId = containerId;
	}
}
