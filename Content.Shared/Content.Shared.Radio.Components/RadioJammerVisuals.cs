using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Radio.Components;

[Serializable]
[NetSerializable]
public enum RadioJammerVisuals : byte
{
	ChargeLevel,
	LEDOn
}
