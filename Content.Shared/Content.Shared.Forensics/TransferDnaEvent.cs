using Robust.Shared.GameObjects;

namespace Content.Shared.Forensics;

[ByRefEvent]
public record struct TransferDnaEvent()
{
	public EntityUid Donor = default(EntityUid);

	public EntityUid Recipient = default(EntityUid);

	public bool CanDnaBeCleaned = true;
}
