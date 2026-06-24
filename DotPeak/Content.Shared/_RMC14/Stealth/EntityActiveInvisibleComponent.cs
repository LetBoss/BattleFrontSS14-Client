// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Stealth.EntityActiveInvisibleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
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
namespace Content.Shared._RMC14.Stealth;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class EntityActiveInvisibleComponent : 
  Component,
  ISerializationGenerated<EntityActiveInvisibleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Opacity = 0.1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 EvasionModifier = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 EvasionFriendlyModifier = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DisableMobCollision;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EntityActiveInvisibleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EntityActiveInvisibleComponent) target1;
    if (serialization.TryCustomCopy<EntityActiveInvisibleComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Opacity, ref target2, hookCtx, false, context))
      target2 = this.Opacity;
    target.Opacity = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.EvasionModifier, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.EvasionModifier, hookCtx, context);
    target.EvasionModifier = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.EvasionFriendlyModifier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.EvasionFriendlyModifier, hookCtx, context);
    target.EvasionFriendlyModifier = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisableMobCollision, ref target5, hookCtx, false, context))
      target5 = this.DisableMobCollision;
    target.DisableMobCollision = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EntityActiveInvisibleComponent target,
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
    EntityActiveInvisibleComponent target1 = (EntityActiveInvisibleComponent) target;
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
    EntityActiveInvisibleComponent target1 = (EntityActiveInvisibleComponent) target;
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
    EntityActiveInvisibleComponent target1 = (EntityActiveInvisibleComponent) target;
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
  virtual EntityActiveInvisibleComponent Component.Instantiate()
  {
    return new EntityActiveInvisibleComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EntityActiveInvisibleComponent_AutoState : IComponentState
  {
    public float Opacity;
    public FixedPoint2 EvasionModifier;
    public FixedPoint2 EvasionFriendlyModifier;
    public bool DisableMobCollision;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EntityActiveInvisibleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EntityActiveInvisibleComponent, ComponentGetState>(new ComponentEventRefHandler<EntityActiveInvisibleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EntityActiveInvisibleComponent, ComponentHandleState>(new ComponentEventRefHandler<EntityActiveInvisibleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EntityActiveInvisibleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EntityActiveInvisibleComponent.EntityActiveInvisibleComponent_AutoState()
      {
        Opacity = component.Opacity,
        EvasionModifier = component.EvasionModifier,
        EvasionFriendlyModifier = component.EvasionFriendlyModifier,
        DisableMobCollision = component.DisableMobCollision
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EntityActiveInvisibleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EntityActiveInvisibleComponent.EntityActiveInvisibleComponent_AutoState current))
        return;
      component.Opacity = current.Opacity;
      component.EvasionModifier = current.EvasionModifier;
      component.EvasionFriendlyModifier = current.EvasionFriendlyModifier;
      component.DisableMobCollision = current.DisableMobCollision;
    }
  }
}
