using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events;

[Serializable]
[NetSerializable]
public sealed class IFFShowIFFMessage : BoundUserInterfaceMessage
{
	public bool Show;
}
