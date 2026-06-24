// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.CameraShake.RMCCameraShakingComponent
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
namespace Content.Shared._RMC14.CameraShake;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCCameraShakeSystem)})]
public sealed class RMCCameraShakingComponent : 
  Component,
  ISerializationGenerated<RMCCameraShakingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Spacing = TimeSpan.FromSeconds(0.2);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan NextShake;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Shakes;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Strength;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCCameraShakingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCCameraShakingComponent) target1;
    if (serialization.TryCustomCopy<RMCCameraShakingComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Spacing, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Spacing, hookCtx, context);
    target.Spacing = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextShake, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.NextShake, hookCtx, context);
    target.NextShake = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Shakes, ref target4, hookCtx, false, context))
      target4 = this.Shakes;
    target.Shakes = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Strength, ref target5, hookCtx, false, context))
      target5 = this.Strength;
    target.Strength = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCCameraShakingComponent target,
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
    RMCCameraShakingComponent target1 = (RMCCameraShakingComponent) target;
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
    RMCCameraShakingComponent target1 = (RMCCameraShakingComponent) target;
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
    RMCCameraShakingComponent target1 = (RMCCameraShakingComponent) target;
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
  virtual RMCCameraShakingComponent Component.Instantiate() => new RMCCameraShakingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCCameraShakingComponent_AutoState : IComponentState
  {
    public TimeSpan Spacing;
    public TimeSpan NextShake;
    public int Shakes;
    public int Strength;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCCameraShakingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCCameraShakingComponent, ComponentGetState>(new ComponentEventRefHandler<RMCCameraShakingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCCameraShakingComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCCameraShakingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCCameraShakingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCCameraShakingComponent.RMCCameraShakingComponent_AutoState()
      {
        Spacing = component.Spacing,
        NextShake = component.NextShake,
        Shakes = component.Shakes,
        Strength = component.Strength
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCCameraShakingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCCameraShakingComponent.RMCCameraShakingComponent_AutoState current))
        return;
      component.Spacing = current.Spacing;
      component.NextShake = current.NextShake;
      component.Shakes = current.Shakes;
      component.Strength = current.Strength;
    }
  }
}
