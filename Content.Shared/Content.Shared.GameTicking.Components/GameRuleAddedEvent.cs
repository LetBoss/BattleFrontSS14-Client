using Robust.Shared.GameObjects;

namespace Content.Shared.GameTicking.Components;

[ByRefEvent]
public readonly record struct GameRuleAddedEvent(EntityUid RuleEntity, string RuleId);
