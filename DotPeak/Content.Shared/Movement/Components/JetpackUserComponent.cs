// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.JetpackUserComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class JetpackUserComponent : 
  Component,
  ISerializationGenerated<JetpackUserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Jetpack;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WeightlessAcceleration;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WeightlessFriction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WeightlessFrictionNoInput;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WeightlessModifier;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref JetpackUserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (JetpackUserComponent) target1;
    if (serialization.TryCustomCopy<JetpackUserComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Jetpack, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Jetpack, hookCtx, context);
    target.Jetpack = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessAcceleration, ref target3, hookCtx, false, context))
      target3 = this.WeightlessAcceleration;
    target.WeightlessAcceleration = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessFriction, ref target4, hookCtx, false, context))
      target4 = this.WeightlessFriction;
    target.WeightlessFriction = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessFrictionNoInput, ref target5, hookCtx, false, context))
      target5 = this.WeightlessFrictionNoInput;
    target.WeightlessFrictionNoInput = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeightlessModifier, ref target6, hookCtx, false, context))
      target6 = this.WeightlessModifier;
    target.WeightlessModifier = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref JetpackUserComponent target,
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
    JetpackUserComponent target1 = (JetpackUserComponent) target;
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
    JetpackUserComponent target1 = (JetpackUserComponent) target;
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
    JetpackUserComponent target1 = (JetpackUserComponent) target;
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
  virtual JetpackUserComponent Component.Instantiate() => new JetpackUserComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class JetpackUserComponent_AutoState : IComponentState
  {
    public NetEntity Jetpack;
    public float WeightlessAcceleration;
    public float WeightlessFriction;
    public float WeightlessFrictionNoInput;
    public float WeightlessModifier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class JetpackUserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<JetpackUserComponent, ComponentGetState>(new ComponentEventRefHandler<JetpackUserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<JetpackUserComponent, ComponentHandleState>(new ComponentEventRefHandler<JetpackUserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      JetpackUserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new JetpackUserComponent.JetpackUserComponent_AutoState()
      {
        Jetpack = this.GetNetEntity(component.Jetpack),
        WeightlessAcceleration = component.WeightlessAcceleration,
        WeightlessFriction = component.WeightlessFriction,
        WeightlessFrictionNoInput = component.WeightlessFrictionNoInput,
        WeightlessModifier = component.WeightlessModifier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      JetpackUserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is JetpackUserComponent.JetpackUserComponent_AutoState current))
        return;
      component.Jetpack = this.EnsureEntity<JetpackUserComponent>(current.Jetpack, uid);
      component.WeightlessAcceleration = current.WeightlessAcceleration;
      component.WeightlessFriction = current.WeightlessFriction;
      component.WeightlessFrictionNoInput = current.WeightlessFrictionNoInput;
      component.WeightlessModifier = current.WeightlessModifier;
    }
  }
}
