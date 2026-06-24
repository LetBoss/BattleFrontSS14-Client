using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Bed.Cryostorage;

[Serializable]
[NetSerializable]
public record struct CryostorageContainedPlayerData()
{
	public string PlayerName = string.Empty;

	public NetEntity PlayerEnt = NetEntity.Invalid;

	public Dictionary<string, string> ItemSlots = new Dictionary<string, string>();

	public Dictionary<string, string> HeldItems = new Dictionary<string, string>();
}
