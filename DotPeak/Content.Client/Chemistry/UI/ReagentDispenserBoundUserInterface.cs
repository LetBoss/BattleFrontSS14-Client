// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.UI.ReagentDispenserBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Controls;
using Content.Shared.Chemistry;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Storage;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Chemistry.UI;

public sealed class ReagentDispenserBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ReagentDispenserWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<ReagentDispenserWindow>((BoundUserInterface) this);
    this._window.SetInfoFromEntity(this.EntMan, this.Owner);
    ((BaseButton) this._window.EjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent("beakerSlot")));
    ((BaseButton) this._window.ClearButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ReagentDispenserClearContainerSolutionMessage()));
    this._window.AmountGrid.OnButtonPressed += (Action<string>) (s => this.SendMessage((BoundUserInterfaceMessage) new ReagentDispenserSetDispenseAmountMessage(s)));
    this._window.OnDispenseReagentButtonPressed += (Action<ItemStorageLocation>) (location => this.SendMessage((BoundUserInterfaceMessage) new ReagentDispenserDispenseReagentMessage(location)));
    this._window.OnEjectJugButtonPressed += (Action<ItemStorageLocation>) (location => this.SendMessage((BoundUserInterfaceMessage) new ReagentDispenserEjectContainerMessage(location)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.UpdateState(state);
  }
}
