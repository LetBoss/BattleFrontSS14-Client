using Robust.Shared.GameObjects;

[ByRefEvent]
public record struct OnAccessOverriderAccessUpdatedEvent(EntityUid UserUid, bool Handled = false);
