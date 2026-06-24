// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.FrictionContactsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Systems;
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

[NetworkedComponent]
[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (FrictionContactsSystem)})]
public sealed class FrictionContactsComponent : 
  Component,
  ISerializationGenerated<FrictionContactsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AffectAirborne;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MobFriction = 0.05f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? MobFrictionNoInput = new float?(0.05f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MobAcceleration = 0.1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FrictionContactsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FrictionContactsComponent) target1;
    if (serialization.TryCustomCopy<FrictionContactsComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.AffectAirborne, ref target2, hookCtx, false, context))
      target2 = this.AffectAirborne;
    target.AffectAirborne = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MobFriction, ref target3, hookCtx, false, context))
      target3 = this.MobFriction;
    target.MobFriction = target3;
    float? target4 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.MobFrictionNoInput, ref target4, hookCtx, false, context))
      target4 = this.MobFrictionNoInput;
    target.MobFrictionNoInput = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MobAcceleration, ref target5, hookCtx, false, context))
      target5 = this.MobAcceleration;
    target.MobAcceleration = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FrictionContactsComponent target,
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
    FrictionContactsComponent target1 = (FrictionContactsComponent) target;
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
    FrictionContactsComponent target1 = (FrictionContactsComponent) target;
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
    FrictionContactsComponent target1 = (FrictionContactsComponent) target;
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
  virtual FrictionContactsComponent Component.Instantiate() => new FrictionContactsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FrictionContactsComponent_AutoState : IComponentState
  {
    public bool AffectAirborne;
    public float MobFriction;
    public float? MobFrictionNoInput;
    public float MobAcceleration;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FrictionContactsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FrictionContactsComponent, ComponentGetState>(new ComponentEventRefHandler<FrictionContactsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FrictionContactsComponent, ComponentHandleState>(new ComponentEventRefHandler<FrictionContactsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FrictionContactsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FrictionContactsComponent.FrictionContactsComponent_AutoState()
      {
        AffectAirborne = component.AffectAirborne,
        MobFriction = component.MobFriction,
        MobFrictionNoInput = component.MobFrictionNoInput,
        MobAcceleration = component.MobAcceleration
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FrictionContactsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FrictionContactsComponent.FrictionContactsComponent_AutoState current))
        return;
      component.AffectAirborne = current.AffectAirborne;
      component.MobFriction = current.MobFriction;
      component.MobFrictionNoInput = current.MobFrictionNoInput;
      component.MobAcceleration = current.MobAcceleration;
    }
  }
}
