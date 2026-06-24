using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Barricade.Components;

[Serializable]
[NetSerializable]
public enum BarbedWireVisuals : byte
{
	UnWired,
	WiredClosed,
	WiredOpen
}
