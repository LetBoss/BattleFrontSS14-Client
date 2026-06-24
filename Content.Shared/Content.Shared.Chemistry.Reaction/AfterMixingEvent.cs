using Robust.Shared.GameObjects;

namespace Content.Shared.Chemistry.Reaction;

public readonly record struct AfterMixingEvent(EntityUid Mixed, EntityUid Mixer);
