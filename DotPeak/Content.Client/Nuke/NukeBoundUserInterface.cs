// Decompiled with JetBrains decompiler
// Type: Content.Client.Nuke.NukeBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Containers.ItemSlots;
using Content.Shared.Nuke;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Nuke;

public sealed class NukeBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private NukeMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<NukeMenu>((BoundUserInterface) this);
    this._menu.OnKeypadButtonPressed += (Action<int>) (i => this.SendMessage((BoundUserInterfaceMessage) new NukeKeypadMessage(i)));
    this._menu.OnEnterButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new NukeKeypadEnterMessage()));
    this._menu.OnClearButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new NukeKeypadClearMessage()));
    ((BaseButton) this._menu.EjectButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent("Nuke")));
    ((BaseButton) this._menu.AnchorButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new NukeAnchorMessage()));
    ((BaseButton) this._menu.ArmButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new NukeArmedMessage()));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._menu == null || !(state is NukeUiState state1))
      return;
    this._menu.UpdateState(state1);
  }
}
