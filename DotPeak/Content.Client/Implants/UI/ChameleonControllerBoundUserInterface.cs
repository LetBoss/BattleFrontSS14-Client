// Decompiled with JetBrains decompiler
// Type: Content.Client.Implants.UI.ChameleonControllerBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Implants;
using Content.Shared.Timing;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Client.Implants.UI;

public sealed class ChameleonControllerBoundUserInterface : BoundUserInterface
{
  private readonly UseDelaySystem _delay;
  [Robust.Shared.ViewVariables.ViewVariables]
  private ChameleonControllerMenu? _menu;

  public ChameleonControllerBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._delay = this.EntMan.System<UseDelaySystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<ChameleonControllerMenu>((BoundUserInterface) this);
    this._menu.OnJobSelected += new Action<ProtoId<ChameleonOutfitPrototype>>(this.OnJobSelected);
  }

  private void OnJobSelected(ProtoId<ChameleonOutfitPrototype> outfit)
  {
    UseDelayComponent useDelayComponent;
    if (!this.EntMan.TryGetComponent<UseDelayComponent>(this.Owner, ref useDelayComponent) || !this._delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((this.Owner, useDelayComponent)), true))
      return;
    this.SendMessage((BoundUserInterfaceMessage) new ChameleonControllerSelectedOutfitMessage(outfit));
    UseDelayInfo info;
    if (!this._delay.TryGetDelayInfo(Entity<UseDelayComponent>.op_Implicit((this.Owner, useDelayComponent)), out info) || this._menu == null)
      return;
    this._menu._lockedUntil = new DateTime?(DateTime.Now.Add(info.Length));
    this._menu.UpdateGrid(true);
  }
}
