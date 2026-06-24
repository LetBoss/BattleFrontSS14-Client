using Robust.Shared.GameObjects;

namespace Content.Shared.GameTicking.Components;

[ByRefEvent]
public readonly record struct GameRuleStartedEvent(EntityUid RuleEntity, string RuleId);
