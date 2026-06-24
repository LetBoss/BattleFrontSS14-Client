using Robust.Shared.GameObjects;

namespace Content.Shared.Cloning.Events;

[ByRefEvent]
public record struct CloningEvent(CloningSettingsPrototype Settings, EntityUid CloneUid);
