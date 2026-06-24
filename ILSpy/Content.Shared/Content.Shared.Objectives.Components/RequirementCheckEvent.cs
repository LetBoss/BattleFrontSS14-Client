using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.Shared.Objectives.Components;

[ByRefEvent]
public record struct RequirementCheckEvent(EntityUid MindId, MindComponent Mind, bool Cancelled = false);
