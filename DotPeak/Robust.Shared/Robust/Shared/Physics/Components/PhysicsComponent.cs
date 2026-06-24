// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Components.PhysicsComponent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PhysicsComponent : 
  Component,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated,
  ISerializationGenerated<IComponentDelta>,
  ISerializationGenerated<PhysicsComponent>
{
  [Access(new Type[] {typeof (SharedPhysicsSystem)})]
  public bool Island;
  [Access(new Type[] {typeof (SharedPhysicsSystem)})]
  public Dictionary<int, int> IslandIndex = new Dictionary<int, int>();
  internal readonly LinkedList<Contact> Contacts = new LinkedList<Contact>();
  [DataField(null, false, 1, false, false, null)]
  public bool IgnorePaused;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public BodyType BodyType = BodyType.Static;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public bool Awake;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public bool SleepingAllowed = true;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float SleepTime;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public bool CanCollide = true;
  internal float _mass;
  internal float _invMass;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  internal float _inertia;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float InvI;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public bool FixedRotation = true;
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  internal Vector2 _localCenter = Vector2.Zero;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public Vector2 Force;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Torque;
  internal float _friction;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float LinearDamping = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float AngularDamping = 0.2f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.ReadExecute)]
  public Vector2 LinearVelocity;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.ReadExecute)]
  public float AngularVelocity;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)})]
  public bool IgnoreGravity;
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedPhysicsSystem)})]
  public bool Predict;

  public GameTick LastFieldUpdate { get; set; }

  public GameTick[] LastModifiedFields { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int ContactCount => this.Contacts.Count;

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedPhysicsSystem), typeof (FixtureSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public bool Hard { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedPhysicsSystem), typeof (FixtureSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public int CollisionLayer { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedPhysicsSystem), typeof (FixtureSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public int CollisionMask { get; internal set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float FixturesMass => this._mass;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public float Mass
  {
    get
    {
      return (this.BodyType & (BodyType.KinematicController | BodyType.Dynamic)) == BodyType.Kinematic ? 0.0f : this._mass;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float InvMass
  {
    get
    {
      return (this.BodyType & (BodyType.KinematicController | BodyType.Dynamic)) == BodyType.Kinematic ? 0.0f : this._invMass;
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float Inertia
  {
    get => this._inertia + this._mass * Vector2.Dot(this._localCenter, this._localCenter);
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 LocalCenter => this._localCenter;

  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public float Friction => this._friction;

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 Momentum => this.LinearVelocity * this.Mass;

  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedPhysicsSystem)}, Friend = AccessPermissions.ReadWriteExecute, Other = AccessPermissions.Read)]
  public BodyStatus BodyStatus { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PhysicsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PhysicsComponent) target1;
    if (serialization.TryCustomCopy<PhysicsComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnorePaused, ref target2, hookCtx, false, context))
      target2 = this.IgnorePaused;
    target.IgnorePaused = target2;
    BodyType target3 = BodyType.Kinematic;
    if (!serialization.TryCustomCopy<BodyType>(this.BodyType, ref target3, hookCtx, false, context))
      target3 = this.BodyType;
    target.BodyType = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.SleepingAllowed, ref target4, hookCtx, false, context))
      target4 = this.SleepingAllowed;
    target.SleepingAllowed = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SleepTime, ref target5, hookCtx, false, context))
      target5 = this.SleepTime;
    target.SleepTime = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanCollide, ref target6, hookCtx, false, context))
      target6 = this.CanCollide;
    target.CanCollide = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.FixedRotation, ref target7, hookCtx, false, context))
      target7 = this.FixedRotation;
    target.FixedRotation = target7;
    Vector2 target8 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Force, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2>(this.Force, hookCtx, context);
    target.Force = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Torque, ref target9, hookCtx, false, context))
      target9 = this.Torque;
    target.Torque = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LinearDamping, ref target10, hookCtx, false, context))
      target10 = this.LinearDamping;
    target.LinearDamping = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AngularDamping, ref target11, hookCtx, false, context))
      target11 = this.AngularDamping;
    target.AngularDamping = target11;
    BodyStatus target12 = BodyStatus.OnGround;
    if (!serialization.TryCustomCopy<BodyStatus>(this.BodyStatus, ref target12, hookCtx, false, context))
      target12 = this.BodyStatus;
    target.BodyStatus = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreGravity, ref target13, hookCtx, false, context))
      target13 = this.IgnoreGravity;
    target.IgnoreGravity = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PhysicsComponent target,
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
    PhysicsComponent target1 = (PhysicsComponent) target;
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
    PhysicsComponent target1 = (PhysicsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PhysicsComponent target1 = (PhysicsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PhysicsComponent target1 = (PhysicsComponent) target;
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
  virtual PhysicsComponent Component.Instantiate() => new PhysicsComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }
}
