// Decompiled with JetBrains decompiler
// Type: Content.Shared.SSDIndicator.SSDIndicatorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CCVar;
using Content.Shared.StatusEffectNew;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.SSDIndicator;

public sealed class SSDIndicatorSystem : EntitySystem
{
  public static readonly EntProtoId StatusEffectSSDSleeping = (EntProtoId) nameof (StatusEffectSSDSleeping);
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedStatusEffectsSystem _statusEffects;
  private bool _icSsdSleep;
  private float _icSsdSleepTime;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SSDIndicatorComponent, PlayerAttachedEvent>(new ComponentEventHandler<SSDIndicatorComponent, PlayerAttachedEvent>(this.OnPlayerAttached));
    this.SubscribeLocalEvent<SSDIndicatorComponent, PlayerDetachedEvent>(new ComponentEventHandler<SSDIndicatorComponent, PlayerDetachedEvent>(this.OnPlayerDetached));
    this.SubscribeLocalEvent<SSDIndicatorComponent, MapInitEvent>(new ComponentEventHandler<SSDIndicatorComponent, MapInitEvent>(this.OnMapInit));
    this._cfg.OnValueChanged<bool>(CCVars.ICSSDSleep, (Action<bool>) (obj => this._icSsdSleep = obj), true);
    this._cfg.OnValueChanged<float>(CCVars.ICSSDSleepTime, (Action<float>) (obj => this._icSsdSleepTime = obj), true);
  }

  private void OnPlayerAttached(
    EntityUid uid,
    SSDIndicatorComponent component,
    PlayerAttachedEvent args)
  {
    component.IsSSD = false;
    if (this._icSsdSleep)
    {
      component.FallAsleepTime = TimeSpan.Zero;
      this._statusEffects.TryRemoveStatusEffect(uid, SSDIndicatorSystem.StatusEffectSSDSleeping);
    }
    this.Dirty(uid, (IComponent) component);
  }

  private void OnPlayerDetached(
    EntityUid uid,
    SSDIndicatorComponent component,
    PlayerDetachedEvent args)
  {
    component.IsSSD = true;
    if (this._icSsdSleep)
      component.FallAsleepTime = this._timing.CurTime + TimeSpan.FromSeconds((double) this._icSsdSleepTime);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnMapInit(EntityUid uid, SSDIndicatorComponent component, MapInitEvent args)
  {
    if (!this._icSsdSleep || !component.IsSSD || !(component.FallAsleepTime == TimeSpan.Zero))
      return;
    component.FallAsleepTime = this._timing.CurTime + TimeSpan.FromSeconds((double) this._icSsdSleepTime);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._icSsdSleep)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<SSDIndicatorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SSDIndicatorComponent>();
    EntityUid uid;
    SSDIndicatorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.IsSSD && comp1.FallAsleepTime <= this._timing.CurTime && !this.TerminatingOrDeleted(uid))
        this._statusEffects.TrySetStatusEffectDuration(uid, SSDIndicatorSystem.StatusEffectSSDSleeping);
    }
  }
}
