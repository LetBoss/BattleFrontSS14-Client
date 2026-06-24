using Robust.Shared.Map;

namespace Robust.Shared.GameObjects;

public readonly record struct MapRemovedEvent(EntityUid Uid, MapId MapId);
