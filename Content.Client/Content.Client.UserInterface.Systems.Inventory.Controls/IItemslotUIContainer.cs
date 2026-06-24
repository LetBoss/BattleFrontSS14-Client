using Content.Client.UserInterface.Controls;

namespace Content.Client.UserInterface.Systems.Inventory.Controls;

public interface IItemslotUIContainer
{
	bool TryRegisterButton(SlotControl control, string newSlotName);

	bool TryAddButton(SlotControl control);
}
