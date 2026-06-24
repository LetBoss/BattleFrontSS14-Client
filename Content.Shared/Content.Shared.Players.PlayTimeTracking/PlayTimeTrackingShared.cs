using Robust.Shared.Prototypes;

namespace Content.Shared.Players.PlayTimeTracking;

public static class PlayTimeTrackingShared
{
	public static readonly ProtoId<PlayTimeTrackerPrototype> TrackerOverall = ProtoId<PlayTimeTrackerPrototype>.op_Implicit("Overall");

	public static readonly ProtoId<PlayTimeTrackerPrototype> TrackerAdmin = ProtoId<PlayTimeTrackerPrototype>.op_Implicit("Admin");
}
