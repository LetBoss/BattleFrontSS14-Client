// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.UserDamageOverTimeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Damage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCDamageableSystem)})]
public sealed class UserDamageOverTimeComponent : 
  Component,
  ISerializationGenerated<UserDamageOverTimeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DamageEvery = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextDamageAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup Collision = CollisionGroup.LargeMobLayer | CollisionGroup.Impassable;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref UserDamageOverTimeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (UserDamageOverTimeComponent) target1;
    if (serialization.TryCustomCopy<UserDamageOverTimeComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DamageEvery, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.DamageEvery, hookCtx, context);
    target.DamageEvery = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextDamageAt, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.NextDamageAt, hookCtx, context);
    target.NextDamageAt = target3;
    CollisionGroup target4 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.Collision, ref target4, hookCtx, false, context))
      target4 = this.Collision;
    target.Collision = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref UserDamageOverTimeComponent target,
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
    UserDamageOverTimeComponent target1 = (UserDamageOverTimeComponent) target;
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
    UserDamageOverTimeComponent target1 = (UserDamageOverTimeComponent) target;
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
    UserDamageOverTimeComponent target1 = (UserDamageOverTimeComponent) target;
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
  virtual UserDamageOverTimeComponent Component.Instantiate() => new UserDamageOverTimeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class UserDamageOverTimeComponent_AutoState : IComponentState
  {
    public TimeSpan DamageEvery;
    public TimeSpan NextDamageAt;
    public CollisionGroup Collision;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class UserDamageOverTimeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<UserDamageOverTimeComponent, ComponentGetState>(new ComponentEventRefHandler<UserDamageOverTimeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<UserDamageOverTimeComponent, ComponentHandleState>(new ComponentEventRefHandler<UserDamageOverTimeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      UserDamageOverTimeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new UserDamageOverTimeComponent.UserDamageOverTimeComponent_AutoState()
      {
        DamageEvery = component.DamageEvery,
        NextDamageAt = component.NextDamageAt,
        Collision = component.Collision
      };
    }

    private void OnHandleState(
      EntityUid uid,
      UserDamageOverTimeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is UserDamageOverTimeComponent.UserDamageOverTimeComponent_AutoState current))
        return;
      component.DamageEvery = current.DamageEvery;
      component.NextDamageAt = current.NextDamageAt;
      component.Collision = current.Collision;
    }
  }
}
