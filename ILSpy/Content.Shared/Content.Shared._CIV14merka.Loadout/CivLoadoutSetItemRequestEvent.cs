using System;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Loadout;

[Serializable]
[NetSerializable]
public sealed class CivLoadoutSetItemRequestEvent : EntityEventArgs
{
	public readonly string Faction;

	public readonly CivTdmClass Class;

	public readonly string ItemKey;

	public readonly bool Disabled;

	public CivLoadoutSetItemRequestEvent(string faction, CivTdmClass cls, string itemKey, bool disabled)
	{
		Faction = faction;
		Class = cls;
		ItemKey = itemKey;
		Disabled = disabled;
	}
}
