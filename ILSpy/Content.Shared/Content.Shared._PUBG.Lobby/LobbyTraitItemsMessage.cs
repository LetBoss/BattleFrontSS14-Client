using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Lobby;

[Serializable]
[NetSerializable]
public sealed class LobbyTraitItemsMessage : EntityEventArgs
{
	public List<string> UnlockedTraitItems { get; }

	public LobbyTraitItemsMessage(List<string> unlockedTraitItems)
	{
		UnlockedTraitItems = unlockedTraitItems;
	}
}
