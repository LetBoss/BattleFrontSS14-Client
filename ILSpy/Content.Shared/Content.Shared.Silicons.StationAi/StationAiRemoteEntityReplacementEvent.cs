using Robust.Shared.GameObjects;

namespace Content.Shared.Silicons.StationAi;

[ByRefEvent]
public record struct StationAiRemoteEntityReplacementEvent(EntityUid? NewRemoteEntity);
