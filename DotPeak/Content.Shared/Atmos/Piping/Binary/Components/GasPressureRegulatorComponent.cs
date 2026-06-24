// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Piping.Binary.Components.GasPressureRegulatorComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Piping.Binary.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, true)]
[AutoGenerateComponentPause]
public sealed class GasPressureRegulatorComponent : 
  Component,
  ISerializationGenerated<GasPressureRegulatorComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Enabled;
  [DataField(null, false, 1, false, false, null)]
  public string InletName = "inlet";
  [DataField(null, false, 1, false, false, null)]
  public string OutletName = "outlet";
  [GuidebookData]
  [DataField(null, false, 1, false, false, null)]
  public float MaxTransferRate = 200f;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextUiUpdate = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Threshold;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1L);
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FlowRate;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float InletPressure;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float OutletPressure;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GasPressureRegulatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (GasPressureRegulatorComponent) component;
    if (serialization.TryCustomCopy<GasPressureRegulatorComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag, hookCtx, false, context))
      flag = this.Enabled;
    target.Enabled = flag;
    string str1 = (string) null;
    if (this.InletName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.InletName, ref str1, hookCtx, false, context))
      str1 = this.InletName;
    target.InletName = str1;
    string str2 = (string) null;
    if (this.OutletName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OutletName, ref str2, hookCtx, false, context))
      str2 = this.OutletName;
    target.OutletName = str2;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTransferRate, ref num1, hookCtx, false, context))
      num1 = this.MaxTransferRate;
    target.MaxTransferRate = num1;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUiUpdate, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextUiUpdate, hookCtx, context, false);
    target.NextUiUpdate = timeSpan1;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Threshold, ref num2, hookCtx, false, context))
      num2 = this.Threshold;
    target.Threshold = num2;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateInterval, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.UpdateInterval, hookCtx, context, false);
    target.UpdateInterval = timeSpan2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FlowRate, ref num3, hookCtx, false, context))
      num3 = this.FlowRate;
    target.FlowRate = num3;
    float num4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InletPressure, ref num4, hookCtx, false, context))
      num4 = this.InletPressure;
    target.InletPressure = num4;
    float num5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OutletPressure, ref num5, hookCtx, false, context))
      num5 = this.OutletPressure;
    target.OutletPressure = num5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GasPressureRegulatorComponent target,
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
    GasPressureRegulatorComponent target1 = (GasPressureRegulatorComponent) target;
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
    GasPressureRegulatorComponent target1 = (GasPressureRegulatorComponent) target;
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
    GasPressureRegulatorComponent target1 = (GasPressureRegulatorComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GasPressureRegulatorComponent target1 = (GasPressureRegulatorComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponentDelta) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GasPressureRegulatorComponent Component.Instantiate()
  {
    return new GasPressureRegulatorComponent();
  }

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasPressureRegulatorComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasPressureRegulatorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<GasPressureRegulatorComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      GasPressureRegulatorComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUiUpdate += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GasPressureRegulatorComponent_AutoState : IComponentState
  {
    public bool Enabled;
    public float Threshold;
    public float FlowRate;
    public float InletPressure;
    public float OutletPressure;

    public 
    #nullable enable
    GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState ShallowClone()
    {
      return new GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState()
      {
        Enabled = this.Enabled,
        Threshold = this.Threshold,
        FlowRate = this.FlowRate,
        InletPressure = this.InletPressure,
        OutletPressure = this.OutletPressure
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GasPressureRegulatorComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<GasPressureRegulatorComponent>(new string[5]
      {
        "Enabled",
        "Threshold",
        "FlowRate",
        "InletPressure",
        "OutletPressure"
      });
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasPressureRegulatorComponent, ComponentGetState>(new ComponentEventRefHandler<GasPressureRegulatorComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<GasPressureRegulatorComponent, ComponentHandleState>(new ComponentEventRefHandler<GasPressureRegulatorComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      GasPressureRegulatorComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta icomponentDelta = (IComponentDelta) component;
      if (icomponentDelta != null && GameTick.op_GreaterThan(((ComponentGetState) ref args).FromTick, component.CreationTick) && GameTick.op_GreaterThanOrEqual(icomponentDelta.LastFieldUpdate, ((ComponentGetState) ref args).FromTick))
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, ((ComponentGetState) ref args).FromTick))
        {
          case 1:
            ((ComponentGetState) ref args).State = (IComponentState) new GasPressureRegulatorComponent.Enabled_FieldComponentState()
            {
              Enabled = component.Enabled
            };
            return;
          case 2:
            ((ComponentGetState) ref args).State = (IComponentState) new GasPressureRegulatorComponent.Threshold_FieldComponentState()
            {
              Threshold = component.Threshold
            };
            return;
          case 4:
            ((ComponentGetState) ref args).State = (IComponentState) new GasPressureRegulatorComponent.FlowRate_FieldComponentState()
            {
              FlowRate = component.FlowRate
            };
            return;
          case 8:
            ((ComponentGetState) ref args).State = (IComponentState) new GasPressureRegulatorComponent.InletPressure_FieldComponentState()
            {
              InletPressure = component.InletPressure
            };
            return;
          case 16 /*0x10*/:
            ((ComponentGetState) ref args).State = (IComponentState) new GasPressureRegulatorComponent.OutletPressure_FieldComponentState()
            {
              OutletPressure = component.OutletPressure
            };
            return;
        }
      }
      ((ComponentGetState) ref args).State = (IComponentState) new GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState()
      {
        Enabled = component.Enabled,
        Threshold = component.Threshold,
        FlowRate = component.FlowRate,
        InletPressure = component.InletPressure,
        OutletPressure = component.OutletPressure
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GasPressureRegulatorComponent component,
      ref ComponentHandleState args)
    {
      switch (((ComponentHandleState) ref args).Current)
      {
        case GasPressureRegulatorComponent.Enabled_FieldComponentState fieldComponentState1:
          component.Enabled = fieldComponentState1.Enabled;
          break;
        case GasPressureRegulatorComponent.Threshold_FieldComponentState fieldComponentState2:
          component.Threshold = fieldComponentState2.Threshold;
          break;
        case GasPressureRegulatorComponent.FlowRate_FieldComponentState fieldComponentState3:
          component.FlowRate = fieldComponentState3.FlowRate;
          break;
        case GasPressureRegulatorComponent.InletPressure_FieldComponentState fieldComponentState4:
          component.InletPressure = fieldComponentState4.InletPressure;
          break;
        case GasPressureRegulatorComponent.OutletPressure_FieldComponentState fieldComponentState5:
          component.OutletPressure = fieldComponentState5.OutletPressure;
          break;
        case GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState componentAutoState:
          component.Enabled = componentAutoState.Enabled;
          component.Threshold = componentAutoState.Threshold;
          component.FlowRate = componentAutoState.FlowRate;
          component.InletPressure = componentAutoState.InletPressure;
          component.OutletPressure = componentAutoState.OutletPressure;
          break;
        default:
          return;
      }
      IComponentState current = ((ComponentHandleState) ref args).Current;
      if (current == null)
        return;
      AfterAutoHandleStateEvent handleStateEvent;
      // ISSUE: explicit constructor call
      ((AfterAutoHandleStateEvent) ref handleStateEvent).\u002Ector(current);
      ((IDirectedEventBus) this.EntityManager.EventBus).RaiseComponentEvent<AfterAutoHandleStateEvent, GasPressureRegulatorComponent>(uid, component, ref handleStateEvent);
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Enabled_FieldComponentState : 
    IComponentDeltaState<GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Enabled;

    public void ApplyToFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      fullState.Enabled = this.Enabled;
    }

    public GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState CreateNewFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Threshold_FieldComponentState : 
    IComponentDeltaState<GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float Threshold;

    public void ApplyToFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      fullState.Threshold = this.Threshold;
    }

    public GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState CreateNewFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FlowRate_FieldComponentState : 
    IComponentDeltaState<GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float FlowRate;

    public void ApplyToFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      fullState.FlowRate = this.FlowRate;
    }

    public GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState CreateNewFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class InletPressure_FieldComponentState : 
    IComponentDeltaState<GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float InletPressure;

    public void ApplyToFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      fullState.InletPressure = this.InletPressure;
    }

    public GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState CreateNewFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class OutletPressure_FieldComponentState : 
    IComponentDeltaState<GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float OutletPressure;

    public void ApplyToFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      fullState.OutletPressure = this.OutletPressure;
    }

    public GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState CreateNewFullState(
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState)
    {
      GasPressureRegulatorComponent.GasPressureRegulatorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
