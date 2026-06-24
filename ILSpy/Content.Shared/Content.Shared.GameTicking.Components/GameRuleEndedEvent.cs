using Robust.Shared.GameObjects;

namespace Content.Shared.GameTicking.Components;

[ByRefEvent]
public readonly record struct GameRuleEndedEvent(EntityUid RuleEntity, string RuleId);
