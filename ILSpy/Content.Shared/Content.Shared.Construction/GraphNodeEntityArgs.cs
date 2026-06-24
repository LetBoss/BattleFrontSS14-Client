using Robust.Shared.GameObjects;

namespace Content.Shared.Construction;

public readonly struct GraphNodeEntityArgs(IEntityManager entityManager)
{
	public readonly IEntityManager EntityManager = entityManager;
}
