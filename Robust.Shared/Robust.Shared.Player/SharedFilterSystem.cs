using Robust.Shared.GameObjects;

namespace Robust.Shared.Player;

internal abstract class SharedFilterSystem : EntitySystem
{
	public abstract Filter FromEntities(Filter filter, params EntityUid[] entities);
}
