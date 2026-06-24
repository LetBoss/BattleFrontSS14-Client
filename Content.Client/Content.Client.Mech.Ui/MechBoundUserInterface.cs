using System;
using System.Collections.Generic;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Robust.Client.UserInterface;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Mech.Ui;

public sealed class MechBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private MechMenu? _menu;

	public MechBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<MechMenu>((BoundUserInterface)(object)this);
		_menu.SetEntity(((BoundUserInterface)this).Owner);
		_menu.OnRemoveButtonPressed += delegate(EntityUid uid)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new MechEquipmentRemoveMessage(base.EntMan.GetNetEntity(uid, (MetaDataComponent)null)));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is MechBoundUiState state2)
		{
			UpdateEquipmentControls(state2);
			_menu?.UpdateMechStats();
			_menu?.UpdateEquipmentView();
		}
	}

	public void UpdateEquipmentControls(MechBoundUiState state)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		MechComponent mechComponent = default(MechComponent);
		if (!base.EntMan.TryGetComponent<MechComponent>(((BoundUserInterface)this).Owner, ref mechComponent))
		{
			return;
		}
		foreach (EntityUid containedEntity in ((BaseContainer)mechComponent.EquipmentContainer).ContainedEntities)
		{
			UIFragment equipmentUi = GetEquipmentUi(containedEntity);
			if (equipmentUi == null)
			{
				continue;
			}
			foreach (var (val3, state2) in state.EquipmentStates)
			{
				if (containedEntity == base.EntMan.GetEntity(val3))
				{
					equipmentUi.UpdateState(state2);
				}
			}
		}
	}

	public UIFragment? GetEquipmentUi(EntityUid? uid)
	{
		UIFragmentComponent componentOrNull = EntityManagerExt.GetComponentOrNull<UIFragmentComponent>(base.EntMan, uid);
		componentOrNull?.Ui?.Setup((BoundUserInterface)(object)this, uid);
		return componentOrNull?.Ui;
	}
}
