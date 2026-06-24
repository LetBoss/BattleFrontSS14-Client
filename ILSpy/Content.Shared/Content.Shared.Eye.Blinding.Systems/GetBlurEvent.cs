using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Eye.Blinding.Systems;

public sealed class GetBlurEvent : EntityEventArgs, IInventoryRelayEvent
{
	public readonly float BaseBlur;

	public float Blur;

	public float CorrectionPower = 2f;

	public SlotFlags TargetSlots => SlotFlags.HEAD | SlotFlags.EYES | SlotFlags.MASK;

	public GetBlurEvent(float blur)
	{
		Blur = blur;
		BaseBlur = blur;
	}
}
