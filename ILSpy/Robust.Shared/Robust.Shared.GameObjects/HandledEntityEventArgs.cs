using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public abstract class HandledEntityEventArgs : EntityEventArgs
{
	public bool Handled { get; set; }
}
