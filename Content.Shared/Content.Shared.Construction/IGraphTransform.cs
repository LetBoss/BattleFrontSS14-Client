using Robust.Shared.GameObjects;

namespace Content.Shared.Construction;

public interface IGraphTransform
{
	void Transform(EntityUid oldUid, EntityUid newUid, EntityUid? userUid, GraphTransformArgs args);
}
