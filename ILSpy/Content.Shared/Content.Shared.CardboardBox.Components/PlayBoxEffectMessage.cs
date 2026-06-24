using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CardboardBox.Components;

[Serializable]
[NetSerializable]
public sealed class PlayBoxEffectMessage : EntityEventArgs
{
	public NetEntity Source;

	public NetEntity Mover;

	public PlayBoxEffectMessage(NetEntity source, NetEntity mover)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Source = source;
		Mover = mover;
	}
}
