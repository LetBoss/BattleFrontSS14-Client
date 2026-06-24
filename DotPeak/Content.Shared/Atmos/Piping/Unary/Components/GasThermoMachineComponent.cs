// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Unary.Components.GasThermoMachineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Guidebook;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Piping.Unary.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class GasThermoMachineComponent : 
  Component,
  ISerializationGenerated<GasThermoMachineComponent>,
  ISerializationGenerated
{
  [DataField("inlet", false, 1, false, false, null)]
  public string InletName = "pipe";
  [DataField(null, false, 1, false, false, null)]
  [GuidebookData]
  public float HeatCapacity = 5000f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float TargetTemperature = 293.15f;
  [GuidebookData]
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public float TemperatureTolerance = 2f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool HysteresisState;
  [DataField("coefficientOfPerformance", false, 1, false, false, null)]
  public float Cp = 0.9f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [GuidebookData]
  public float MinTemperature = 73.15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [GuidebookData]
  public float MaxTemperature = 593.15f;
  [DataField(null, false, 1, false, false, null)]
  public float LastEnergyDelta;
  [DataField(null, false, 1, false, false, null)]
  [GuidebookData]
  public float EnergyLeakPercentage;
  [DataField(null, false, 1, false, false, null)]
  public bool Atmospheric;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasThermoMachineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasThermoMachineComponent) component;
    if (serialization.TryCustomCopy<GasThermoMachineComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.InletName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InletName, ref str, hookCtx, false, context))
      str = this.InletName;
    target.InletName = str;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HeatCapacity, ref num1, hookCtx, false, context))
      num1 = this.HeatCapacity;
    target.HeatCapacity = num1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TargetTemperature, ref num2, hookCtx, false, context))
      num2 = this.TargetTemperature;
    target.TargetTemperature = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TemperatureTolerance, ref num3, hookCtx, false, context))
      num3 = this.TemperatureTolerance;
    target.TemperatureTolerance = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Cp, ref num4, hookCtx, false, context))
      num4 = this.Cp;
    target.Cp = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinTemperature, ref num5, hookCtx, false, context))
      num5 = this.MinTemperature;
    target.MinTemperature = num5;
    float num6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTemperature, ref num6, hookCtx, false, context))
      num6 = this.MaxTemperature;
    target.MaxTemperature = num6;
    float num7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LastEnergyDelta, ref num7, hookCtx, false, context))
      num7 = this.LastEnergyDelta;
    target.LastEnergyDelta = num7;
    float num8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EnergyLeakPercentage, ref num8, hookCtx, false, context))
      num8 = this.EnergyLeakPercentage;
    target.EnergyLeakPercentage = num8;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Atmospheric, ref flag, hookCtx, false, context))
      flag = this.Atmospheric;
    target.Atmospheric = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasThermoMachineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasThermoMachineComponent target1 = (GasThermoMachineComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasThermoMachineComponent target1 = (GasThermoMachineComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasThermoMachineComponent target1 = (GasThermoMachineComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GasThermoMachineComponent Component.Instantiate() => new GasThermoMachineComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasThermoMachineComponent_AutoState : IComponentState
  {
    public float TargetTemperature;
    public float MinTemperature;
    public float MaxTemperature;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasThermoMachineComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasThermoMachineComponent, ComponentGetState>(new ComponentEventRefHandler<GasThermoMachineComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasThermoMachineComponent, ComponentHandleState>(new ComponentEventRefHandler<GasThermoMachineComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      GasThermoMachineComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new GasThermoMachineComponent.GasThermoMachineComponent_AutoState()
      {
        TargetTemperature = component.TargetTemperature,
        MinTemperature = component.MinTemperature,
        MaxTemperature = component.MaxTemperature
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasThermoMachineComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is GasThermoMachineComponent.GasThermoMachineComponent_AutoState current))
        return;
      component.TargetTemperature = current.TargetTemperature;
      component.MinTemperature = current.MinTemperature;
      component.MaxTemperature = current.MaxTemperature;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(((ComponentHandleState) ref args).Current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, GasThermoMachineComponent>(uid, component, ref handleStateEvent);
    }
  }
}
