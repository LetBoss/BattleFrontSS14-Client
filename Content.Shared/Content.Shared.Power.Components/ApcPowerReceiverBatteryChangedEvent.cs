using Robust.Shared.GameObjects;

namespace Content.Shared.Power.Components;

[ByRefEvent]
public readonly record struct ApcPowerReceiverBatteryChangedEvent(bool Enabled);
