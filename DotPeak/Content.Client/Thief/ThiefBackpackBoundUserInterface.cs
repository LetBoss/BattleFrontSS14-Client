// Decompiled with JetBrains decompiler
// Type: Content.Client.Thief.ThiefBackpackBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Thief;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Thief;

public sealed class ThiefBackpackBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  private ThiefBackpackMenu? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<ThiefBackpackMenu>((BoundUserInterface) this);
    this._window.OnApprove += new Action(this.SendApprove);
    this._window.OnSetChange += new Action<int>(this.SendChangeSelected);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is ThiefBackpackBoundUserInterfaceState state1))
      return;
    this._window?.UpdateState(state1);
  }

  public void SendChangeSelected(int setNumber)
  {
    this.SendMessage((BoundUserInterfaceMessage) new ThiefBackpackChangeSetMessage(setNumber));
  }

  public void SendApprove()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ThiefBackpackApproveMessage());
  }
}
