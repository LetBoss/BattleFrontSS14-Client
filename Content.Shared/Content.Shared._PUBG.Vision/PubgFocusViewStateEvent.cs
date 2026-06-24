using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Vision;

[Serializable]
[NetSerializable]
public sealed class PubgFocusViewStateEvent : EntityEventArgs
{
	public readonly NetEntity Target;

	public readonly bool Active;

	public PubgFocusViewStateEvent(NetEntity target, bool active)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
		Active = active;
	}
}
