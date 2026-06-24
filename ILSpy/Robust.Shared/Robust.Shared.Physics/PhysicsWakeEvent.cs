using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Components;

namespace Robust.Shared.Physics;

[ByRefEvent]
public readonly record struct PhysicsWakeEvent(EntityUid Entity, PhysicsComponent Body);
