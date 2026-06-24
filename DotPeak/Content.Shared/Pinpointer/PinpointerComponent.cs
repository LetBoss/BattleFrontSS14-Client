// Decompiled with JetBrains decompiler
// Type: Content.Shared.Pinpointer.PinpointerComponent
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
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Pinpointer;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedPinpointerSystem)})]
public sealed class PinpointerComponent : 
  Robust.Shared.GameObjects.Component,
  ISerializationGenerated<PinpointerComponent>,
  ISerializationGenerated
{
  [DataField("component", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? Component;
  [DataField("mediumDistance", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float MediumDistance = 16f;
  [DataField("closeDistance", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float CloseDistance = 8f;
  [DataField("reachedDistance", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ReachedDistance = 1f;
  [DataField("precision", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public double Precision = 0.09;
  [DataField("targetName", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? TargetName;
  [DataField("updateTargetName", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool UpdateTargetName;
  [DataField("canRetarget", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool CanRetarget;
  [Robust.Shared.ViewVariables.ViewVariables]
  public EntityUid? Target;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool IsActive;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public Angle ArrowAngle;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public Distance DistanceToTarget;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool HasTarget => this.DistanceToTarget != 0;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PinpointerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Robust.Shared.GameObjects.Component target1 = (Robust.Shared.GameObjects.Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PinpointerComponent) target1;
    if (serialization.TryCustomCopy<PinpointerComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Component, ref target2, hookCtx, false, context))
      target2 = this.Component;
    target.Component = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MediumDistance, ref target3, hookCtx, false, context))
      target3 = this.MediumDistance;
    target.MediumDistance = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CloseDistance, ref target4, hookCtx, false, context))
      target4 = this.CloseDistance;
    target.CloseDistance = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReachedDistance, ref target5, hookCtx, false, context))
      target5 = this.ReachedDistance;
    target.ReachedDistance = target5;
    double target6 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.Precision, ref target6, hookCtx, false, context))
      target6 = this.Precision;
    target.Precision = target6;
    string target7 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.TargetName, ref target7, hookCtx, false, context))
      target7 = this.TargetName;
    target.TargetName = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.UpdateTargetName, ref target8, hookCtx, false, context))
      target8 = this.UpdateTargetName;
    target.UpdateTargetName = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanRetarget, ref target9, hookCtx, false, context))
      target9 = this.CanRetarget;
    target.CanRetarget = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PinpointerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Robust.Shared.GameObjects.Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PinpointerComponent target1 = (PinpointerComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Robust.Shared.GameObjects.Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PinpointerComponent target1 = (PinpointerComponent) target;
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
    PinpointerComponent target1 = (PinpointerComponent) target;
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
  virtual PinpointerComponent Robust.Shared.GameObjects.Component.Instantiate()
  {
    return new PinpointerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class PinpointerComponent_AutoState : IComponentState
  {
    public bool IsActive;
    public Angle ArrowAngle;
    public Distance DistanceToTarget;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PinpointerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<PinpointerComponent, ComponentGetState>(new ComponentEventRefHandler<PinpointerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<PinpointerComponent, ComponentHandleState>(new ComponentEventRefHandler<PinpointerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      PinpointerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new PinpointerComponent.PinpointerComponent_AutoState()
      {
        IsActive = component.IsActive,
        ArrowAngle = component.ArrowAngle,
        DistanceToTarget = component.DistanceToTarget
      };
    }

    private void OnHandleState(
      EntityUid uid,
      PinpointerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is PinpointerComponent.PinpointerComponent_AutoState current))
        return;
      component.IsActive = current.IsActive;
      component.ArrowAngle = current.ArrowAngle;
      component.DistanceToTarget = current.DistanceToTarget;
    }
  }
}
