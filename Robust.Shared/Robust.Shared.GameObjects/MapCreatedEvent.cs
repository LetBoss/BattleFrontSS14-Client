using Robust.Shared.Map;

namespace Robust.Shared.GameObjects;

public readonly record struct MapCreatedEvent(EntityUid Uid, MapId MapId);
