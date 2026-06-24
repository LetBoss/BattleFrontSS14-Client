using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class BwoinkPlayerTypingUpdated : EntityEventArgs
{
	public NetUserId Channel { get; }

	public string PlayerName { get; }

	public bool Typing { get; }

	public BwoinkPlayerTypingUpdated(NetUserId channel, string playerName, bool typing)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Channel = channel;
		PlayerName = playerName;
		Typing = typing;
	}
}
