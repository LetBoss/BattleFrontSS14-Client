// Decompiled with JetBrains decompiler
// Type: Content.Shared.Projectiles.ProjectileComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Projectiles;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ProjectileComponent : 
  Component,
  ISerializationGenerated<ProjectileComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle Angle;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public EntProtoId? ImpactEffect;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Shooter;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Weapon;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IgnoreShooter = true;
  [DataField(null, false, 1, true, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public DamageSpecifier Damage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  public bool DeleteOnCollide = true;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreResistances;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundHit;
  [DataField(null, false, 1, false, false, null)]
  public bool ForceSound;
  [DataField(null, false, 1, false, false, null)]
  public bool OnlyCollideWhenShot;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ProjectileSpent;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 PenetrationThreshold = FixedPoint2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public List<string>? PenetrationDamageTypeRequirement;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 PenetrationAmount = FixedPoint2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? MaxFixedRange;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProjectileComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProjectileComponent) target1;
    if (serialization.TryCustomCopy<ProjectileComponent>(this, ref target, hookCtx, false, context))
      return;
    Angle target2 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Angle, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Angle>(this.Angle, hookCtx, context);
    target.Angle = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.ImpactEffect, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.ImpactEffect, hookCtx, context);
    target.ImpactEffect = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Shooter, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Shooter, hookCtx, context);
    target.Shooter = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Weapon, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Weapon, hookCtx, context);
    target.Weapon = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreShooter, ref target6, hookCtx, false, context))
      target6 = this.IgnoreShooter;
    target.IgnoreShooter = target6;
    DamageSpecifier target7 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target7, hookCtx, false, context))
    {
      if (this.Damage == null)
        target7 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target7, hookCtx, context, true);
    }
    target.Damage = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeleteOnCollide, ref target8, hookCtx, false, context))
      target8 = this.DeleteOnCollide;
    target.DeleteOnCollide = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreResistances, ref target9, hookCtx, false, context))
      target9 = this.IgnoreResistances;
    target.IgnoreResistances = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundHit, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.SoundHit, hookCtx, context);
    target.SoundHit = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.ForceSound, ref target11, hookCtx, false, context))
      target11 = this.ForceSound;
    target.ForceSound = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnlyCollideWhenShot, ref target12, hookCtx, false, context))
      target12 = this.OnlyCollideWhenShot;
    target.OnlyCollideWhenShot = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.ProjectileSpent, ref target13, hookCtx, false, context))
      target13 = this.ProjectileSpent;
    target.ProjectileSpent = target13;
    FixedPoint2 target14 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PenetrationThreshold, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<FixedPoint2>(this.PenetrationThreshold, hookCtx, context);
    target.PenetrationThreshold = target14;
    List<string> target15 = (List<string>) null;
    if (!serialization.TryCustomCopy<List<string>>(this.PenetrationDamageTypeRequirement, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<List<string>>(this.PenetrationDamageTypeRequirement, hookCtx, context);
    target.PenetrationDamageTypeRequirement = target15;
    FixedPoint2 target16 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.PenetrationAmount, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<FixedPoint2>(this.PenetrationAmount, hookCtx, context);
    target.PenetrationAmount = target16;
    float? target17 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.MaxFixedRange, ref target17, hookCtx, false, context))
      target17 = this.MaxFixedRange;
    target.MaxFixedRange = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProjectileComponent target,
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
    ProjectileComponent target1 = (ProjectileComponent) target;
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
    ProjectileComponent target1 = (ProjectileComponent) target;
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
    ProjectileComponent target1 = (ProjectileComponent) target;
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
  virtual ProjectileComponent Component.Instantiate() => new ProjectileComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ProjectileComponent_AutoState : IComponentState
  {
    public Angle Angle;
    public NetEntity? Shooter;
    public NetEntity? Weapon;
    public bool IgnoreShooter;
    public bool ProjectileSpent;
    public float? MaxFixedRange;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ProjectileComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ProjectileComponent, ComponentGetState>(new ComponentEventRefHandler<ProjectileComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ProjectileComponent, ComponentHandleState>(new ComponentEventRefHandler<ProjectileComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ProjectileComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ProjectileComponent.ProjectileComponent_AutoState()
      {
        Angle = component.Angle,
        Shooter = this.GetNetEntity(component.Shooter),
        Weapon = this.GetNetEntity(component.Weapon),
        IgnoreShooter = component.IgnoreShooter,
        ProjectileSpent = component.ProjectileSpent,
        MaxFixedRange = component.MaxFixedRange
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ProjectileComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ProjectileComponent.ProjectileComponent_AutoState current))
        return;
      component.Angle = current.Angle;
      component.Shooter = this.EnsureEntity<ProjectileComponent>(current.Shooter, uid);
      component.Weapon = this.EnsureEntity<ProjectileComponent>(current.Weapon, uid);
      component.IgnoreShooter = current.IgnoreShooter;
      component.ProjectileSpent = current.ProjectileSpent;
      component.MaxFixedRange = current.MaxFixedRange;
    }
  }
}
