using System;
using Content.Client.Power.EntitySystems;
using Content.Shared.Disposal.Components;
using Content.Shared.Power.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.Disposal.Unit;

public sealed class DisposalUnitBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private DisposalUnitWindow? _disposalUnitWindow;

	public DisposalUnitBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	private void ButtonPressed(DisposalUnitComponent.UiButton button)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DisposalUnitComponent.UiButtonPressedMessage(button));
	}

	protected override void Open()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_disposalUnitWindow = BoundUserInterfaceExt.CreateWindow<DisposalUnitWindow>((BoundUserInterface)(object)this);
		((BaseWindow)_disposalUnitWindow).OpenCenteredRight();
		((BaseButton)_disposalUnitWindow.Eject).OnPressed += delegate
		{
			ButtonPressed(DisposalUnitComponent.UiButton.Eject);
		};
		((BaseButton)_disposalUnitWindow.Engage).OnPressed += delegate
		{
			ButtonPressed(DisposalUnitComponent.UiButton.Engage);
		};
		((BaseButton)_disposalUnitWindow.Power).OnPressed += delegate
		{
			ButtonPressed(DisposalUnitComponent.UiButton.Power);
		};
		DisposalUnitComponent item = default(DisposalUnitComponent);
		if (base.EntMan.TryGetComponent<DisposalUnitComponent>(((BoundUserInterface)this).Owner, ref item))
		{
			Refresh(Entity<DisposalUnitComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)));
		}
	}

	public void Refresh(Entity<DisposalUnitComponent> entity)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (_disposalUnitWindow != null)
		{
			DisposalUnitSystem disposalUnitSystem = base.EntMan.System<DisposalUnitSystem>();
			_disposalUnitWindow.Title = base.EntMan.GetComponent<MetaDataComponent>(entity.Owner).EntityName;
			DisposalsPressureState state = disposalUnitSystem.GetState(entity.Owner, entity.Comp);
			_disposalUnitWindow.UnitState.Text = Loc.GetString($"disposal-unit-state-{state}");
			((BaseButton)_disposalUnitWindow.Power).Pressed = base.EntMan.System<PowerReceiverSystem>().IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(((BoundUserInterface)this).Owner));
			((BaseButton)_disposalUnitWindow.Engage).Pressed = entity.Comp.Engaged;
			_disposalUnitWindow.FullPressure = disposalUnitSystem.EstimatedFullPressure(entity.Owner, entity.Comp);
		}
	}
}
