// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Weapon.DropshipAmmoComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Explosion.Implosion;
using Content.Shared.Damage;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Weapon;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedDropshipWeaponSystem)})]
public sealed class DropshipAmmoComponent : 
  Component,
  ISerializationGenerated<DropshipAmmoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int TargetSpread = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BulletSpread = 3;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public int Rounds = 400;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public int MaxRounds = 400;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public int RoundsPerShot = 40;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShotsPerVolley = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ArmorPiercing = 10;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId<DropshipWeaponComponent> Weapon = new EntProtoId<DropshipWeaponComponent>(string.Empty);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TravelTime = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SoundTravelTime = TimeSpan.FromSeconds(1.1);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundCockpit;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundMarker;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundGround;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundImpact;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? SoundWarning;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> ImpactEffects = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string? AmmoType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCExplosion? Explosion;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCImplosion? Implosion;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCFire? Fire;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SoundEveryShots = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DeleteOnEmpty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MarkerWarning;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipAmmoComponent) target1;
    if (serialization.TryCustomCopy<DropshipAmmoComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.TargetSpread, ref target2, hookCtx, false, context))
      target2 = this.TargetSpread;
    target.TargetSpread = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.BulletSpread, ref target3, hookCtx, false, context))
      target3 = this.BulletSpread;
    target.BulletSpread = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Rounds, ref target4, hookCtx, false, context))
      target4 = this.Rounds;
    target.Rounds = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxRounds, ref target5, hookCtx, false, context))
      target5 = this.MaxRounds;
    target.MaxRounds = target5;
    int target6 = 0;
    if (!serialization.TryCustomCopy<int>(this.RoundsPerShot, ref target6, hookCtx, false, context))
      target6 = this.RoundsPerShot;
    target.RoundsPerShot = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShotsPerVolley, ref target7, hookCtx, false, context))
      target7 = this.ShotsPerVolley;
    target.ShotsPerVolley = target7;
    DamageSpecifier target8 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target8, hookCtx, false, context))
    {
      if (this.Damage == null)
        target8 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target8, hookCtx, context);
    }
    target.Damage = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.ArmorPiercing, ref target9, hookCtx, false, context))
      target9 = this.ArmorPiercing;
    target.ArmorPiercing = target9;
    EntProtoId<DropshipWeaponComponent> target10 = new EntProtoId<DropshipWeaponComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<DropshipWeaponComponent>>(this.Weapon, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntProtoId<DropshipWeaponComponent>>(this.Weapon, hookCtx, context);
    target.Weapon = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TravelTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.TravelTime, hookCtx, context);
    target.TravelTime = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SoundTravelTime, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.SoundTravelTime, hookCtx, context);
    target.SoundTravelTime = target12;
    SoundSpecifier target13 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundCockpit, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<SoundSpecifier>(this.SoundCockpit, hookCtx, context);
    target.SoundCockpit = target13;
    SoundSpecifier target14 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundMarker, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<SoundSpecifier>(this.SoundMarker, hookCtx, context);
    target.SoundMarker = target14;
    SoundSpecifier target15 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundGround, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<SoundSpecifier>(this.SoundGround, hookCtx, context);
    target.SoundGround = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundImpact, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.SoundImpact, hookCtx, context);
    target.SoundImpact = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundWarning, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.SoundWarning, hookCtx, context);
    target.SoundWarning = target17;
    List<EntProtoId> target18 = (List<EntProtoId>) null;
    if (this.ImpactEffects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.ImpactEffects, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<List<EntProtoId>>(this.ImpactEffects, hookCtx, context);
    target.ImpactEffects = target18;
    string target19 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.AmmoType, ref target19, hookCtx, false, context))
      target19 = this.AmmoType;
    target.AmmoType = target19;
    RMCExplosion target20 = (RMCExplosion) null;
    if (!serialization.TryCustomCopy<RMCExplosion>(this.Explosion, ref target20, hookCtx, false, context))
    {
      if (this.Explosion == null)
        target20 = (RMCExplosion) null;
      else
        serialization.CopyTo<RMCExplosion>(this.Explosion, ref target20, hookCtx, context);
    }
    target.Explosion = target20;
    RMCImplosion target21 = (RMCImplosion) null;
    if (!serialization.TryCustomCopy<RMCImplosion>(this.Implosion, ref target21, hookCtx, false, context))
    {
      if (this.Implosion == null)
        target21 = (RMCImplosion) null;
      else
        serialization.CopyTo<RMCImplosion>(this.Implosion, ref target21, hookCtx, context);
    }
    target.Implosion = target21;
    RMCFire target22 = (RMCFire) null;
    if (!serialization.TryCustomCopy<RMCFire>(this.Fire, ref target22, hookCtx, false, context))
    {
      if (this.Fire == null)
        target22 = (RMCFire) null;
      else
        serialization.CopyTo<RMCFire>(this.Fire, ref target22, hookCtx, context);
    }
    target.Fire = target22;
    int target23 = 0;
    if (!serialization.TryCustomCopy<int>(this.SoundEveryShots, ref target23, hookCtx, false, context))
      target23 = this.SoundEveryShots;
    target.SoundEveryShots = target23;
    bool target24 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteOnEmpty, ref target24, hookCtx, false, context))
      target24 = this.DeleteOnEmpty;
    target.DeleteOnEmpty = target24;
    bool target25 = false;
    if (!serialization.TryCustomCopy<bool>(this.MarkerWarning, ref target25, hookCtx, false, context))
      target25 = this.MarkerWarning;
    target.MarkerWarning = target25;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipAmmoComponent target1 = (DropshipAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipAmmoComponent target1 = (DropshipAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DropshipAmmoComponent target1 = (DropshipAmmoComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DropshipAmmoComponent Component.Instantiate() => new DropshipAmmoComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipAmmoComponent_AutoState : IComponentState
  {
    public int TargetSpread;
    public int BulletSpread;
    public int Rounds;
    public int MaxRounds;
    public int RoundsPerShot;
    public int ShotsPerVolley;
    public DamageSpecifier? Damage;
    public int ArmorPiercing;
    public EntProtoId<DropshipWeaponComponent> Weapon;
    public TimeSpan TravelTime;
    public TimeSpan SoundTravelTime;
    public SoundSpecifier? SoundCockpit;
    public SoundSpecifier? SoundMarker;
    public SoundSpecifier? SoundGround;
    public SoundSpecifier? SoundImpact;
    public SoundSpecifier? SoundWarning;
    public List<EntProtoId> ImpactEffects;
    public string? AmmoType;
    public RMCExplosion? Explosion;
    public RMCImplosion? Implosion;
    public RMCFire? Fire;
    public int SoundEveryShots;
    public bool DeleteOnEmpty;
    public bool MarkerWarning;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipAmmoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipAmmoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipAmmoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipAmmoComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipAmmoComponent.DropshipAmmoComponent_AutoState()
      {
        TargetSpread = component.TargetSpread,
        BulletSpread = component.BulletSpread,
        Rounds = component.Rounds,
        MaxRounds = component.MaxRounds,
        RoundsPerShot = component.RoundsPerShot,
        ShotsPerVolley = component.ShotsPerVolley,
        Damage = component.Damage,
        ArmorPiercing = component.ArmorPiercing,
        Weapon = component.Weapon,
        TravelTime = component.TravelTime,
        SoundTravelTime = component.SoundTravelTime,
        SoundCockpit = component.SoundCockpit,
        SoundMarker = component.SoundMarker,
        SoundGround = component.SoundGround,
        SoundImpact = component.SoundImpact,
        SoundWarning = component.SoundWarning,
        ImpactEffects = component.ImpactEffects,
        AmmoType = component.AmmoType,
        Explosion = component.Explosion,
        Implosion = component.Implosion,
        Fire = component.Fire,
        SoundEveryShots = component.SoundEveryShots,
        DeleteOnEmpty = component.DeleteOnEmpty,
        MarkerWarning = component.MarkerWarning
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipAmmoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipAmmoComponent.DropshipAmmoComponent_AutoState current))
        return;
      component.TargetSpread = current.TargetSpread;
      component.BulletSpread = current.BulletSpread;
      component.Rounds = current.Rounds;
      component.MaxRounds = current.MaxRounds;
      component.RoundsPerShot = current.RoundsPerShot;
      component.ShotsPerVolley = current.ShotsPerVolley;
      component.Damage = current.Damage;
      component.ArmorPiercing = current.ArmorPiercing;
      component.Weapon = current.Weapon;
      component.TravelTime = current.TravelTime;
      component.SoundTravelTime = current.SoundTravelTime;
      component.SoundCockpit = current.SoundCockpit;
      component.SoundMarker = current.SoundMarker;
      component.SoundGround = current.SoundGround;
      component.SoundImpact = current.SoundImpact;
      component.SoundWarning = current.SoundWarning;
      component.ImpactEffects = current.ImpactEffects == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.ImpactEffects);
      component.AmmoType = current.AmmoType;
      component.Explosion = current.Explosion;
      component.Implosion = current.Implosion;
      component.Fire = current.Fire;
      component.SoundEveryShots = current.SoundEveryShots;
      component.DeleteOnEmpty = current.DeleteOnEmpty;
      component.MarkerWarning = current.MarkerWarning;
    }
  }
}
