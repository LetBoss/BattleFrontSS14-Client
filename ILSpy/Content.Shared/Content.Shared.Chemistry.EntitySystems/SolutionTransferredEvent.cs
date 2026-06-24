using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.EntitySystems;

[ByRefEvent]
public record struct SolutionTransferredEvent(EntityUid From, EntityUid To, EntityUid? User, FixedPoint2 Amount);
