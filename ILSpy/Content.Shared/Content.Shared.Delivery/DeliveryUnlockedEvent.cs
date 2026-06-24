using Robust.Shared.GameObjects;

namespace Content.Shared.Delivery;

[ByRefEvent]
public readonly record struct DeliveryUnlockedEvent(EntityUid User);
