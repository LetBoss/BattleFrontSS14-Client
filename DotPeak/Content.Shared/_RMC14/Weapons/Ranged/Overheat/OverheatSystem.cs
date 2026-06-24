// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Overheat.OverheatSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Overheat;

public sealed class OverheatSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IGameTiming _time;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<OverheatComponent, GunShotEvent>(new EntityEventRefHandler<OverheatComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<OverheatComponent, AttemptShootEvent>(new EntityEventRefHandler<OverheatComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<OverheatComponent, TryGainHeatEvent>(new EntityEventRefHandler<OverheatComponent, TryGainHeatEvent>(this.OnTryGainHeat));
    this.SubscribeLocalEvent<OverheatComponent, OverheatedEvent>(new EntityEventRefHandler<OverheatComponent, OverheatedEvent>(this.OnOverheated));
  }

  private void OnAttemptShoot(Entity<OverheatComponent> ent, ref AttemptShootEvent args)
  {
    if (!ent.Comp.OverHeated)
      return;
    args.Cancelled = true;
  }

  private void OnGunShot(Entity<OverheatComponent> ent, ref GunShotEvent args)
  {
    TryGainHeatEvent args1 = new TryGainHeatEvent(ent.Comp.HeatPerShot);
    this.RaiseLocalEvent<TryGainHeatEvent>((EntityUid) ent, ref args1);
  }

  private void OnTryGainHeat(Entity<OverheatComponent> ent, ref TryGainHeatEvent args)
  {
    ent.Comp.Heat = MathF.Max(0.0f, ent.Comp.Heat + args.HeatGained);
    this.Dirty<OverheatComponent>(ent);
    HeatGainedEvent args1 = new HeatGainedEvent(ent.Comp.Heat);
    this.RaiseLocalEvent<HeatGainedEvent>((EntityUid) ent, ref args1);
    if ((double) ent.Comp.Heat < (double) ent.Comp.MaxHeat)
      return;
    OverheatedEvent args2 = new OverheatedEvent(true, ent.Comp.Damage);
    this.RaiseLocalEvent<OverheatedEvent>((EntityUid) ent, ref args2);
  }

  private void OnOverheated(Entity<OverheatComponent> ent, ref OverheatedEvent args)
  {
    if (!args.OverHeated)
    {
      ent.Comp.OverHeated = false;
      TryGainHeatEvent args1 = new TryGainHeatEvent(ent.Comp.Heat * ent.Comp.EmergencyCooldownMultiplier - ent.Comp.Heat);
      this.RaiseLocalEvent<TryGainHeatEvent>((EntityUid) ent, ref args1);
    }
    else
    {
      ent.Comp.OverHeated = true;
      ent.Comp.OverHeatedAt = this._time.CurTime;
      if (this._net.IsServer)
        this._audio.PlayPvs(ent.Comp.OverheatSound, (EntityUid) ent);
    }
    this.Dirty<OverheatComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<OverheatComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OverheatComponent>();
    EntityUid uid;
    OverheatComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if ((double) comp1.Heat != 0.0)
      {
        if (!comp1.OverHeated)
        {
          TryGainHeatEvent args = new TryGainHeatEvent((float) -((double) comp1.CooldownRate * (double) frameTime));
          this.RaiseLocalEvent<TryGainHeatEvent>(uid, ref args);
        }
        else if (this._time.CurTime > comp1.OverHeatedAt + comp1.EmergencyCooldownDelay)
        {
          OverheatedEvent args = new OverheatedEvent(false);
          this.RaiseLocalEvent<OverheatedEvent>(uid, ref args);
        }
      }
    }
  }
}
