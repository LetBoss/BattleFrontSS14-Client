// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.ElectronicSystem.DropshipSpotlightComponent
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
namespace Content.Shared._RMC14.Dropship.ElectronicSystem;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class DropshipSpotlightComponent : 
  Component,
  ISerializationGenerated<DropshipSpotlightComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Radius = 15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Energy = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Softness = 5f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DropshipSpotlightComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DropshipSpotlightComponent) target1;
    if (serialization.TryCustomCopy<DropshipSpotlightComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target3, hookCtx, false, context))
      target3 = this.Radius;
    target.Radius = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Energy, ref target4, hookCtx, false, context))
      target4 = this.Energy;
    target.Energy = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Softness, ref target5, hookCtx, false, context))
      target5 = this.Softness;
    target.Softness = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DropshipSpotlightComponent target,
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
    DropshipSpotlightComponent target1 = (DropshipSpotlightComponent) target;
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
    DropshipSpotlightComponent target1 = (DropshipSpotlightComponent) target;
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
    DropshipSpotlightComponent target1 = (DropshipSpotlightComponent) target;
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
  virtual DropshipSpotlightComponent Component.Instantiate() => new DropshipSpotlightComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DropshipSpotlightComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float Radius;
    public float Energy;
    public float Softness;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DropshipSpotlightComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DropshipSpotlightComponent, ComponentGetState>(new ComponentEventRefHandler<DropshipSpotlightComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DropshipSpotlightComponent, ComponentHandleState>(new ComponentEventRefHandler<DropshipSpotlightComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DropshipSpotlightComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new DropshipSpotlightComponent.DropshipSpotlightComponent_AutoState()
      {
        Enabled = component.Enabled,
        Radius = component.Radius,
        Energy = component.Energy,
        Softness = component.Softness
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DropshipSpotlightComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DropshipSpotlightComponent.DropshipSpotlightComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
      component.Radius = current.Radius;
      component.Energy = current.Energy;
      component.Softness = current.Softness;
    }
  }
}
