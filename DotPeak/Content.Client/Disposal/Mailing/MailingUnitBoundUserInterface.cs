// Decompiled with JetBrains decompiler
// Type: Content.Client.Disposal.Mailing.MailingUnitBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Disposal.Unit;
using Content.Client.Power.EntitySystems;
using Content.Shared.Disposal;
using Content.Shared.Disposal.Components;
using Content.Shared.Power.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;
using System.Linq;

#nullable enable
namespace Content.Client.Disposal.Mailing;

public sealed class MailingUnitBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  public MailingUnitWindow? MailingUnitWindow;

  private void ButtonPressed(DisposalUnitComponent.UiButton button)
  {
    this.SendMessage((BoundUserInterfaceMessage) new DisposalUnitComponent.UiButtonPressedMessage(button));
  }

  private void TargetSelected(ItemList.ItemListSelectedEventArgs args)
  {
    this.SendMessage((BoundUserInterfaceMessage) new TargetSelectedMessage(((ItemList.ItemListEventArgs) args).ItemList[args.ItemIndex].Text));
  }

  protected virtual void Open()
  {
    base.Open();
    this.MailingUnitWindow = BoundUserInterfaceExt.CreateWindow<MailingUnitWindow>((BoundUserInterface) this);
    this.MailingUnitWindow.OpenCenteredRight();
    ((BaseButton) this.MailingUnitWindow.Eject).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ButtonPressed(DisposalUnitComponent.UiButton.Eject));
    ((BaseButton) this.MailingUnitWindow.Engage).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ButtonPressed(DisposalUnitComponent.UiButton.Engage));
    ((BaseButton) this.MailingUnitWindow.Power).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ButtonPressed(DisposalUnitComponent.UiButton.Power));
    this.MailingUnitWindow.TargetListContainer.OnItemSelected += new Action<ItemList.ItemListSelectedEventArgs>(this.TargetSelected);
    MailingUnitComponent mailingUnitComponent;
    if (!this.EntMan.TryGetComponent<MailingUnitComponent>(this.Owner, ref mailingUnitComponent))
      return;
    this.Refresh(Entity<MailingUnitComponent>.op_Implicit((this.Owner, mailingUnitComponent)));
  }

  public void Refresh(Entity<MailingUnitComponent> entity)
  {
    if (this.MailingUnitWindow == null)
      return;
    DisposalUnitComponent component;
    if (this.EntMan.TryGetComponent<DisposalUnitComponent>(entity.Owner, ref component))
    {
      DisposalUnitSystem disposalUnitSystem = this.EntMan.System<DisposalUnitSystem>();
      DisposalsPressureState state = disposalUnitSystem.GetState(this.Owner, component);
      TimeSpan fullTime = disposalUnitSystem.EstimatedFullPressure(this.Owner, component);
      this.MailingUnitWindow.UnitState.Text = Loc.GetString($"disposal-unit-state-{state}");
      this.MailingUnitWindow.FullPressure = fullTime;
      this.MailingUnitWindow.PressureBar.UpdatePressure(fullTime);
      ((BaseButton) this.MailingUnitWindow.Power).Pressed = this.EntMan.System<PowerReceiverSystem>().IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(this.Owner));
      ((BaseButton) this.MailingUnitWindow.Engage).Pressed = component.Engaged;
    }
    this.MailingUnitWindow.Title = Loc.GetString("ui-mailing-unit-window-title", new (string, object)[1]
    {
      ("tag", (object) (entity.Comp.Tag ?? " "))
    });
    this.MailingUnitWindow.Target.Text = entity.Comp.Target;
    this.MailingUnitWindow.TargetListContainer.SetItems(entity.Comp.TargetList.Select<string, ItemList.Item>((Func<string, ItemList.Item>) (target => new ItemList.Item(this.MailingUnitWindow.TargetListContainer)
    {
      Text = target,
      Selected = target == entity.Comp.Target
    })).ToList<ItemList.Item>());
  }
}
