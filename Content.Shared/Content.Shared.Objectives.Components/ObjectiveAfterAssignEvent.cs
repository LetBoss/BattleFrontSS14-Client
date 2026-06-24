using Content.Shared.Mind;
using Robust.Shared.GameObjects;

namespace Content.Shared.Objectives.Components;

[ByRefEvent]
public record struct ObjectiveAfterAssignEvent(EntityUid MindId, MindComponent Mind, ObjectiveComponent Objective, MetaDataComponent Meta);
