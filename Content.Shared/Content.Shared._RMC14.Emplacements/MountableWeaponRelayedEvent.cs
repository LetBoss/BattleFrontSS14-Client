using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Emplacements;

[ByRefEvent]
public record struct MountableWeaponRelayedEvent<TEvent>(TEvent Args);
