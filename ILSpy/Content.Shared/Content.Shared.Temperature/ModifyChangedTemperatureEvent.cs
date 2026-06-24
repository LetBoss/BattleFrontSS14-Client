using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

namespace Content.Shared.Temperature;

public sealed class ModifyChangedTemperatureEvent : EntityEventArgs, IInventoryRelayEvent
{
	public float TemperatureDelta;

	public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

	public ModifyChangedTemperatureEvent(float temperature)
	{
		TemperatureDelta = temperature;
	}
}
