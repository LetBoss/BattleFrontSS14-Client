// Decompiled with JetBrains decompiler
// Type: Content.Client.Disposal.Unit.DisposalUnitBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power.EntitySystems;
using Content.Shared.Disposal.Components;
using Content.Shared.Power.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Disposal.Unit;

public sealed class DisposalUnitBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private DisposalUnitWindow? _disposalUnitWindow;

  private void ButtonPressed(DisposalUnitComponent.UiButton button)
  {
    this.SendPredictedMessage((BoundUserInterfaceMessage) new DisposalUnitComponent.UiButtonPressedMessage(button));
  }

  protected virtual void Open()
  {
    base.Open();
    this._disposalUnitWindow = BoundUserInterfaceExt.CreateWindow<DisposalUnitWindow>((BoundUserInterface) this);
    this._disposalUnitWindow.OpenCenteredRight();
    ((BaseButton) this._disposalUnitWindow.Eject).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ButtonPressed(DisposalUnitComponent.UiButton.Eject));
    ((BaseButton) this._disposalUnitWindow.Engage).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ButtonPressed(DisposalUnitComponent.UiButton.Engage));
    ((BaseButton) this._disposalUnitWindow.Power).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ButtonPressed(DisposalUnitComponent.UiButton.Power));
    DisposalUnitComponent disposalUnitComponent;
    if (!this.EntMan.TryGetComponent<DisposalUnitComponent>(this.Owner, ref disposalUnitComponent))
      return;
    this.Refresh(Entity<DisposalUnitComponent>.op_Implicit((this.Owner, disposalUnitComponent)));
  }

  public void Refresh(Entity<DisposalUnitComponent> entity)
  {
    if (this._disposalUnitWindow == null)
      return;
    DisposalUnitSystem disposalUnitSystem = this.EntMan.System<DisposalUnitSystem>();
    this._disposalUnitWindow.Title = this.EntMan.GetComponent<MetaDataComponent>(entity.Owner).EntityName;
    this._disposalUnitWindow.UnitState.Text = Loc.GetString($"disposal-unit-state-{disposalUnitSystem.GetState(entity.Owner, entity.Comp)}");
    ((BaseButton) this._disposalUnitWindow.Power).Pressed = this.EntMan.System<PowerReceiverSystem>().IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(this.Owner));
    ((BaseButton) this._disposalUnitWindow.Engage).Pressed = entity.Comp.Engaged;
    this._disposalUnitWindow.FullPressure = disposalUnitSystem.EstimatedFullPressure(entity.Owner, entity.Comp);
  }
}
