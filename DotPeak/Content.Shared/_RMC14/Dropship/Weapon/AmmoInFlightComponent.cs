// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Weapon.AmmoInFlightComponent
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
using Robust.Shared.Map;
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
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedDropshipWeaponSystem)})]
public sealed class AmmoInFlightComponent : 
  Component,
  ISerializationGenerated<AmmoInFlightComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SpawnedMarker;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MarkerAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Marker;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextShot;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan ShotDelay = TimeSpan.FromSeconds(0.1);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShotsLeft;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ShotsPerVolley;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SoundEveryShots = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SoundShotsLeft;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan? PlayGroundSoundAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier? Damage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int ArmorPiercing = 10;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int BulletSpread = 3;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SoundTravelTime = TimeSpan.FromSeconds(1.1);
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
  public bool WarnedSound;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MarkerWarning;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? WarningMarker;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan WarningMarkerAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool WarnedMarker;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<EntProtoId> ImpactEffects = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCExplosion? Explosion;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCImplosion? Implosion;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCFire? Fire;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AmmoInFlightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AmmoInFlightComponent) target1;
    if (serialization.TryCustomCopy<AmmoInFlightComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityCoordinates target2 = new EntityCoordinates();
    if (!serialization.TryCustomCopy<EntityCoordinates>(this.Target, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityCoordinates>(this.Target, hookCtx, context);
    target.Target = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.SpawnedMarker, ref target3, hookCtx, false, context))
      target3 = this.SpawnedMarker;
    target.SpawnedMarker = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MarkerAt, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.MarkerAt, hookCtx, context);
    target.MarkerAt = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Marker, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Marker, hookCtx, context);
    target.Marker = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextShot, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextShot, hookCtx, context);
    target.NextShot = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ShotDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.ShotDelay, hookCtx, context);
    target.ShotDelay = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShotsLeft, ref target8, hookCtx, false, context))
      target8 = this.ShotsLeft;
    target.ShotsLeft = target8;
    int target9 = 0;
    if (!serialization.TryCustomCopy<int>(this.ShotsPerVolley, ref target9, hookCtx, false, context))
      target9 = this.ShotsPerVolley;
    target.ShotsPerVolley = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.SoundEveryShots, ref target10, hookCtx, false, context))
      target10 = this.SoundEveryShots;
    target.SoundEveryShots = target10;
    int target11 = 0;
    if (!serialization.TryCustomCopy<int>(this.SoundShotsLeft, ref target11, hookCtx, false, context))
      target11 = this.SoundShotsLeft;
    target.SoundShotsLeft = target11;
    TimeSpan? target12 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.PlayGroundSoundAt, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan?>(this.PlayGroundSoundAt, hookCtx, context);
    target.PlayGroundSoundAt = target12;
    DamageSpecifier target13 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target13, hookCtx, false, context))
    {
      if (this.Damage == null)
        target13 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target13, hookCtx, context);
    }
    target.Damage = target13;
    int target14 = 0;
    if (!serialization.TryCustomCopy<int>(this.ArmorPiercing, ref target14, hookCtx, false, context))
      target14 = this.ArmorPiercing;
    target.ArmorPiercing = target14;
    int target15 = 0;
    if (!serialization.TryCustomCopy<int>(this.BulletSpread, ref target15, hookCtx, false, context))
      target15 = this.BulletSpread;
    target.BulletSpread = target15;
    TimeSpan target16 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SoundTravelTime, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<TimeSpan>(this.SoundTravelTime, hookCtx, context);
    target.SoundTravelTime = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundMarker, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.SoundMarker, hookCtx, context);
    target.SoundMarker = target17;
    SoundSpecifier target18 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundGround, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<SoundSpecifier>(this.SoundGround, hookCtx, context);
    target.SoundGround = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundImpact, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.SoundImpact, hookCtx, context);
    target.SoundImpact = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundWarning, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.SoundWarning, hookCtx, context);
    target.SoundWarning = target20;
    bool target21 = false;
    if (!serialization.TryCustomCopy<bool>(this.WarnedSound, ref target21, hookCtx, false, context))
      target21 = this.WarnedSound;
    target.WarnedSound = target21;
    bool target22 = false;
    if (!serialization.TryCustomCopy<bool>(this.MarkerWarning, ref target22, hookCtx, false, context))
      target22 = this.MarkerWarning;
    target.MarkerWarning = target22;
    EntityUid? target23 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.WarningMarker, ref target23, hookCtx, false, context))
      target23 = serialization.CreateCopy<EntityUid?>(this.WarningMarker, hookCtx, context);
    target.WarningMarker = target23;
    TimeSpan target24 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WarningMarkerAt, ref target24, hookCtx, false, context))
      target24 = serialization.CreateCopy<TimeSpan>(this.WarningMarkerAt, hookCtx, context);
    target.WarningMarkerAt = target24;
    bool target25 = false;
    if (!serialization.TryCustomCopy<bool>(this.WarnedMarker, ref target25, hookCtx, false, context))
      target25 = this.WarnedMarker;
    target.WarnedMarker = target25;
    List<EntProtoId> target26 = (List<EntProtoId>) null;
    if (this.ImpactEffects == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.ImpactEffects, ref target26, hookCtx, true, context))
      target26 = serialization.CreateCopy<List<EntProtoId>>(this.ImpactEffects, hookCtx, context);
    target.ImpactEffects = target26;
    RMCExplosion target27 = (RMCExplosion) null;
    if (!serialization.TryCustomCopy<RMCExplosion>(this.Explosion, ref target27, hookCtx, false, context))
    {
      if (this.Explosion == null)
        target27 = (RMCExplosion) null;
      else
        serialization.CopyTo<RMCExplosion>(this.Explosion, ref target27, hookCtx, context);
    }
    target.Explosion = target27;
    RMCImplosion target28 = (RMCImplosion) null;
    if (!serialization.TryCustomCopy<RMCImplosion>(this.Implosion, ref target28, hookCtx, false, context))
    {
      if (this.Implosion == null)
        target28 = (RMCImplosion) null;
      else
        serialization.CopyTo<RMCImplosion>(this.Implosion, ref target28, hookCtx, context);
    }
    target.Implosion = target28;
    RMCFire target29 = (RMCFire) null;
    if (!serialization.TryCustomCopy<RMCFire>(this.Fire, ref target29, hookCtx, false, context))
    {
      if (this.Fire == null)
        target29 = (RMCFire) null;
      else
        serialization.CopyTo<RMCFire>(this.Fire, ref target29, hookCtx, context);
    }
    target.Fire = target29;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AmmoInFlightComponent target,
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
    AmmoInFlightComponent target1 = (AmmoInFlightComponent) target;
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
    AmmoInFlightComponent target1 = (AmmoInFlightComponent) target;
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
    AmmoInFlightComponent target1 = (AmmoInFlightComponent) target;
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
  virtual AmmoInFlightComponent Component.Instantiate() => new AmmoInFlightComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AmmoInFlightComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AmmoInFlightComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<AmmoInFlightComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      AmmoInFlightComponent component,
      ref EntityUnpausedEvent args)
    {
      component.WarningMarkerAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AmmoInFlightComponent_AutoState : IComponentState
  {
    public NetCoordinates Target;
    public bool SpawnedMarker;
    public TimeSpan MarkerAt;
    public NetEntity? Marker;
    public TimeSpan NextShot;
    public TimeSpan ShotDelay;
    public int ShotsLeft;
    public int ShotsPerVolley;
    public int SoundEveryShots;
    public int SoundShotsLeft;
    public TimeSpan? PlayGroundSoundAt;
    public 
    #nullable enable
    DamageSpecifier? Damage;
    public int ArmorPiercing;
    public int BulletSpread;
    public TimeSpan SoundTravelTime;
    public SoundSpecifier? SoundMarker;
    public SoundSpecifier? SoundGround;
    public SoundSpecifier? SoundImpact;
    public SoundSpecifier? SoundWarning;
    public bool WarnedSound;
    public bool MarkerWarning;
    public NetEntity? WarningMarker;
    public TimeSpan WarningMarkerAt;
    public bool WarnedMarker;
    public List<EntProtoId> ImpactEffects;
    public RMCExplosion? Explosion;
    public RMCImplosion? Implosion;
    public RMCFire? Fire;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AmmoInFlightComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<AmmoInFlightComponent, ComponentGetState>(new ComponentEventRefHandler<AmmoInFlightComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<AmmoInFlightComponent, ComponentHandleState>(new ComponentEventRefHandler<AmmoInFlightComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      AmmoInFlightComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new AmmoInFlightComponent.AmmoInFlightComponent_AutoState()
      {
        Target = this.GetNetCoordinates(component.Target),
        SpawnedMarker = component.SpawnedMarker,
        MarkerAt = component.MarkerAt,
        Marker = this.GetNetEntity(component.Marker),
        NextShot = component.NextShot,
        ShotDelay = component.ShotDelay,
        ShotsLeft = component.ShotsLeft,
        ShotsPerVolley = component.ShotsPerVolley,
        SoundEveryShots = component.SoundEveryShots,
        SoundShotsLeft = component.SoundShotsLeft,
        PlayGroundSoundAt = component.PlayGroundSoundAt,
        Damage = component.Damage,
        ArmorPiercing = component.ArmorPiercing,
        BulletSpread = component.BulletSpread,
        SoundTravelTime = component.SoundTravelTime,
        SoundMarker = component.SoundMarker,
        SoundGround = component.SoundGround,
        SoundImpact = component.SoundImpact,
        SoundWarning = component.SoundWarning,
        WarnedSound = component.WarnedSound,
        MarkerWarning = component.MarkerWarning,
        WarningMarker = this.GetNetEntity(component.WarningMarker),
        WarningMarkerAt = component.WarningMarkerAt,
        WarnedMarker = component.WarnedMarker,
        ImpactEffects = component.ImpactEffects,
        Explosion = component.Explosion,
        Implosion = component.Implosion,
        Fire = component.Fire
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AmmoInFlightComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is AmmoInFlightComponent.AmmoInFlightComponent_AutoState current))
        return;
      component.Target = this.EnsureCoordinates<AmmoInFlightComponent>(current.Target, uid);
      component.SpawnedMarker = current.SpawnedMarker;
      component.MarkerAt = current.MarkerAt;
      component.Marker = this.EnsureEntity<AmmoInFlightComponent>(current.Marker, uid);
      component.NextShot = current.NextShot;
      component.ShotDelay = current.ShotDelay;
      component.ShotsLeft = current.ShotsLeft;
      component.ShotsPerVolley = current.ShotsPerVolley;
      component.SoundEveryShots = current.SoundEveryShots;
      component.SoundShotsLeft = current.SoundShotsLeft;
      component.PlayGroundSoundAt = current.PlayGroundSoundAt;
      component.Damage = current.Damage;
      component.ArmorPiercing = current.ArmorPiercing;
      component.BulletSpread = current.BulletSpread;
      component.SoundTravelTime = current.SoundTravelTime;
      component.SoundMarker = current.SoundMarker;
      component.SoundGround = current.SoundGround;
      component.SoundImpact = current.SoundImpact;
      component.SoundWarning = current.SoundWarning;
      component.WarnedSound = current.WarnedSound;
      component.MarkerWarning = current.MarkerWarning;
      component.WarningMarker = this.EnsureEntity<AmmoInFlightComponent>(current.WarningMarker, uid);
      component.WarningMarkerAt = current.WarningMarkerAt;
      component.WarnedMarker = current.WarnedMarker;
      component.ImpactEffects = current.ImpactEffects == null ? (List<EntProtoId>) null : new List<EntProtoId>((IEnumerable<EntProtoId>) current.ImpactEffects);
      component.Explosion = current.Explosion;
      component.Implosion = current.Implosion;
      component.Fire = current.Fire;
    }
  }
}
