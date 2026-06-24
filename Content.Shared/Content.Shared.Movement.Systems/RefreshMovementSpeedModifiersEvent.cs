using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Systems;

public sealed class RefreshMovementSpeedModifiersEvent : EntityEventArgs, IInventoryRelayEvent
{
	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public float WalkSpeedModifier { get; private set; } = 1f;

	public float SprintSpeedModifier { get; private set; } = 1f;

	public void ModifySpeed(float walk, float sprint)
	{
		WalkSpeedModifier *= walk;
		SprintSpeedModifier *= sprint;
	}

	public void ModifySpeed(float mod)
	{
		ModifySpeed(mod, mod);
	}
}
