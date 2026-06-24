using Content.Shared.Power;
using Robust.Shared.GameObjects;

namespace Content.Client.Power;

public struct PowerMonitoringConsoleTrackable(EntityUid uid, PowerMonitoringConsoleGroup group)
{
	public EntityUid EntityUid = uid;

	public PowerMonitoringConsoleGroup Group = group;
}
