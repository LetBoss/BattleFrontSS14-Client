using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics;

[ByRefEvent]
public record struct PhysicsSleepEvent(EntityUid Entity, PhysicsComponent Body);
