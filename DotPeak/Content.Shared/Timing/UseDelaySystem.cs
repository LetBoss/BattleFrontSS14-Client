// Decompiled with JetBrains decompiler
// Type: Content.Shared.Timing.UseDelaySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Timing;

public sealed class UseDelaySystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private MetaDataSystem _metadata;
  private const string DefaultId = "default";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<UseDelayComponent, MapInitEvent>(new EntityEventRefHandler<UseDelayComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<UseDelayComponent, EntityUnpausedEvent>(new EntityEventRefHandler<UseDelayComponent, EntityUnpausedEvent>(this.OnUnpaused));
    this.SubscribeLocalEvent<UseDelayComponent, ComponentGetState>(new EntityEventRefHandler<UseDelayComponent, ComponentGetState>(this.OnDelayGetState));
    this.SubscribeLocalEvent<UseDelayComponent, ComponentHandleState>(new EntityEventRefHandler<UseDelayComponent, ComponentHandleState>(this.OnDelayHandleState));
  }

  private void OnDelayHandleState(Entity<UseDelayComponent> ent, ref ComponentHandleState args)
  {
    if (!(args.Current is UseDelayComponentState current))
      return;
    ent.Comp.Delays.Clear();
    foreach ((string key, UseDelayInfo useDelayInfo) in current.Delays)
      ent.Comp.Delays[key] = new UseDelayInfo(useDelayInfo.Length, useDelayInfo.StartTime, useDelayInfo.EndTime);
  }

  private void OnDelayGetState(Entity<UseDelayComponent> ent, ref ComponentGetState args)
  {
    args.State = (IComponentState) new UseDelayComponentState()
    {
      Delays = ent.Comp.Delays
    };
  }

  private void OnMapInit(Entity<UseDelayComponent> ent, ref MapInitEvent args)
  {
    this.SetLength((Entity<UseDelayComponent>) ((EntityUid) ent, ent.Comp), ent.Comp.Delay);
  }

  private void OnUnpaused(Entity<UseDelayComponent> ent, ref EntityUnpausedEvent args)
  {
    foreach (UseDelayInfo useDelayInfo in ent.Comp.Delays.Values)
      useDelayInfo.EndTime += args.PausedTime;
  }

  public bool SetLength(Entity<UseDelayComponent?> ent, TimeSpan length, string id = "default")
  {
    UseDelayComponent comp;
    this.EnsureComp<UseDelayComponent>(ent.Owner, out comp);
    UseDelayInfo useDelayInfo;
    if (comp.Delays.TryGetValue(id, out useDelayInfo))
    {
      if (useDelayInfo.Length == length)
        return true;
      useDelayInfo.Length = length;
    }
    else
      comp.Delays.Add(id, new UseDelayInfo(length));
    this.Dirty<UseDelayComponent>(ent);
    return true;
  }

  public bool IsDelayed(Entity<UseDelayComponent?> ent, string id = "default")
  {
    UseDelayInfo useDelayInfo;
    return this.Resolve<UseDelayComponent>(ent.Owner, ref ent.Comp, false) && ent.Comp.Delays.TryGetValue(id, out useDelayInfo) && useDelayInfo.EndTime >= this._gameTiming.CurTime;
  }

  public void CancelDelay(Entity<UseDelayComponent> ent, string id = "default")
  {
    UseDelayInfo useDelayInfo;
    if (!ent.Comp.Delays.TryGetValue(id, out useDelayInfo))
      return;
    useDelayInfo.EndTime = this._gameTiming.CurTime;
    this.Dirty<UseDelayComponent>(ent);
  }

  public bool TryGetDelayInfo(Entity<UseDelayComponent?> ent, [NotNullWhen(true)] out UseDelayInfo? info, string id = "default")
  {
    if (this.Resolve<UseDelayComponent>(ent.Owner, ref ent.Comp, false))
      return ent.Comp.Delays.TryGetValue(id, out info);
    info = (UseDelayInfo) null;
    return false;
  }

  public UseDelayInfo GetLastEndingDelay(Entity<UseDelayComponent> ent)
  {
    UseDelayInfo lastEndingDelay;
    if (!ent.Comp.Delays.TryGetValue("default", out lastEndingDelay))
      return new UseDelayInfo(TimeSpan.Zero);
    foreach (KeyValuePair<string, UseDelayInfo> delay in ent.Comp.Delays)
    {
      if (delay.Value.EndTime > lastEndingDelay.EndTime)
        lastEndingDelay = delay.Value;
    }
    return lastEndingDelay;
  }

  public bool TryResetDelay(Entity<UseDelayComponent> ent, bool checkDelayed = false, string id = "default")
  {
    UseDelayInfo useDelayInfo;
    if (checkDelayed && this.IsDelayed((Entity<UseDelayComponent>) (ent.Owner, ent.Comp), id) || !ent.Comp.Delays.TryGetValue(id, out useDelayInfo))
      return false;
    TimeSpan curTime = this._gameTiming.CurTime;
    useDelayInfo.StartTime = curTime;
    useDelayInfo.EndTime = curTime - this._metadata.GetPauseTime((EntityUid) ent) + useDelayInfo.Length;
    this.Dirty<UseDelayComponent>(ent);
    return true;
  }

  public bool TryResetDelay(
    EntityUid uid,
    bool checkDelayed = false,
    UseDelayComponent? component = null,
    string id = "default")
  {
    return this.Resolve<UseDelayComponent>(uid, ref component, false) && this.TryResetDelay((Entity<UseDelayComponent>) (uid, component), checkDelayed, id);
  }

  public void ResetAllDelays(Entity<UseDelayComponent> ent)
  {
    TimeSpan curTime = this._gameTiming.CurTime;
    foreach (UseDelayInfo useDelayInfo in ent.Comp.Delays.Values)
    {
      useDelayInfo.StartTime = curTime;
      useDelayInfo.EndTime = curTime - this._metadata.GetPauseTime((EntityUid) ent) + useDelayInfo.Length;
    }
    this.Dirty<UseDelayComponent>(ent);
  }
}
