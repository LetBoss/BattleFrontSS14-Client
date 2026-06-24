using System;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Ghost;

[Serializable]
[NetSerializable]
public sealed class CivGhostChangeClassRequestEvent : EntityEventArgs
{
	public CivTdmClass Class { get; }

	public CivGhostChangeClassRequestEvent(CivTdmClass selectedClass)
	{
		Class = selectedClass;
	}
}
