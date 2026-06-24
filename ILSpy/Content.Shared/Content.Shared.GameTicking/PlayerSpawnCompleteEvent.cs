using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Shared.GameTicking;

public sealed class PlayerSpawnCompleteEvent : EntityEventArgs
{
	public EntityUid Mob { get; }

	public ICommonSession Player { get; }

	public string? JobId { get; }

	public bool LateJoin { get; }

	public bool Silent { get; }

	public EntityUid Station { get; }

	public HumanoidCharacterProfile Profile { get; }

	public int JoinOrder { get; }

	public PlayerSpawnCompleteEvent(EntityUid mob, ICommonSession player, string? jobId, bool lateJoin, bool silent, int joinOrder, EntityUid station, HumanoidCharacterProfile profile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Mob = mob;
		Player = player;
		JobId = jobId;
		LateJoin = lateJoin;
		Silent = silent;
		Station = station;
		Profile = profile;
		JoinOrder = joinOrder;
	}
}
