using Content.Shared._RMC14.Movement;
using Robust.Client.Timing;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Client._RMC14.Movement;

public sealed class RMCLagCompensationSystem : SharedRMCLagCompensationSystem
{
	[Dependency]
	private IClientGameTiming _timing;

	public override GameTick GetLastRealTick(NetUserId? session)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _timing.LastRealTick;
	}
}
