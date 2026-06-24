using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public abstract class CancellableEntityEventArgs : EntityEventArgs
{
	public bool Cancelled { get; private set; }

	public void Cancel()
	{
		Cancelled = true;
	}

	public void Uncancel()
	{
		Cancelled = false;
	}
}
