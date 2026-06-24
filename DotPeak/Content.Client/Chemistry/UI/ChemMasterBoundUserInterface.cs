// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.UI.ChemMasterBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry;
using Content.Shared.Containers.ItemSlots;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Chemistry.UI;

public sealed class ChemMasterBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ChemMasterWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<ChemMasterWindow>((BoundUserInterface) this);
    this._window.Title = this.EntMan.GetComponent<MetaDataComponent>(this.Owner).EntityName;
    ((BaseButton) this._window.InputEjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent("beakerSlot")));
    ((BaseButton) this._window.OutputEjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent("outputSlot")));
    ((BaseButton) this._window.BufferTransferButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ChemMasterSetModeMessage(ChemMasterMode.Transfer)));
    ((BaseButton) this._window.BufferDiscardButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ChemMasterSetModeMessage(ChemMasterMode.Discard)));
    ((BaseButton) this._window.CreatePillButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ChemMasterCreatePillsMessage((uint) this._window.PillDosage.Value, (uint) this._window.PillNumber.Value, this._window.LabelLine)));
    ((BaseButton) this._window.CreateBottleButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ChemMasterOutputToBottleMessage((uint) this._window.BottleDosage.Value, this._window.LabelLine)));
    ((BaseButton) this._window.BufferSortButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ChemMasterSortingTypeCycleMessage()));
    for (uint index = 0; (long) index < (long) this._window.PillTypeButtons.Length; ++index)
    {
      uint pillType = index;
      ((BaseButton) this._window.PillTypeButtons[(int) index]).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ChemMasterSetPillTypeMessage(pillType)));
    }
    this._window.OnReagentButtonPressed += (Action<BaseButton.ButtonEventArgs, ReagentButton>) ((args, button) => this.SendMessage((BoundUserInterfaceMessage) new ChemMasterReagentAmountButtonMessage(button.Id, button.Amount, button.IsBuffer)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._window?.UpdateState(state);
  }
}
