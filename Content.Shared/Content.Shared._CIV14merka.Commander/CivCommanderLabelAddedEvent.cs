using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLabelAddedEvent : EntityEventArgs
{
	public CivCommanderLabelState Label { get; }

	public CivCommanderLabelAddedEvent(CivCommanderLabelState label)
	{
		Label = label;
	}
}
