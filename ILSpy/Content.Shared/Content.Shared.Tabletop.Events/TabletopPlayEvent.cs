using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events;

[Serializable]
[NetSerializable]
public sealed class TabletopPlayEvent : EntityEventArgs
{
	public NetEntity TableUid;

	public NetEntity CameraUid;

	public string Title;

	public Vector2i Size;

	public TabletopPlayEvent(NetEntity tableUid, NetEntity cameraUid, string title, Vector2i size)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		TableUid = tableUid;
		CameraUid = cameraUid;
		Title = title;
		Size = size;
	}
}
