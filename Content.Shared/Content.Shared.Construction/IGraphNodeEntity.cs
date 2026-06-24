using Robust.Shared.GameObjects;

namespace Content.Shared.Construction;

public interface IGraphNodeEntity
{
	string? GetId(EntityUid? uid, EntityUid? userUid, GraphNodeEntityArgs args);
}
