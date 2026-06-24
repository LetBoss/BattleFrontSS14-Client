using Robust.Shared.GameObjects;

namespace Content.Shared.Delivery;

[ByRefEvent]
public readonly record struct DeliveryOpenedEvent(EntityUid User);
