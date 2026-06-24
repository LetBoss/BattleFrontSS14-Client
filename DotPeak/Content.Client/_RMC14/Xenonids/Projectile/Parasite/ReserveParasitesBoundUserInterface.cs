// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Projectile.Parasite.ReserveParasitesBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Projectile.Parasite;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Projectile.Parasite;

public sealed class ReserveParasitesBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private ReserveParasitesWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<ReserveParasitesWindow>((BoundUserInterface) this);
    XenoParasiteThrowerComponent throwerComponent;
    if (this.EntMan.TryGetComponent<XenoParasiteThrowerComponent>(this.Owner, ref throwerComponent))
      this._window.SetReserveShown(throwerComponent.ReservedParasites);
    ((BaseButton) this._window.ApplyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendMessage((BoundUserInterfaceMessage) new XenoChangeParasiteReserveMessage(this._window.ReserveBar.Value));
      ((BaseWindow) this._window).Close();
    });
  }

  public void ChangeReserve(int newReserve)
  {
    this.SendMessage((BoundUserInterfaceMessage) new XenoChangeParasiteReserveMessage(newReserve));
  }
}
