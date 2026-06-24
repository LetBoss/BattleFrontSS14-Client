// Decompiled with JetBrains decompiler
// Type: Content.Shared.MouseRotator.MouseRotatorComponent
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
namespace Content.Shared.MouseRotator;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MouseRotatorComponent : 
  Component,
  ISerializationGenerated<MouseRotatorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle AngleTolerance = Angle.FromDegrees(20.0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle? GoalRotation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double RotationSpeed = 3.4028234663852886E+38;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Simple4DirMode = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MouseRotatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MouseRotatorComponent) target1;
    if (serialization.TryCustomCopy<MouseRotatorComponent>(this, ref target, hookCtx, false, context))
      return;
    Angle target2 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AngleTolerance, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Angle>(this.AngleTolerance, hookCtx, context);
    target.AngleTolerance = target2;
    Angle? target3 = new Angle?();
    if (!serialization.TryCustomCopy<Angle?>(this.GoalRotation, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Angle?>(this.GoalRotation, hookCtx, context);
    target.GoalRotation = target3;
    double target4 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.RotationSpeed, ref target4, hookCtx, false, context))
      target4 = this.RotationSpeed;
    target.RotationSpeed = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Simple4DirMode, ref target5, hookCtx, false, context))
      target5 = this.Simple4DirMode;
    target.Simple4DirMode = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MouseRotatorComponent target,
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
    MouseRotatorComponent target1 = (MouseRotatorComponent) target;
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
    MouseRotatorComponent target1 = (MouseRotatorComponent) target;
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
    MouseRotatorComponent target1 = (MouseRotatorComponent) target;
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
  virtual MouseRotatorComponent Component.Instantiate() => new MouseRotatorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MouseRotatorComponent_AutoState : IComponentState
  {
    public Angle AngleTolerance;
    public Angle? GoalRotation;
    public double RotationSpeed;
    public bool Simple4DirMode;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MouseRotatorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MouseRotatorComponent, ComponentGetState>(new ComponentEventRefHandler<MouseRotatorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MouseRotatorComponent, ComponentHandleState>(new ComponentEventRefHandler<MouseRotatorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MouseRotatorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new MouseRotatorComponent.MouseRotatorComponent_AutoState()
      {
        AngleTolerance = component.AngleTolerance,
        GoalRotation = component.GoalRotation,
        RotationSpeed = component.RotationSpeed,
        Simple4DirMode = component.Simple4DirMode
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MouseRotatorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MouseRotatorComponent.MouseRotatorComponent_AutoState current))
        return;
      component.AngleTolerance = current.AngleTolerance;
      component.GoalRotation = current.GoalRotation;
      component.RotationSpeed = current.RotationSpeed;
      component.Simple4DirMode = current.Simple4DirMode;
    }
  }
}
