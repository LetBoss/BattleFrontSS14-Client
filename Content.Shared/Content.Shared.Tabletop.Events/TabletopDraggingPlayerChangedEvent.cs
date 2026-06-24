using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events;

[Serializable]
[NetSerializable]
public sealed class TabletopDraggingPlayerChangedEvent : EntityEventArgs
{
	public NetEntity DraggedEntityUid;

	public bool IsDragging;

	public TabletopDraggingPlayerChangedEvent(NetEntity draggedEntityUid, bool isDragging)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		DraggedEntityUid = draggedEntityUid;
		IsDragging = isDragging;
	}
}
