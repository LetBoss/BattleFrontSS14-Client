using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class BwoinkClientTypingUpdated : EntityEventArgs
{
	public NetUserId Channel { get; }

	public bool Typing { get; }

	public BwoinkClientTypingUpdated(NetUserId channel, bool typing)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Channel = channel;
		Typing = typing;
	}
}
