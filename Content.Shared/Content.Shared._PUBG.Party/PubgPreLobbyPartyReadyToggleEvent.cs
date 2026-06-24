using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyPartyReadyToggleEvent : EntityEventArgs
{
	public bool IsReady { get; }

	public PubgPreLobbyPartyReadyToggleEvent(bool isReady)
	{
		IsReady = isReady;
	}
}
