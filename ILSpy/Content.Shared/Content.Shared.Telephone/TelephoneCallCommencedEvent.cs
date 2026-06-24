using Robust.Shared.GameObjects;

namespace Content.Shared.Telephone;

[ByRefEvent]
public record struct TelephoneCallCommencedEvent(Entity<TelephoneComponent> Receiver);
