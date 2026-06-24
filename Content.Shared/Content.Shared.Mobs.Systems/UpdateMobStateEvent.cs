using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mobs.Systems;

[ByRefEvent]
public record struct UpdateMobStateEvent(EntityUid Target, MobStateComponent Component, MobState State, EntityUid? Origin = null);
