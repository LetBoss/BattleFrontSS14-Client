using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Materials;

[ByRefEvent]
public record struct GotReclaimedEvent(EntityCoordinates ReclaimerCoordinates);
