// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clumsy.ClumsySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.CCVar;
using Content.Shared.Chemistry.Hypospray.Events;
using Content.Shared.Climbing.Components;
using Content.Shared.Climbing.Events;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Medical;
using Content.Shared.Popups;
using Content.Shared.Random.Helpers;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Clumsy;

public sealed class ClumsySystem : EntitySystem
{
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private INetManager _net;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClumsyComponent, SelfBeforeHyposprayInjectsEvent>(new EntityEventRefHandler<ClumsyComponent, SelfBeforeHyposprayInjectsEvent>((object) this, __methodptr(BeforeHyposprayEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClumsyComponent, SelfBeforeDefibrillatorZapsEvent>(new EntityEventRefHandler<ClumsyComponent, SelfBeforeDefibrillatorZapsEvent>((object) this, __methodptr(BeforeDefibrillatorZapsEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClumsyComponent, SelfBeforeGunShotEvent>(new EntityEventRefHandler<ClumsyComponent, SelfBeforeGunShotEvent>((object) this, __methodptr(BeforeGunShotEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClumsyComponent, CatchAttemptEvent>(new EntityEventRefHandler<ClumsyComponent, CatchAttemptEvent>((object) this, __methodptr(OnCatchAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClumsyComponent, SelfBeforeClimbEvent>(new EntityEventRefHandler<ClumsyComponent, SelfBeforeClimbEvent>((object) this, __methodptr(OnBeforeClimbEvent)), (Type[]) null, (Type[]) null);
  }

  private void BeforeHyposprayEvent(
    Entity<ClumsyComponent> ent,
    ref SelfBeforeHyposprayInjectsEvent args)
  {
    if (!ent.Comp.ClumsyHypo)
      return;
    if (new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>()
    {
      (int) this._timing.CurTick.Value,
      this.GetNetEntity(Entity<ClumsyComponent>.op_Implicit(ent), (MetaDataComponent) null).Id
    })).NextDouble() >= (double) ent.Comp.ClumsyDefaultCheck)
      return;
    args.TargetGettingInjected = args.EntityUsingHypospray;
    args.InjectMessageOverride = this.Loc.GetString(LocId.op_Implicit(ent.Comp.HypoFailedMessage));
    this._audio.PlayPredicted(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), new EntityUid?(args.EntityUsingHypospray), new AudioParams?());
  }

  private void BeforeDefibrillatorZapsEvent(
    Entity<ClumsyComponent> ent,
    ref SelfBeforeDefibrillatorZapsEvent args)
  {
    if (!ent.Comp.ClumsyDefib)
      return;
    if (new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>()
    {
      (int) this._timing.CurTick.Value,
      this.GetNetEntity(Entity<ClumsyComponent>.op_Implicit(ent), (MetaDataComponent) null).Id
    })).NextDouble() >= (double) ent.Comp.ClumsyDefaultCheck)
      return;
    args.DefibTarget = args.EntityUsingDefib;
    this._audio.PlayPvs(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), new AudioParams?());
  }

  private void OnCatchAttempt(Entity<ClumsyComponent> ent, ref CatchAttemptEvent args)
  {
    if (!ent.Comp.ClumsyCatching)
      return;
    if (new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>()
    {
      (int) this._timing.CurTick.Value,
      this.GetNetEntity(args.Item, (MetaDataComponent) null).Id
    })).NextDouble() >= (double) ent.Comp.ClumsyDefaultCheck)
      return;
    args.Cancelled = true;
    if (ent.Comp.CatchingFailDamage != null)
      this._damageable.TryChangeDamage(new EntityUid?(Entity<ClumsyComponent>.op_Implicit(ent)), ent.Comp.CatchingFailDamage, origin: new EntityUid?(args.Item));
    if (this._net.IsClient)
      return;
    string message1 = this.Loc.GetString(LocId.op_Implicit(ent.Comp.CatchingFailedMessageSelf), ("item", (object) ent.Owner), ("catcher", (object) Identity.Entity(ent.Owner, (IEntityManager) this.EntityManager)));
    string message2 = this.Loc.GetString(LocId.op_Implicit(ent.Comp.CatchingFailedMessageOthers), ("item", (object) ent.Owner), ("catcher", (object) Identity.Entity(ent.Owner, (IEntityManager) this.EntityManager)));
    this._popup.PopupEntity(message1, ent.Owner, ent.Owner);
    this._popup.PopupEntity(message2, ent.Owner, Filter.PvsExcept(ent.Owner, 2f, (IEntityManager) null), true);
    this._audio.PlayPvs(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), new AudioParams?());
  }

  private void BeforeGunShotEvent(Entity<ClumsyComponent> ent, ref SelfBeforeGunShotEvent args)
  {
    if (!ent.Comp.ClumsyGuns || args.Gun.Comp.ClumsyProof)
      return;
    if (new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>()
    {
      (int) this._timing.CurTick.Value,
      this.GetNetEntity(Entity<GunComponent>.op_Implicit(args.Gun), (MetaDataComponent) null).Id
    })).NextDouble() >= (double) ent.Comp.ClumsyDefaultCheck)
      return;
    if (ent.Comp.GunShootFailDamage != null)
      this._damageable.TryChangeDamage(new EntityUid?(Entity<ClumsyComponent>.op_Implicit(ent)), ent.Comp.GunShootFailDamage, origin: new EntityUid?(Entity<ClumsyComponent>.op_Implicit(ent)));
    this._stun.TryParalyze(Entity<ClumsyComponent>.op_Implicit(ent), ent.Comp.GunShootFailStunTime, true);
    this._audio.PlayPvs(ent.Comp.GunShootFailSound, Entity<ClumsyComponent>.op_Implicit(ent), new AudioParams?());
    this._audio.PlayPvs(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), new AudioParams?());
    this._popup.PopupEntity(this.Loc.GetString(LocId.op_Implicit(ent.Comp.GunFailedMessage)), Entity<ClumsyComponent>.op_Implicit(ent), Entity<ClumsyComponent>.op_Implicit(ent));
    args.Cancel();
  }

  private void OnBeforeClimbEvent(Entity<ClumsyComponent> ent, ref SelfBeforeClimbEvent args)
  {
    if (!ent.Comp.ClumsyVaulting)
      return;
    System.Random random = new System.Random(SharedRandomExtensions.HashCodeCombine(new List<int>()
    {
      (int) this._timing.CurTick.Value,
      this.GetNetEntity(Entity<ClumsyComponent>.op_Implicit(ent), (MetaDataComponent) null).Id
    }));
    if (!this._cfg.GetCVar<bool>(CCVars.GameTableBonk) && random.NextDouble() >= (double) ent.Comp.ClumsyDefaultCheck)
      return;
    this.HitHeadClumsy(ent, Entity<ClimbableComponent>.op_Implicit(args.BeingClimbedOn));
    this._audio.PlayPredicted(ent.Comp.ClumsySound, Entity<ClumsyComponent>.op_Implicit(ent), new EntityUid?(Entity<ClumsyComponent>.op_Implicit(ent)), new AudioParams?());
    this._audio.PlayPredicted((SoundSpecifier) ent.Comp.TableBonkSound, Entity<ClumsyComponent>.op_Implicit(ent), new EntityUid?(Entity<ClumsyComponent>.op_Implicit(ent)), new AudioParams?());
    EntityUid entityUid1 = Identity.Entity(args.GettingPutOnTable, (IEntityManager) this.EntityManager);
    EntityUid entityUid2 = Identity.Entity(args.PuttingOnTable, (IEntityManager) this.EntityManager);
    if (EntityUid.op_Equality(args.PuttingOnTable, ent.Owner))
      this._popup.PopupPredicted(this.Loc.GetString(LocId.op_Implicit(ent.Comp.VaulingFailedMessageSelf), ("bonkable", (object) args.BeingClimbedOn)), this.Loc.GetString(LocId.op_Implicit(ent.Comp.VaulingFailedMessageOthers), ("victim", (object) entityUid1), ("bonkable", (object) args.BeingClimbedOn)), Entity<ClumsyComponent>.op_Implicit(ent), new EntityUid?(Entity<ClumsyComponent>.op_Implicit(ent)));
    else
      this._popup.PopupPredicted(this.Loc.GetString(LocId.op_Implicit(ent.Comp.VaulingFailedMessageForced), new (string, object)[3]
      {
        ("bonker", (object) entityUid2),
        ("victim", (object) entityUid1),
        ("bonkable", (object) args.BeingClimbedOn)
      }), Entity<ClumsyComponent>.op_Implicit(ent), new EntityUid?());
    args.Cancel();
  }

  public void HitHeadClumsy(Entity<ClumsyComponent> target, EntityUid table)
  {
    TimeSpan time = target.Comp.ClumsyDefaultStunTime;
    BonkableComponent bonkableComponent;
    if (this.TryComp<BonkableComponent>(table, ref bonkableComponent))
    {
      time = bonkableComponent.BonkTime;
      if (bonkableComponent.BonkDamage != null)
        this._damageable.TryChangeDamage(new EntityUid?(Entity<ClumsyComponent>.op_Implicit(target)), bonkableComponent.BonkDamage, true);
    }
    this._stun.TryParalyze(Entity<ClumsyComponent>.op_Implicit(target), time, true);
  }
}
