// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Supply.CivSupplyRefillSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Supply;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Supply;

public sealed class CivSupplyRefillSystem : EntitySystem
{
  private const float RefreshInterval = 1.5f;
  private CivSupplyRefillWindow? _window;
  private float _refreshTimer;

  public virtual void Initialize()
  {
    this.SubscribeNetworkEvent<CivSupplyRefillStateEvent>(new EntityEventHandler<CivSupplyRefillStateEvent>(this.OnState), (Type[]) null, (Type[]) null);
  }

  public void OpenWindow()
  {
    if (this._window == null)
    {
      this._window = new CivSupplyRefillWindow();
      this._window.OnSetPeriodic += (Action<string, int>) ((proto, amount) => this.RaiseNetworkEvent((EntityEventArgs) new CivSupplySetPeriodicEvent(proto, amount)));
      this._window.OnRefillNow += (Action<string, int>) ((proto, amount) => this.RaiseNetworkEvent((EntityEventArgs) new CivSupplyRefillNowEvent(proto, amount)));
      this._window.OnSetThreshold += (Action<int>) (threshold => this.RaiseNetworkEvent((EntityEventArgs) new CivSupplySetRefillThresholdEvent(threshold)));
      ((BaseWindow) this._window).OnClose += (Action) (() => this._window = (CivSupplyRefillWindow) null);
    }
    this._refreshTimer = 0.0f;
    this.RaiseNetworkEvent((EntityEventArgs) new CivSupplyRefillRequestEvent());
    ((BaseWindow) this._window).OpenCentered();
  }

  public void CloseWindow()
  {
    ((BaseWindow) this._window)?.Close();
    this._window = (CivSupplyRefillWindow) null;
  }

  public virtual void Update(float frameTime)
  {
    if (this._window == null)
      return;
    this._refreshTimer += frameTime;
    if ((double) this._refreshTimer < 1.5)
      return;
    this._refreshTimer = 0.0f;
    this.RaiseNetworkEvent((EntityEventArgs) new CivSupplyRefillRequestEvent());
  }

  private void OnState(CivSupplyRefillStateEvent ev)
  {
    if (this._window == null)
      return;
    this._window.Populate(ev.Entries);
    this._window.SetThreshold(ev.RefillThreshold);
  }
}
