using Robust.Shared.GameObjects;

namespace Content.Shared.Construction;

public readonly struct GraphTransformArgs(IEntityManager entityManager)
{
	public readonly IEntityManager EntityManager = entityManager;
}
