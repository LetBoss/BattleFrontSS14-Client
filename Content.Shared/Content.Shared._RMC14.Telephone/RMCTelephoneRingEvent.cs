using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Telephone;

[ByRefEvent]
public readonly record struct RMCTelephoneRingEvent(EntityUid Receiving, EntityUid Calling, EntityUid Actor);
