using System;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterSelectClassRequestEvent : EntityEventArgs
{
	public CivTdmClass Class { get; }

	public CivRosterSelectClassRequestEvent(CivTdmClass selectedClass)
	{
		Class = selectedClass;
	}
}
