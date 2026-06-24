using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components;

[Serializable]
[NetSerializable]
public enum MicrowaveVisualState
{
	Idle,
	Cooking,
	Broken,
	Bloody
}
