// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.ProjectileFixedDistanceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Weapons.Ranged;
using Content.Shared._RMC14.Xenonids.Projectile;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CMGunSystem), typeof (XenoProjectileSystem), typeof (PubgGunRangeSystem)})]
public sealed class ProjectileFixedDistanceComponent : 
  Component,
  ISerializationGenerated<ProjectileFixedDistanceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan FlyEndTime;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public MapCoordinates? TargetCoordinates;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ArcProj;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProjectileFixedDistanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProjectileFixedDistanceComponent) target1;
    if (serialization.TryCustomCopy<ProjectileFixedDistanceComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FlyEndTime, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.FlyEndTime, hookCtx, context);
    target.FlyEndTime = target2;
    MapCoordinates? target3 = new MapCoordinates?();
    if (!serialization.TryCustomCopy<MapCoordinates?>(this.TargetCoordinates, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<MapCoordinates?>(this.TargetCoordinates, hookCtx, context);
    target.TargetCoordinates = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ArcProj, ref target4, hookCtx, false, context))
      target4 = this.ArcProj;
    target.ArcProj = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProjectileFixedDistanceComponent target,
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
    ProjectileFixedDistanceComponent target1 = (ProjectileFixedDistanceComponent) target;
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
    ProjectileFixedDistanceComponent target1 = (ProjectileFixedDistanceComponent) target;
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
    ProjectileFixedDistanceComponent target1 = (ProjectileFixedDistanceComponent) target;
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
  virtual ProjectileFixedDistanceComponent Component.Instantiate()
  {
    return new ProjectileFixedDistanceComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ProjectileFixedDistanceComponent_AutoState : IComponentState
  {
    public TimeSpan FlyEndTime;
    public MapCoordinates? TargetCoordinates;
    public bool ArcProj;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ProjectileFixedDistanceComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ProjectileFixedDistanceComponent, ComponentGetState>(new ComponentEventRefHandler<ProjectileFixedDistanceComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ProjectileFixedDistanceComponent, ComponentHandleState>(new ComponentEventRefHandler<ProjectileFixedDistanceComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ProjectileFixedDistanceComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ProjectileFixedDistanceComponent.ProjectileFixedDistanceComponent_AutoState()
      {
        FlyEndTime = component.FlyEndTime,
        TargetCoordinates = component.TargetCoordinates,
        ArcProj = component.ArcProj
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ProjectileFixedDistanceComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ProjectileFixedDistanceComponent.ProjectileFixedDistanceComponent_AutoState current))
        return;
      component.FlyEndTime = current.FlyEndTime;
      component.TargetCoordinates = current.TargetCoordinates;
      component.ArcProj = current.ArcProj;
    }
  }
}
