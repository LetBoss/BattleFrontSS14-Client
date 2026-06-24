namespace Content.Shared.Inventory;

public interface IInventoryRelayEvent
{
	SlotFlags TargetSlots { get; }
}
