// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.DropshipHijackerBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Message;
using Content.Shared._RMC14.Dropship;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Dropship;

public sealed class DropshipHijackerBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private DropshipHijackerWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    if (!(this.State is DropshipHijackerBuiState state))
      return;
    this.Set(state);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is DropshipHijackerBuiState s))
      return;
    this.Set(s);
  }

  private void Set(DropshipHijackerBuiState s)
  {
    if (this._window == null)
    {
      this._window = BoundUserInterfaceExt.CreateWindow<DropshipHijackerWindow>((BoundUserInterface) this);
      this._window.Header.SetMarkup("[bold]Where to 'land'?[/bold]");
    }
    ((Control) this._window.Destinations).DisposeAllChildren();
    foreach ((NetEntity netEntity, string Name) in s.Destinations)
    {
      Button button1 = new Button();
      button1.Text = Name;
      ((Control) button1).StyleClasses.Add("OpenBoth");
      Button button2 = button1;
      ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
      {
        this.SendPredictedMessage((BoundUserInterfaceMessage) new DropshipHijackerDestinationChosenBuiMsg(netEntity));
        this.Close();
      });
      ((Control) this._window.Destinations).AddChild((Control) button2);
    }
  }
}
