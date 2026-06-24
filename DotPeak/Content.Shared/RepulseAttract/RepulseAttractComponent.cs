// Decompiled with JetBrains decompiler
// Type: Content.Shared.RepulseAttract.RepulseAttractComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Physics;
using Content.Shared.Whitelist;
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
namespace Content.Shared.RepulseAttract;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RepulseAttractSystem)})]
public sealed class RepulseAttractComponent : 
  Component,
  ISerializationGenerated<RepulseAttractComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CollisionGroup CollisionMask = CollisionGroup.GhostImpassable;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RepulseAttractComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RepulseAttractComponent) target1;
    if (serialization.TryCustomCopy<RepulseAttractComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref target2, hookCtx, false, context))
      target2 = this.Speed;
    target.Speed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    EntityWhitelist target4 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        target4 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref target4, hookCtx, context);
    }
    target.Whitelist = target4;
    CollisionGroup target5 = CollisionGroup.None;
    if (!serialization.TryCustomCopy<CollisionGroup>(this.CollisionMask, ref target5, hookCtx, false, context))
      target5 = this.CollisionMask;
    target.CollisionMask = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RepulseAttractComponent target,
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
    RepulseAttractComponent target1 = (RepulseAttractComponent) target;
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
    RepulseAttractComponent target1 = (RepulseAttractComponent) target;
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
    RepulseAttractComponent target1 = (RepulseAttractComponent) target;
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
  virtual RepulseAttractComponent Component.Instantiate() => new RepulseAttractComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RepulseAttractComponent_AutoState : IComponentState
  {
    public float Speed;
    public float Range;
    public EntityWhitelist? Whitelist;
    public CollisionGroup CollisionMask;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RepulseAttractComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RepulseAttractComponent, ComponentGetState>(new ComponentEventRefHandler<RepulseAttractComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RepulseAttractComponent, ComponentHandleState>(new ComponentEventRefHandler<RepulseAttractComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RepulseAttractComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RepulseAttractComponent.RepulseAttractComponent_AutoState()
      {
        Speed = component.Speed,
        Range = component.Range,
        Whitelist = component.Whitelist,
        CollisionMask = component.CollisionMask
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RepulseAttractComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RepulseAttractComponent.RepulseAttractComponent_AutoState current))
        return;
      component.Speed = current.Speed;
      component.Range = current.Range;
      component.Whitelist = current.Whitelist;
      component.CollisionMask = current.CollisionMask;
    }
  }
}
