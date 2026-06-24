// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Respawn.CivRespawnSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Respawn.UI;
using Content.Shared._CIV14merka;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Respawn;

public sealed class CivRespawnSystem : EntitySystem
{
  private readonly Queue<CivRespawnAvailableEvent> _pendingRespawns = new Queue<CivRespawnAvailableEvent>();
  private readonly HashSet<int> _pendingRespawnIds = new HashSet<int>();
  private CivRespawnWindow? _window;
  private CivRespawnAvailableEvent? _activeRespawn;
  private bool _closingWindow;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivRespawnAvailableEvent>(new EntitySessionEventHandler<CivRespawnAvailableEvent>(this.OnRespawnAvailable), (Type[]) null, (Type[]) null);
  }

  private void OnRespawnAvailable(CivRespawnAvailableEvent msg, EntitySessionEventArgs args)
  {
    this.EnsureWindow();
    if (!this._pendingRespawnIds.Add(msg.RespawnId))
      return;
    this._pendingRespawns.Enqueue(msg);
    this.ShowNextRespawn();
  }

  private void EnsureWindow()
  {
    if (this._window != null)
      return;
    this._window = new CivRespawnWindow();
    this._window.AcceptPressed += new Action(this.OnAcceptPressed);
    this._window.DeclinePressed += new Action(this.OnDeclinePressed);
    ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
  }

  private void OnAcceptPressed() => this.Respond(true);

  private void OnDeclinePressed() => this.Respond(false);

  private void OnWindowClosed()
  {
    if (this._closingWindow)
    {
      this._closingWindow = false;
      this.DisposeWindow();
    }
    else
    {
      this.RespondToAllPending(false);
      this.DisposeWindow();
    }
  }

  private void DisposeWindow()
  {
    if (this._window == null)
      return;
    this._window.AcceptPressed -= new Action(this.OnAcceptPressed);
    this._window.DeclinePressed -= new Action(this.OnDeclinePressed);
    ((BaseWindow) this._window).OnClose -= new Action(this.OnWindowClosed);
    this._window = (CivRespawnWindow) null;
  }

  private void Respond(bool accept)
  {
    if (this._activeRespawn == null)
      return;
    int respawnId = this._activeRespawn.RespawnId;
    this._activeRespawn = (CivRespawnAvailableEvent) null;
    this._pendingRespawnIds.Remove(respawnId);
    this.RaiseNetworkEvent((EntityEventArgs) new CivRespawnChoiceEvent(respawnId, accept));
    this.ShowNextRespawn();
  }

  private void RespondToAllPending(bool accept)
  {
    if (this._activeRespawn != null)
    {
      int respawnId = this._activeRespawn.RespawnId;
      this._activeRespawn = (CivRespawnAvailableEvent) null;
      this._pendingRespawnIds.Remove(respawnId);
      this.RaiseNetworkEvent((EntityEventArgs) new CivRespawnChoiceEvent(respawnId, accept));
    }
    CivRespawnAvailableEvent result;
    while (this._pendingRespawns.TryDequeue(out result))
    {
      this._pendingRespawnIds.Remove(result.RespawnId);
      this.RaiseNetworkEvent((EntityEventArgs) new CivRespawnChoiceEvent(result.RespawnId, accept));
    }
  }

  private void ShowNextRespawn()
  {
    this.EnsureWindow();
    CivRespawnAvailableEvent result;
    if (this._activeRespawn == null && this._pendingRespawns.TryDequeue(out result))
      this._activeRespawn = result;
    if (this._activeRespawn == null)
    {
      CivRespawnWindow window = this._window;
      if (window == null || !((BaseWindow) window).IsOpen)
        return;
      this._closingWindow = true;
      ((BaseWindow) this._window).Close();
    }
    else
    {
      if (((BaseWindow) this._window).IsOpen)
        return;
      ((BaseWindow) this._window).OpenCentered();
    }
  }
}
