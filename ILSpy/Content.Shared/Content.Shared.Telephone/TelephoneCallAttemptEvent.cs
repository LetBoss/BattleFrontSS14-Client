using Robust.Shared.GameObjects;

namespace Content.Shared.Telephone;

[ByRefEvent]
public record struct TelephoneCallAttemptEvent(Entity<TelephoneComponent> Source, Entity<TelephoneComponent> Receiver, EntityUid? User)
{
	public bool Cancelled = false;
}
