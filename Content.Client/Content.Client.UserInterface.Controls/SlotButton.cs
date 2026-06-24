using Content.Client.Inventory;

namespace Content.Client.UserInterface.Controls;

public sealed class SlotButton : SlotControl
{
	public SlotButton()
	{
	}

	public SlotButton(ClientInventorySystem.SlotData slotData)
	{
		base.ButtonTexturePath = slotData.TextureName;
		base.FullButtonTexturePath = slotData.FullTextureName;
		base.Blocked = slotData.Blocked;
		base.Highlight = slotData.Highlighted;
		base.StorageTexturePath = "Slots/back";
		base.SlotName = slotData.SlotName;
	}
}
