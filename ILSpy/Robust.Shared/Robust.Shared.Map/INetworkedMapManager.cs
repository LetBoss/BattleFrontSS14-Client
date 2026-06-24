using Robust.Shared.Timing;

namespace Robust.Shared.Map;

internal interface INetworkedMapManager : IMapManagerInternal, IMapManager
{
	void CullDeletionHistory(GameTick upToTick);
}
