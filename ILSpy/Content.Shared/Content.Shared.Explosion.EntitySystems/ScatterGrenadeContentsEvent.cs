using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Explosion.EntitySystems;

[ByRefEvent]
public record struct ScatterGrenadeContentsEvent(int TotalCount, int ThrownCount, Angle Angle, bool Handled = false);
