// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleFarSoundComponent
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
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class VehicleFarSoundComponent : 
  Component,
  ISerializationGenerated<VehicleFarSoundComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AudioRange = 55f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReferenceDistance = 3f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RolloffFactor = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float VolumeBoost = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float FilterRefreshInterval = 3f;
  public TimeSpan NextFilterRefresh;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleFarSoundComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleFarSoundComponent) target1;
    if (serialization.TryCustomCopy<VehicleFarSoundComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AudioRange, ref target2, hookCtx, false, context))
      target2 = this.AudioRange;
    target.AudioRange = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReferenceDistance, ref target3, hookCtx, false, context))
      target3 = this.ReferenceDistance;
    target.ReferenceDistance = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RolloffFactor, ref target4, hookCtx, false, context))
      target4 = this.RolloffFactor;
    target.RolloffFactor = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VolumeBoost, ref target5, hookCtx, false, context))
      target5 = this.VolumeBoost;
    target.VolumeBoost = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FilterRefreshInterval, ref target6, hookCtx, false, context))
      target6 = this.FilterRefreshInterval;
    target.FilterRefreshInterval = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleFarSoundComponent target,
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
    VehicleFarSoundComponent target1 = (VehicleFarSoundComponent) target;
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
    VehicleFarSoundComponent target1 = (VehicleFarSoundComponent) target;
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
    VehicleFarSoundComponent target1 = (VehicleFarSoundComponent) target;
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
  virtual VehicleFarSoundComponent Component.Instantiate() => new VehicleFarSoundComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleFarSoundComponent_AutoState : IComponentState
  {
    public float AudioRange;
    public float ReferenceDistance;
    public float RolloffFactor;
    public float VolumeBoost;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleFarSoundComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleFarSoundComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleFarSoundComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleFarSoundComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleFarSoundComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleFarSoundComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleFarSoundComponent.VehicleFarSoundComponent_AutoState()
      {
        AudioRange = component.AudioRange,
        ReferenceDistance = component.ReferenceDistance,
        RolloffFactor = component.RolloffFactor,
        VolumeBoost = component.VolumeBoost
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleFarSoundComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleFarSoundComponent.VehicleFarSoundComponent_AutoState current))
        return;
      component.AudioRange = current.AudioRange;
      component.ReferenceDistance = current.ReferenceDistance;
      component.RolloffFactor = current.RolloffFactor;
      component.VolumeBoost = current.VolumeBoost;
    }
  }
}
