using Robust.Shared.GameObjects;

namespace Content.Shared.IdentityManagement;

[ByRefEvent]
public record struct IdentityChangedEvent(EntityUid CharacterEntity, EntityUid IdentityEntity);
