using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.Shared.Objectives.Components;

[ByRefEvent]
public record struct ObjectiveGetProgressEvent(EntityUid MindId, MindComponent Mind, float? Progress = null);
