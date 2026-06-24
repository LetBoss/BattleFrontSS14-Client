using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Maps;

[Serializable]
[NetSerializable]
public sealed class GridDragToggleMessage : EntityEventArgs
{
	public bool Enabled;
}
