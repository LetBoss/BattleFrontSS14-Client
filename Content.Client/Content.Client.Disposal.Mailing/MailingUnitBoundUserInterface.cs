using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Disposal.Unit;
using Content.Client.Power.EntitySystems;
using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using Content.Shared.Power.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.Disposal.Mailing;

public sealed class MailingUnitBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	public MailingUnitWindow? MailingUnitWindow;

	public MailingUnitBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	private void ButtonPressed(DisposalUnitComponent.UiButton button)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new DisposalUnitComponent.UiButtonPressedMessage(button));
	}

	private void TargetSelected(ItemListSelectedEventArgs args)
	{
		Item val = ((ItemListEventArgs)args).ItemList[args.ItemIndex];
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new TargetSelectedMessage(val.Text));
	}

	protected override void Open()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		MailingUnitWindow = BoundUserInterfaceExt.CreateWindow<MailingUnitWindow>((BoundUserInterface)(object)this);
		((BaseWindow)MailingUnitWindow).OpenCenteredRight();
		((BaseButton)MailingUnitWindow.Eject).OnPressed += delegate
		{
			ButtonPressed(DisposalUnitComponent.UiButton.Eject);
		};
		((BaseButton)MailingUnitWindow.Engage).OnPressed += delegate
		{
			ButtonPressed(DisposalUnitComponent.UiButton.Engage);
		};
		((BaseButton)MailingUnitWindow.Power).OnPressed += delegate
		{
			ButtonPressed(DisposalUnitComponent.UiButton.Power);
		};
		MailingUnitWindow.TargetListContainer.OnItemSelected += TargetSelected;
		MailingUnitComponent item = default(MailingUnitComponent);
		if (base.EntMan.TryGetComponent<MailingUnitComponent>(((BoundUserInterface)this).Owner, ref item))
		{
			Refresh(Entity<MailingUnitComponent>.op_Implicit((((BoundUserInterface)this).Owner, item)));
		}
	}

	public void Refresh(Entity<MailingUnitComponent> entity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (MailingUnitWindow != null)
		{
			DisposalUnitComponent disposalUnitComponent = default(DisposalUnitComponent);
			if (base.EntMan.TryGetComponent<DisposalUnitComponent>(entity.Owner, ref disposalUnitComponent))
			{
				DisposalUnitSystem disposalUnitSystem = base.EntMan.System<DisposalUnitSystem>();
				DisposalsPressureState state = disposalUnitSystem.GetState(((BoundUserInterface)this).Owner, disposalUnitComponent);
				TimeSpan timeSpan = disposalUnitSystem.EstimatedFullPressure(((BoundUserInterface)this).Owner, disposalUnitComponent);
				MailingUnitWindow.UnitState.Text = Loc.GetString($"disposal-unit-state-{state}");
				MailingUnitWindow.FullPressure = timeSpan;
				MailingUnitWindow.PressureBar.UpdatePressure(timeSpan);
				((BaseButton)MailingUnitWindow.Power).Pressed = base.EntMan.System<PowerReceiverSystem>().IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(((BoundUserInterface)this).Owner));
				((BaseButton)MailingUnitWindow.Engage).Pressed = disposalUnitComponent.Engaged;
			}
			MailingUnitWindow.Title = Loc.GetString("ui-mailing-unit-window-title", new(string, object)[1] { ("tag", entity.Comp.Tag ?? " ") });
			MailingUnitWindow.Target.Text = entity.Comp.Target;
			List<Item> items = ((IEnumerable<string>)entity.Comp.TargetList).Select((Func<string, Item>)((string target) => new Item(MailingUnitWindow.TargetListContainer)
			{
				Text = target,
				Selected = (target == entity.Comp.Target)
			})).ToList();
			MailingUnitWindow.TargetListContainer.SetItems(items);
		}
	}
}
