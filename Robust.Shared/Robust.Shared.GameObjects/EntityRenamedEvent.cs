namespace Robust.Shared.GameObjects;

[ByRefEvent]
public readonly record struct EntityRenamedEvent(EntityUid Uid, string OldName, string NewName);
