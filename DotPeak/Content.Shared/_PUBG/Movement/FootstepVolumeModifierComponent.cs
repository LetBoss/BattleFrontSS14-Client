// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Movement.FootstepVolumeModifierComponent
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
namespace Content.Shared._PUBG.Movement;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class FootstepVolumeModifierComponent : 
  Component,
  ISerializationGenerated<FootstepVolumeModifierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprintVolumeModifier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WalkVolumeModifier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprintMaxDistance;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WalkMaxDistance = 4f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FootstepVolumeModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FootstepVolumeModifierComponent) target1;
    if (serialization.TryCustomCopy<FootstepVolumeModifierComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintVolumeModifier, ref target2, hookCtx, false, context))
      target2 = this.SprintVolumeModifier;
    target.SprintVolumeModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkVolumeModifier, ref target3, hookCtx, false, context))
      target3 = this.WalkVolumeModifier;
    target.WalkVolumeModifier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintMaxDistance, ref target4, hookCtx, false, context))
      target4 = this.SprintMaxDistance;
    target.SprintMaxDistance = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkMaxDistance, ref target5, hookCtx, false, context))
      target5 = this.WalkMaxDistance;
    target.WalkMaxDistance = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FootstepVolumeModifierComponent target,
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
    FootstepVolumeModifierComponent target1 = (FootstepVolumeModifierComponent) target;
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
    FootstepVolumeModifierComponent target1 = (FootstepVolumeModifierComponent) target;
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
    FootstepVolumeModifierComponent target1 = (FootstepVolumeModifierComponent) target;
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
  virtual FootstepVolumeModifierComponent Component.Instantiate()
  {
    return new FootstepVolumeModifierComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FootstepVolumeModifierComponent_AutoState : IComponentState
  {
    public float SprintVolumeModifier;
    public float WalkVolumeModifier;
    public float SprintMaxDistance;
    public float WalkMaxDistance;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FootstepVolumeModifierComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FootstepVolumeModifierComponent, ComponentGetState>(new ComponentEventRefHandler<FootstepVolumeModifierComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FootstepVolumeModifierComponent, ComponentHandleState>(new ComponentEventRefHandler<FootstepVolumeModifierComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FootstepVolumeModifierComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FootstepVolumeModifierComponent.FootstepVolumeModifierComponent_AutoState()
      {
        SprintVolumeModifier = component.SprintVolumeModifier,
        WalkVolumeModifier = component.WalkVolumeModifier,
        SprintMaxDistance = component.SprintMaxDistance,
        WalkMaxDistance = component.WalkMaxDistance
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FootstepVolumeModifierComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FootstepVolumeModifierComponent.FootstepVolumeModifierComponent_AutoState current))
        return;
      component.SprintVolumeModifier = current.SprintVolumeModifier;
      component.WalkVolumeModifier = current.WalkVolumeModifier;
      component.SprintMaxDistance = current.SprintMaxDistance;
      component.WalkMaxDistance = current.WalkMaxDistance;
    }
  }
}
