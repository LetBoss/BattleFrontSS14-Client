using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Events;

[Serializable]
[NetSerializable]
public sealed class PanicBunkerChangedEvent : EntityEventArgs
{
	public PanicBunkerStatus Status;

	public PanicBunkerChangedEvent(PanicBunkerStatus status)
	{
		Status = status;
	}
}
