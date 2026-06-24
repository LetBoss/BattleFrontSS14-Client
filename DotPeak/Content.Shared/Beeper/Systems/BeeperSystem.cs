// Decompiled with JetBrains decompiler
// Type: Content.Shared.Beeper.Systems.BeeperSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Beeper.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Beeper.Systems;

public sealed class BeeperSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedAudioSystem _audio;

  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<BeeperComponent, ItemToggleComponent> entityQueryEnumerator = this.EntityQueryEnumerator<BeeperComponent, ItemToggleComponent>();
    EntityUid owner;
    BeeperComponent beeper;
    ItemToggleComponent itemToggleComponent;
    while (entityQueryEnumerator.MoveNext(ref owner, ref beeper, ref itemToggleComponent))
    {
      if (itemToggleComponent.Activated)
        this.RunUpdate_Internal(owner, beeper);
    }
  }

  public void SetIntervalScaling(EntityUid owner, BeeperComponent beeper, FixedPoint2 newScaling)
  {
    newScaling = FixedPoint2.Clamp(newScaling, (FixedPoint2) 0, (FixedPoint2) 1);
    beeper.IntervalScaling = newScaling;
    this.RunUpdate_Internal(owner, beeper);
    this.Dirty(owner, (IComponent) beeper, (MetaDataComponent) null);
  }

  public void SetInterval(EntityUid owner, BeeperComponent beeper, TimeSpan newInterval)
  {
    if (newInterval < beeper.MinBeepInterval)
      newInterval = beeper.MinBeepInterval;
    if (newInterval > beeper.MaxBeepInterval)
      newInterval = beeper.MaxBeepInterval;
    beeper.Interval = newInterval;
    this.RunUpdate_Internal(owner, beeper);
    this.Dirty(owner, (IComponent) beeper, (MetaDataComponent) null);
  }

  public void SetIntervalScaling(EntityUid owner, FixedPoint2 newScaling, BeeperComponent? beeper = null)
  {
    if (!this.Resolve<BeeperComponent>(owner, ref beeper, true))
      return;
    this.SetIntervalScaling(owner, beeper, newScaling);
  }

  public void SetMute(EntityUid owner, bool isMuted, BeeperComponent? comp = null)
  {
    if (!this.Resolve<BeeperComponent>(owner, ref comp, true))
      return;
    comp.IsMuted = isMuted;
    this.Dirty(owner, (IComponent) comp, (MetaDataComponent) null);
  }

  private void UpdateBeepInterval(EntityUid owner, BeeperComponent beeper)
  {
    float num = beeper.IntervalScaling.Float();
    TimeSpan timeSpan = (beeper.MaxBeepInterval - beeper.MinBeepInterval) * (double) num + beeper.MinBeepInterval;
    if (beeper.Interval == timeSpan)
      return;
    beeper.Interval = timeSpan;
    this.Dirty(owner, (IComponent) beeper, (MetaDataComponent) null);
  }

  public void ForceUpdate(EntityUid owner, BeeperComponent? beeper = null)
  {
    if (!this.Resolve<BeeperComponent>(owner, ref beeper, true))
      return;
    this.RunUpdate_Internal(owner, beeper);
  }

  private void RunUpdate_Internal(EntityUid owner, BeeperComponent beeper)
  {
    if (!this._toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(owner)))
      return;
    this.UpdateBeepInterval(owner, beeper);
    if (beeper.NextBeep >= this._timing.CurTime)
      return;
    BeepPlayedEvent beepPlayedEvent = new BeepPlayedEvent(beeper.IsMuted);
    this.RaiseLocalEvent<BeepPlayedEvent>(owner, ref beepPlayedEvent, false);
    if (!beeper.IsMuted && this._net.IsServer)
      this._audio.PlayPvs(beeper.BeepSound, owner, new AudioParams?());
    beeper.LastBeepTime = this._timing.CurTime;
  }
}
