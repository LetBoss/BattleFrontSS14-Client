// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Commander.CivAirstrikeFlybySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Commander;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._CIV14merka.Commander;

public sealed class CivAirstrikeFlybySystem : EntitySystem
{
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IGameTiming _timing;
  private CivAirstrikeFlybyOverlay? _overlay;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<CivAirstrikeFlybyEvent>(new EntityEventHandler<CivAirstrikeFlybyEvent>(this.OnFlyby), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlay != null && this._overlayManager.HasOverlay<CivAirstrikeFlybyOverlay>())
      this._overlayManager.RemoveOverlay((Overlay) this._overlay);
    this._overlay = (CivAirstrikeFlybyOverlay) null;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._overlay == null)
      return;
    TimeSpan realTime = this._timing.RealTime;
    List<CivAirstrikeFlybyInstance> instances = this._overlay.Instances;
    for (int index = instances.Count - 1; index >= 0; --index)
    {
      CivAirstrikeFlybyInstance airstrikeFlybyInstance = instances[index];
      if ((realTime - airstrikeFlybyInstance.StartTime).TotalSeconds > (double) (airstrikeFlybyInstance.Total / airstrikeFlybyInstance.Speed) + 0.5)
        instances.RemoveAt(index);
    }
  }

  private void OnFlyby(CivAirstrikeFlybyEvent ev)
  {
    this.EnsureOverlay();
    if (this._overlay == null)
      return;
    TimeSpan timeSpan = this._timing.RealTime + TimeSpan.FromSeconds((double) ev.StartDelay);
    this._overlay.Instances.Add(new CivAirstrikeFlybyInstance()
    {
      Origin = ev.Origin,
      Entry = ev.Entry,
      EntryCtr = ev.EntryCtr,
      Approach = ev.Approach,
      Target = ev.Target,
      RunEnd = ev.RunEnd,
      ExitTurn = ev.ExitTurn,
      ExitCtr = ev.ExitCtr,
      Exit = ev.Exit,
      EntryLineLen = ev.EntryLineLen,
      EntryArcLen = ev.EntryArcLen,
      ExitLen = ev.ExitLen,
      EntryCcw = ev.EntryCcw,
      ExitCcw = ev.ExitCcw,
      Speed = ev.Speed,
      Count = ev.Count,
      Spacing = ev.Spacing,
      Alpha = ev.Alpha,
      ScaleMin = ev.ScaleMin,
      ScaleMax = ev.ScaleMax,
      Side = ev.Side,
      MapId = ev.MapId,
      StartTime = timeSpan
    });
  }

  private void EnsureOverlay()
  {
    if (this._overlay != null)
      return;
    this._overlay = new CivAirstrikeFlybyOverlay();
    if (this._overlayManager.HasOverlay<CivAirstrikeFlybyOverlay>())
      return;
    this._overlayManager.AddOverlay((Overlay) this._overlay);
  }
}
