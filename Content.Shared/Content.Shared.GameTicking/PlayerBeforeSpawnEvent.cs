using Content.Shared.Preferences;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;

namespace Content.Shared.GameTicking;

public sealed class PlayerBeforeSpawnEvent : HandledEntityEventArgs
{
	public ICommonSession Player { get; }

	public HumanoidCharacterProfile Profile { get; }

	public string? JobId { get; }

	public bool LateJoin { get; }

	public EntityUid Station { get; }

	public PlayerBeforeSpawnEvent(ICommonSession player, HumanoidCharacterProfile profile, string? jobId, bool lateJoin, EntityUid station)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Player = player;
		Profile = profile;
		JobId = jobId;
		LateJoin = lateJoin;
		Station = station;
	}
}
