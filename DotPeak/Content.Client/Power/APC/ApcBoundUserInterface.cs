// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.APC.ApcBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power.APC.UI;
using Content.Shared.Access.Systems;
using Content.Shared.APC;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Power.APC;

public sealed class ApcBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ApcMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<ApcMenu>((BoundUserInterface) this);
    this._menu.SetEntity(this.Owner);
    this._menu.OnBreaker += new Action(this.BreakerPressed);
    bool hasAccess = false;
    if (this.PlayerManager.LocalEntity.HasValue)
      hasAccess = this.EntMan.System<AccessReaderSystem>().IsAllowed(this.PlayerManager.LocalEntity.Value, this.Owner);
    this._menu?.SetAccessEnabled(hasAccess);
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    this._menu?.UpdateState(state);
  }

  public void BreakerPressed()
  {
    this.SendMessage((BoundUserInterfaceMessage) new ApcToggleMainBreakerMessage());
  }
}
