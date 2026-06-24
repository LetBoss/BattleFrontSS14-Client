using Robust.Shared.GameObjects;

namespace Content.Shared.Power;

[ByRefEvent]
public readonly record struct PowerChangedEvent(bool Powered, float ReceivingPower);
