using System;
using Content.Shared._RMC14.Attachable;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Attachable.Ui;

public sealed class AttachmentChooseSlotBui : BoundUserInterface
{
	[ViewVariables]
	private AttachableHolderChooseSlotMenu? _menu;

	public AttachmentChooseSlotBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<AttachableHolderChooseSlotMenu>((BoundUserInterface)(object)this);
		MetaDataComponent val = default(MetaDataComponent);
		if (base.EntMan.GetEntityQuery<MetaDataComponent>().TryGetComponent(((BoundUserInterface)this).Owner, ref val))
		{
			_menu.Title = val.EntityName;
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is AttachableHolderChooseSlotUserInterfaceState attachableHolderChooseSlotUserInterfaceState && _menu != null)
		{
			_menu.UpdateMenu(attachableHolderChooseSlotUserInterfaceState.AttachableSlots, delegate(string slotId)
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AttachableHolderAttachToSlotMessage(slotId));
				((BaseWindow)_menu).Close();
			});
		}
	}
}
