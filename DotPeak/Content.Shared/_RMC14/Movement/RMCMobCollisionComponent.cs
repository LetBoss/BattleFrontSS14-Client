// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Movement.RMCMobCollisionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Movement;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCMovementSystem)})]
public sealed class RMCMobCollisionComponent : 
  Component,
  ISerializationGenerated<RMCMobCollisionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FixtureId = "rmc_mob_collision";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public IPhysShape FixtureShape = (IPhysShape) new PhysShapeCircle(0.4f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup FixtureLayer = CollisionGroup.MobCollision;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCMobCollisionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCMobCollisionComponent) target1;
    if (serialization.TryCustomCopy<RMCMobCollisionComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.FixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FixtureId, ref target2, hookCtx, false, context))
      target2 = this.FixtureId;
    target.FixtureId = target2;
    IPhysShape target3 = (IPhysShape) null;
    if (this.FixtureShape == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<IPhysShape>(this.FixtureShape, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<IPhysShape>(this.FixtureShape, hookCtx, context);
    target.FixtureShape = target3;
    CollisionGroup target4 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.FixtureLayer, ref target4, hookCtx, false, context))
      target4 = this.FixtureLayer;
    target.FixtureLayer = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCMobCollisionComponent target,
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
    RMCMobCollisionComponent target1 = (RMCMobCollisionComponent) target;
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
    RMCMobCollisionComponent target1 = (RMCMobCollisionComponent) target;
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
    RMCMobCollisionComponent target1 = (RMCMobCollisionComponent) target;
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
  virtual RMCMobCollisionComponent Component.Instantiate() => new RMCMobCollisionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCMobCollisionComponent_AutoState : IComponentState
  {
    public string FixtureId;
    public IPhysShape FixtureShape;
    public CollisionGroup FixtureLayer;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCMobCollisionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCMobCollisionComponent, ComponentGetState>(new ComponentEventRefHandler<RMCMobCollisionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCMobCollisionComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCMobCollisionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCMobCollisionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCMobCollisionComponent.RMCMobCollisionComponent_AutoState()
      {
        FixtureId = component.FixtureId,
        FixtureShape = component.FixtureShape,
        FixtureLayer = component.FixtureLayer
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCMobCollisionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCMobCollisionComponent.RMCMobCollisionComponent_AutoState current))
        return;
      component.FixtureId = current.FixtureId;
      component.FixtureShape = current.FixtureShape;
      component.FixtureLayer = current.FixtureLayer;
    }
  }
}
