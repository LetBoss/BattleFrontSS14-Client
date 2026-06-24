using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Mobs;

public record struct MobStateChangedEvent(EntityUid Target, MobStateComponent Component, MobState OldMobState, MobState NewMobState, EntityUid? Origin = null);
