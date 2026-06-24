using Robust.Shared.GameObjects;

namespace Content.Shared._CIV14merka.Supply;

public sealed class CivSupplyVendItemAttemptEvent : CancellableEntityEventArgs
{
	public readonly EntityUid Actor;

	public readonly int Section;

	public readonly int Entry;

	public CivSupplyVendItemAttemptEvent(EntityUid actor, int section, int entry)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Actor = actor;
		Section = section;
		Entry = entry;
	}
}
