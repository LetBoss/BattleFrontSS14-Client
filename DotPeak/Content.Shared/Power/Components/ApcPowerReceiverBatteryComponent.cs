// Decompiled with JetBrains decompiler
// Type: Content.Shared.Power.Components.ApcPowerReceiverBatteryComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Power.EntitySystems;
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
namespace Content.Shared.Power.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedPowerNetSystem), typeof (SharedPowerReceiverSystem)})]
public sealed class ApcPowerReceiverBatteryComponent : 
  Component,
  ISerializationGenerated<ApcPowerReceiverBatteryComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  public float IdleLoad = 5f;
  [DataField(null, false, 1, false, false, null)]
  public float BatteryRechargeRate = 50f;
  [DataField(null, false, 1, false, false, null)]
  public float BatteryRechargeEfficiency = 1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ApcPowerReceiverBatteryComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ApcPowerReceiverBatteryComponent) target1;
    if (serialization.TryCustomCopy<ApcPowerReceiverBatteryComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target2, hookCtx, false, context))
      target2 = this.Enabled;
    target.Enabled = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IdleLoad, ref target3, hookCtx, false, context))
      target3 = this.IdleLoad;
    target.IdleLoad = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BatteryRechargeRate, ref target4, hookCtx, false, context))
      target4 = this.BatteryRechargeRate;
    target.BatteryRechargeRate = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BatteryRechargeEfficiency, ref target5, hookCtx, false, context))
      target5 = this.BatteryRechargeEfficiency;
    target.BatteryRechargeEfficiency = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ApcPowerReceiverBatteryComponent target,
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
    ApcPowerReceiverBatteryComponent target1 = (ApcPowerReceiverBatteryComponent) target;
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
    ApcPowerReceiverBatteryComponent target1 = (ApcPowerReceiverBatteryComponent) target;
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
    ApcPowerReceiverBatteryComponent target1 = (ApcPowerReceiverBatteryComponent) target;
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
  virtual ApcPowerReceiverBatteryComponent Component.Instantiate()
  {
    return new ApcPowerReceiverBatteryComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ApcPowerReceiverBatteryComponent_AutoState : IComponentState
  {
    public bool Enabled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ApcPowerReceiverBatteryComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ApcPowerReceiverBatteryComponent, ComponentGetState>(new ComponentEventRefHandler<ApcPowerReceiverBatteryComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ApcPowerReceiverBatteryComponent, ComponentHandleState>(new ComponentEventRefHandler<ApcPowerReceiverBatteryComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ApcPowerReceiverBatteryComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ApcPowerReceiverBatteryComponent.ApcPowerReceiverBatteryComponent_AutoState()
      {
        Enabled = component.Enabled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ApcPowerReceiverBatteryComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ApcPowerReceiverBatteryComponent.ApcPowerReceiverBatteryComponent_AutoState current))
        return;
      component.Enabled = current.Enabled;
    }
  }
}
