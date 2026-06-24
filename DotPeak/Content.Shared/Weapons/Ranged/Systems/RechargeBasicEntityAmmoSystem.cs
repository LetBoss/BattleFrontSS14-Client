// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.RechargeBasicEntityAmmoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class RechargeBasicEntityAmmoSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _netManager;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private MetaDataSystem _metadata;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, MapInitEvent>(new ComponentEventHandler<RechargeBasicEntityAmmoComponent, MapInitEvent>(this.OnInit));
    this.SubscribeLocalEvent<RechargeBasicEntityAmmoComponent, ExaminedEvent>(new ComponentEventHandler<RechargeBasicEntityAmmoComponent, ExaminedEvent>(this.OnExamined));
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<RechargeBasicEntityAmmoComponent, BasicEntityAmmoProviderComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RechargeBasicEntityAmmoComponent, BasicEntityAmmoProviderComponent>();
    EntityUid uid;
    RechargeBasicEntityAmmoComponent comp1;
    BasicEntityAmmoProviderComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      int? nullable1 = comp2.Count;
      if (nullable1.HasValue)
      {
        nullable1 = comp2.Count;
        int? nullable2 = comp2.Capacity;
        if (!(nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue) && comp1.NextCharge.HasValue)
        {
          TimeSpan? nextCharge = comp1.NextCharge;
          TimeSpan curTime = this._timing.CurTime;
          if ((nextCharge.HasValue ? (nextCharge.GetValueOrDefault() > curTime ? 1 : 0) : 0) == 0)
          {
            if (this._gun.UpdateBasicEntityAmmoCount(uid, comp2.Count.Value + 1, comp2) && this._netManager.IsServer)
              this._audio.PlayPvs(comp1.RechargeSound, uid);
            nullable2 = comp2.Count;
            nullable1 = comp2.Capacity;
            if (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable2.HasValue == nullable1.HasValue)
            {
              comp1.NextCharge = new TimeSpan?();
              this.Dirty(uid, (IComponent) comp1);
            }
            else
            {
              comp1.NextCharge = new TimeSpan?(comp1.NextCharge.Value + TimeSpan.FromSeconds((double) comp1.RechargeCooldown));
              this.Dirty(uid, (IComponent) comp1);
            }
          }
        }
      }
    }
  }

  private void OnInit(EntityUid uid, RechargeBasicEntityAmmoComponent component, MapInitEvent args)
  {
    component.NextCharge = new TimeSpan?(this._timing.CurTime);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnExamined(
    EntityUid uid,
    RechargeBasicEntityAmmoComponent component,
    ExaminedEvent args)
  {
    if (!component.ShowExamineText)
      return;
    BasicEntityAmmoProviderComponent comp;
    if (this.TryComp<BasicEntityAmmoProviderComponent>(uid, out comp))
    {
      int? count = comp.Count;
      int? capacity = comp.Capacity;
      if (!(count.GetValueOrDefault() == capacity.GetValueOrDefault() & count.HasValue == capacity.HasValue) && component.NextCharge.HasValue)
      {
        TimeSpan? nextCharge = component.NextCharge;
        TimeSpan pauseTime = this._metadata.GetPauseTime(uid);
        TimeSpan? nullable1 = nextCharge.HasValue ? new TimeSpan?(nextCharge.GetValueOrDefault() + pauseTime) : new TimeSpan?();
        TimeSpan curTime = this._timing.CurTime;
        TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(nullable1.GetValueOrDefault() - curTime) : new TimeSpan?();
        args.PushMarkup(this.Loc.GetString("recharge-basic-entity-ammo-can-recharge", ("seconds", (object) Math.Round(nullable2.Value.TotalSeconds, 1))));
        return;
      }
    }
    args.PushMarkup(this.Loc.GetString("recharge-basic-entity-ammo-full"));
  }

  public void Reset(EntityUid uid, RechargeBasicEntityAmmoComponent? recharge = null)
  {
    if (!this.Resolve<RechargeBasicEntityAmmoComponent>(uid, ref recharge, false))
      return;
    if (recharge.NextCharge.HasValue)
    {
      TimeSpan? nextCharge = recharge.NextCharge;
      TimeSpan curTime = this._timing.CurTime;
      if ((nextCharge.HasValue ? (nextCharge.GetValueOrDefault() < curTime ? 1 : 0) : 0) == 0)
        return;
    }
    recharge.NextCharge = new TimeSpan?(this._timing.CurTime + TimeSpan.FromSeconds((double) recharge.RechargeCooldown));
    this.Dirty(uid, (IComponent) recharge);
  }
}
