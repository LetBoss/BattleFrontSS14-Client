using System;
using Content.Shared._RMC14.Attachable;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Attachable.Ui;

public sealed class AttachmentStripBui : BoundUserInterface
{
	private AttachableHolderStripMenu? _menu;

	public AttachmentStripBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<AttachableHolderStripMenu>((BoundUserInterface)(object)this);
		MetaDataComponent val = default(MetaDataComponent);
		if (base.EntMan.GetEntityQuery<MetaDataComponent>().TryGetComponent(((BoundUserInterface)this).Owner, ref val))
		{
			_menu.Title = val.EntityName;
		}
		((BaseWindow)_menu).OpenCentered();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is AttachableHolderStripUserInterfaceState attachableHolderStripUserInterfaceState)
		{
			_menu?.UpdateMenu(attachableHolderStripUserInterfaceState.AttachableSlots, delegate(string slotId)
			{
				((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AttachableHolderDetachMessage(slotId));
			});
		}
	}
}
