// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Systems.DamageOnHighSpeedImpactSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Components;
using Content.Shared.Effects;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Damage.Systems;

public sealed class DamageOnHighSpeedImpactSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IRobustRandom _robustRandom;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _color;
  [Dependency]
  private SharedStunSystem _stun;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageOnHighSpeedImpactComponent, StartCollideEvent>(new ComponentEventRefHandler<DamageOnHighSpeedImpactComponent, StartCollideEvent>((object) this, __methodptr(HandleCollide)), (Type[]) null, (Type[]) null);
  }

  private void HandleCollide(
    EntityUid uid,
    DamageOnHighSpeedImpactComponent component,
    ref StartCollideEvent args)
  {
    if (!args.OurFixture.Hard || !args.OtherFixture.Hard || !this.HasComp<DamageableComponent>(uid))
      return;
    float num1 = args.OurBody.LinearVelocity.Length();
    if ((double) num1 < (double) component.MinimumSpeed || component.LastHit.HasValue && (this._gameTiming.CurTime - component.LastHit.Value).TotalSeconds < (double) component.DamageCooldown)
      return;
    component.LastHit = new TimeSpan?(this._gameTiming.CurTime);
    if (RandomExtensions.Prob(this._robustRandom, component.StunChance))
      this._stun.TryStun(uid, TimeSpan.FromSeconds((double) component.StunSeconds), true);
    float num2 = component.SpeedDamageFactor * num1 / component.MinimumSpeed;
    this._damageable.TryChangeDamage(new EntityUid?(uid), component.Damage * num2);
    if (this._gameTiming.IsFirstTimePredicted)
    {
      SharedAudioSystem audio = this._audio;
      SoundSpecifier soundHit = component.SoundHit;
      EntityUid entityUid = uid;
      AudioParams audioParams = ((AudioParams) ref AudioParams.Default).WithVariation(new float?(0.125f));
      AudioParams? nullable = new AudioParams?(((AudioParams) ref audioParams).WithVolume(-0.125f));
      audio.PlayPvs(soundHit, entityUid, nullable);
    }
    SharedColorFlashEffectSystem color = this._color;
    Color red = Color.Red;
    List<EntityUid> entities = new List<EntityUid>();
    entities.Add(uid);
    Filter filter = Filter.Pvs(uid, 2f, (IEntityManager) this.EntityManager, (ISharedPlayerManager) null, (IConfigurationManager) null);
    color.RaiseEffect(red, entities, filter);
  }

  public void ChangeCollide(
    EntityUid uid,
    float minimumSpeed,
    float stunSeconds,
    float damageCooldown,
    float speedDamage,
    DamageOnHighSpeedImpactComponent? collide = null)
  {
    if (!this.Resolve<DamageOnHighSpeedImpactComponent>(uid, ref collide, false))
      return;
    collide.MinimumSpeed = minimumSpeed;
    collide.StunSeconds = stunSeconds;
    collide.DamageCooldown = damageCooldown;
    collide.SpeedDamageFactor = speedDamage;
    this.Dirty(uid, (IComponent) collide, (MetaDataComponent) null);
  }
}
