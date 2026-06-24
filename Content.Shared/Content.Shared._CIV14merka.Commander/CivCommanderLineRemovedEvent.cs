using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLineRemovedEvent : EntityEventArgs
{
	public int Id { get; }

	public CivCommanderLineRemovedEvent(int id)
	{
		Id = id;
	}
}
