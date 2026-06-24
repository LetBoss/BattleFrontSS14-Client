using Robust.Shared.GameObjects;

namespace Content.Shared.Roles;

[ByRefEvent]
public record struct MindIsAntagonistEvent(bool IsAntagonist, bool IsExclusiveAntagonist);
