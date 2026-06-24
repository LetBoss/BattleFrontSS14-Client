using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DragDrop;

[Serializable]
[NetSerializable]
public sealed class DragDropRequestEvent : EntityEventArgs
{
	public NetEntity Dragged { get; }

	public NetEntity Target { get; }

	public DragDropRequestEvent(NetEntity dragged, NetEntity target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Dragged = dragged;
		Target = target;
	}
}
