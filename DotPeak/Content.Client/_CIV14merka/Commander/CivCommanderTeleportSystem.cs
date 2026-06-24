// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivCommanderTeleportSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Commander.UI;
using Content.Shared._CIV14merka.Commander;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;
using System.Linq;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivCommanderTeleportSystem : EntitySystem
{
  private CivCommanderTeleportWindow? _window;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivCommanderTeleportTargetsResponseEvent>(new EntitySessionEventHandler<CivCommanderTeleportTargetsResponseEvent>(this.OnTargetsResponse), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    ((BaseWindow) this._window)?.Close();
    this._window = (CivCommanderTeleportWindow) null;
  }

  public void OpenWindow()
  {
    this.EnsureWindow();
    ((BaseWindow) this._window).OpenCentered();
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderTeleportTargetsRequestEvent());
  }

  public void CloseWindow() => ((BaseWindow) this._window)?.Close();

  private void EnsureWindow()
  {
    if (this._window != null)
      return;
    this._window = new CivCommanderTeleportWindow();
    this._window.TargetSelected += new Action<NetEntity>(this.OnTargetSelected);
  }

  private void OnTargetsResponse(
    CivCommanderTeleportTargetsResponseEvent msg,
    EntitySessionEventArgs args)
  {
    this.EnsureWindow();
    this._window.UpdateTargets(msg.Targets.Select<CivCommanderTeleportTargetState, (string, NetEntity)>((Func<CivCommanderTeleportTargetState, (string, NetEntity)>) (target => (target.Name, target.Entity))));
  }

  private void OnTargetSelected(NetEntity target)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new CivCommanderTeleportToTargetRequestEvent(target));
    ((BaseWindow) this._window)?.Close();
  }
}
