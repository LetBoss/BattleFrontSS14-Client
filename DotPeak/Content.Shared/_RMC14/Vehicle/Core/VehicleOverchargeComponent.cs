// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleOverchargeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
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
public sealed class VehicleOverchargeComponent : 
  Component,
  ISerializationGenerated<VehicleOverchargeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float SpeedMultiplier = 1.6f;
  [DataField(null, false, 1, false, false, null)]
  public float Duration = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float Cooldown = 16f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? OverchargeSound;
  [AutoNetworkedField]
  public TimeSpan ActiveUntil;
  [AutoNetworkedField]
  public TimeSpan CooldownUntil;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleOverchargeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleOverchargeComponent) target1;
    if (serialization.TryCustomCopy<VehicleOverchargeComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedMultiplier, ref target2, hookCtx, false, context))
      target2 = this.SpeedMultiplier;
    target.SpeedMultiplier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Duration, ref target3, hookCtx, false, context))
      target3 = this.Duration;
    target.Duration = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Cooldown, ref target4, hookCtx, false, context))
      target4 = this.Cooldown;
    target.Cooldown = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.OverchargeSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.OverchargeSound, hookCtx, context);
    target.OverchargeSound = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleOverchargeComponent target,
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
    VehicleOverchargeComponent target1 = (VehicleOverchargeComponent) target;
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
    VehicleOverchargeComponent target1 = (VehicleOverchargeComponent) target;
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
    VehicleOverchargeComponent target1 = (VehicleOverchargeComponent) target;
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
  virtual VehicleOverchargeComponent Component.Instantiate() => new VehicleOverchargeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleOverchargeComponent_AutoState : IComponentState
  {
    public TimeSpan ActiveUntil;
    public TimeSpan CooldownUntil;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleOverchargeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleOverchargeComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleOverchargeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleOverchargeComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleOverchargeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleOverchargeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleOverchargeComponent.VehicleOverchargeComponent_AutoState()
      {
        ActiveUntil = component.ActiveUntil,
        CooldownUntil = component.CooldownUntil
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleOverchargeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleOverchargeComponent.VehicleOverchargeComponent_AutoState current))
        return;
      component.ActiveUntil = current.ActiveUntil;
      component.CooldownUntil = current.CooldownUntil;
    }
  }
}
