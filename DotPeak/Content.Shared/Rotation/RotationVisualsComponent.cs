// Decompiled with JetBrains decompiler
// Type: Content.Shared.Rotation.RotationVisualsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Rotation;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RotationVisualsComponent : 
  Component,
  ISerializationGenerated<RotationVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Angle DefaultRotation = Angle.FromDegrees(90.0);
  [DataField(null, false, 1, false, false, null)]
  public Angle VerticalRotation = Angle.op_Implicit(0.0f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle HorizontalRotation = Angle.FromDegrees(90.0);
  [DataField(null, false, 1, false, false, null)]
  public float AnimationTime = 0.125f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RotationVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RotationVisualsComponent) target1;
    if (serialization.TryCustomCopy<RotationVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    Angle target2 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.DefaultRotation, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Angle>(this.DefaultRotation, hookCtx, context);
    target.DefaultRotation = target2;
    Angle target3 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.VerticalRotation, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Angle>(this.VerticalRotation, hookCtx, context);
    target.VerticalRotation = target3;
    Angle target4 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.HorizontalRotation, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Angle>(this.HorizontalRotation, hookCtx, context);
    target.HorizontalRotation = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AnimationTime, ref target5, hookCtx, false, context))
      target5 = this.AnimationTime;
    target.AnimationTime = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RotationVisualsComponent target,
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
    RotationVisualsComponent target1 = (RotationVisualsComponent) target;
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
    RotationVisualsComponent target1 = (RotationVisualsComponent) target;
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
    RotationVisualsComponent target1 = (RotationVisualsComponent) target;
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
  virtual RotationVisualsComponent Component.Instantiate() => new RotationVisualsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RotationVisualsComponent_AutoState : IComponentState
  {
    public Angle HorizontalRotation;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RotationVisualsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RotationVisualsComponent, ComponentGetState>(new ComponentEventRefHandler<RotationVisualsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RotationVisualsComponent, ComponentHandleState>(new ComponentEventRefHandler<RotationVisualsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RotationVisualsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RotationVisualsComponent.RotationVisualsComponent_AutoState()
      {
        HorizontalRotation = component.HorizontalRotation
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RotationVisualsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RotationVisualsComponent.RotationVisualsComponent_AutoState current))
        return;
      component.HorizontalRotation = current.HorizontalRotation;
    }
  }
}
