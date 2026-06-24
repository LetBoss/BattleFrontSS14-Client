using System;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable]
[NetSerializable]
public sealed class IFFConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
	public IFFFlags AllowedFlags;

	public IFFFlags Flags;
}
