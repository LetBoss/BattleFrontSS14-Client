using Robust.Shared.GameObjects;

namespace Content.Shared.CombatMode.Pacification;

[ByRefEvent]
public struct AttemptPacifiedThrowEvent(EntityUid itemUid, EntityUid playerUid)
{
	public EntityUid ItemUid = itemUid;

	public EntityUid PlayerUid = playerUid;

	public bool Cancelled { get; private set; } = false;

	public string? CancelReasonMessageId { get; private set; } = null;

	public void Cancel(string? reasonMessageId = null)
	{
		Cancelled = true;
		CancelReasonMessageId = reasonMessageId;
	}
}
