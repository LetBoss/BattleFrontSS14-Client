using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Vendors;

public sealed class CMAutomatedVendorAccessAttemptEvent(EntityUid user, bool showPopup) : CancellableEntityEventArgs
{
	public EntityUid User { get; } = user;

	public bool ShowPopup { get; } = showPopup;

	public string? Reason { get; private set; }

	public void Deny(string? reason = null)
	{
		((CancellableEntityEventArgs)this).Cancel();
		Reason = reason;
	}
}
