using System;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Loadout;

[Serializable]
[NetSerializable]
public sealed class CivLoadoutSetSlotChoiceRequestEvent : EntityEventArgs
{
	public readonly string Faction;

	public readonly CivTdmClass Class;

	public readonly string Slot;

	public readonly string Proto;

	public CivLoadoutSetSlotChoiceRequestEvent(string faction, CivTdmClass cls, string slot, string proto)
	{
		Faction = faction;
		Class = cls;
		Slot = slot;
		Proto = proto;
	}
}
