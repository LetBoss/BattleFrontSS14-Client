using Content.Shared.FixedPoint;

namespace Content.Shared._RMC14.Actions;

public record struct ActionReducedUseDelayEvent(FixedPoint2 Amount);
