// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.ThirstComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ThirstSystem)})]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
public sealed class ThirstComponent : 
  Component,
  ISerializationGenerated<ThirstComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("baseDecayRate", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BaseDecayRate = 0.1f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float ActualDecayRate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ThirstThreshold CurrentThirstThreshold;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ThirstThreshold LastThirstThreshold;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("startingThirst", false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CurrentThirst = -1f;
  [DataField("nextUpdateTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextUpdateTime;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateRate = TimeSpan.FromSeconds(1L);
  [DataField("thresholds", false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<ThirstThreshold, float> ThirstThresholds = new Dictionary<ThirstThreshold, float>()
  {
    {
      ThirstThreshold.OverHydrated,
      600f
    },
    {
      ThirstThreshold.Okay,
      450f
    },
    {
      ThirstThreshold.Thirsty,
      300f
    },
    {
      ThirstThreshold.Parched,
      150f
    },
    {
      ThirstThreshold.Dead,
      0.0f
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertCategoryPrototype> ThirstyCategory = (ProtoId<AlertCategoryPrototype>) "Thirst";
  public static readonly Dictionary<ThirstThreshold, ProtoId<AlertPrototype>> ThirstThresholdAlertTypes = new Dictionary<ThirstThreshold, ProtoId<AlertPrototype>>()
  {
    {
      ThirstThreshold.Thirsty,
      (ProtoId<AlertPrototype>) "Thirsty"
    },
    {
      ThirstThreshold.Parched,
      (ProtoId<AlertPrototype>) "Parched"
    },
    {
      ThirstThreshold.Dead,
      (ProtoId<AlertPrototype>) "Parched"
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ThirstComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ThirstComponent) target1;
    if (serialization.TryCustomCopy<ThirstComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseDecayRate, ref target2, hookCtx, false, context))
      target2 = this.BaseDecayRate;
    target.BaseDecayRate = target2;
    ThirstThreshold target3 = ThirstThreshold.Dead;
    if (!serialization.TryCustomCopy<ThirstThreshold>(this.CurrentThirstThreshold, ref target3, hookCtx, false, context))
      target3 = this.CurrentThirstThreshold;
    target.CurrentThirstThreshold = target3;
    ThirstThreshold target4 = ThirstThreshold.Dead;
    if (!serialization.TryCustomCopy<ThirstThreshold>(this.LastThirstThreshold, ref target4, hookCtx, false, context))
      target4 = this.LastThirstThreshold;
    target.LastThirstThreshold = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CurrentThirst, ref target5, hookCtx, false, context))
      target5 = this.CurrentThirst;
    target.CurrentThirst = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdateTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.NextUpdateTime, hookCtx, context);
    target.NextUpdateTime = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateRate, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.UpdateRate, hookCtx, context);
    target.UpdateRate = target7;
    Dictionary<ThirstThreshold, float> target8 = (Dictionary<ThirstThreshold, float>) null;
    if (this.ThirstThresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ThirstThreshold, float>>(this.ThirstThresholds, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<Dictionary<ThirstThreshold, float>>(this.ThirstThresholds, hookCtx, context);
    target.ThirstThresholds = target8;
    ProtoId<AlertCategoryPrototype> target9 = new ProtoId<AlertCategoryPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertCategoryPrototype>>(this.ThirstyCategory, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<AlertCategoryPrototype>>(this.ThirstyCategory, hookCtx, context);
    target.ThirstyCategory = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ThirstComponent target,
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
    ThirstComponent target1 = (ThirstComponent) target;
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
    ThirstComponent target1 = (ThirstComponent) target;
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
    ThirstComponent target1 = (ThirstComponent) target;
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

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IComponentDelta target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ThirstComponent target1 = (ThirstComponent) target;
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
  virtual ThirstComponent Component.Instantiate() => new ThirstComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ThirstComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ThirstComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ThirstComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ThirstComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdateTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ThirstComponent_AutoState : IComponentState
  {
    public float BaseDecayRate;
    public float ActualDecayRate;
    public ThirstThreshold CurrentThirstThreshold;
    public ThirstThreshold LastThirstThreshold;
    public float CurrentThirst;
    public TimeSpan NextUpdateTime;
    public TimeSpan UpdateRate;
    public 
    #nullable enable
    Dictionary<ThirstThreshold, float> ThirstThresholds;

    public ThirstComponent.ThirstComponent_AutoState ShallowClone()
    {
      return new ThirstComponent.ThirstComponent_AutoState()
      {
        BaseDecayRate = this.BaseDecayRate,
        ActualDecayRate = this.ActualDecayRate,
        CurrentThirstThreshold = this.CurrentThirstThreshold,
        LastThirstThreshold = this.LastThirstThreshold,
        CurrentThirst = this.CurrentThirst,
        NextUpdateTime = this.NextUpdateTime,
        UpdateRate = this.UpdateRate,
        ThirstThresholds = this.ThirstThresholds
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ThirstComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<ThirstComponent>("BaseDecayRate", "ActualDecayRate", "CurrentThirstThreshold", "LastThirstThreshold", "CurrentThirst", "NextUpdateTime", "UpdateRate", "ThirstThresholds");
      this.SubscribeLocalEvent<ThirstComponent, ComponentGetState>(new ComponentEventRefHandler<ThirstComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ThirstComponent, ComponentHandleState>(new ComponentEventRefHandler<ThirstComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, ThirstComponent component, ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new ThirstComponent.BaseDecayRate_FieldComponentState()
            {
              BaseDecayRate = component.BaseDecayRate
            };
            return;
          case 2:
            args.State = (IComponentState) new ThirstComponent.ActualDecayRate_FieldComponentState()
            {
              ActualDecayRate = component.ActualDecayRate
            };
            return;
          case 4:
            args.State = (IComponentState) new ThirstComponent.CurrentThirstThreshold_FieldComponentState()
            {
              CurrentThirstThreshold = component.CurrentThirstThreshold
            };
            return;
          case 8:
            args.State = (IComponentState) new ThirstComponent.LastThirstThreshold_FieldComponentState()
            {
              LastThirstThreshold = component.LastThirstThreshold
            };
            return;
          case 16 /*0x10*/:
            args.State = (IComponentState) new ThirstComponent.CurrentThirst_FieldComponentState()
            {
              CurrentThirst = component.CurrentThirst
            };
            return;
          case 32 /*0x20*/:
            args.State = (IComponentState) new ThirstComponent.NextUpdateTime_FieldComponentState()
            {
              NextUpdateTime = component.NextUpdateTime
            };
            return;
          case 64 /*0x40*/:
            args.State = (IComponentState) new ThirstComponent.UpdateRate_FieldComponentState()
            {
              UpdateRate = component.UpdateRate
            };
            return;
          case 128 /*0x80*/:
            args.State = (IComponentState) new ThirstComponent.ThirstThresholds_FieldComponentState()
            {
              ThirstThresholds = component.ThirstThresholds
            };
            return;
        }
      }
      args.State = (IComponentState) new ThirstComponent.ThirstComponent_AutoState()
      {
        BaseDecayRate = component.BaseDecayRate,
        ActualDecayRate = component.ActualDecayRate,
        CurrentThirstThreshold = component.CurrentThirstThreshold,
        LastThirstThreshold = component.LastThirstThreshold,
        CurrentThirst = component.CurrentThirst,
        NextUpdateTime = component.NextUpdateTime,
        UpdateRate = component.UpdateRate,
        ThirstThresholds = component.ThirstThresholds
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ThirstComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case ThirstComponent.BaseDecayRate_FieldComponentState fieldComponentState1:
          component.BaseDecayRate = fieldComponentState1.BaseDecayRate;
          break;
        case ThirstComponent.ActualDecayRate_FieldComponentState fieldComponentState2:
          component.ActualDecayRate = fieldComponentState2.ActualDecayRate;
          break;
        case ThirstComponent.CurrentThirstThreshold_FieldComponentState fieldComponentState3:
          component.CurrentThirstThreshold = fieldComponentState3.CurrentThirstThreshold;
          break;
        case ThirstComponent.LastThirstThreshold_FieldComponentState fieldComponentState4:
          component.LastThirstThreshold = fieldComponentState4.LastThirstThreshold;
          break;
        case ThirstComponent.CurrentThirst_FieldComponentState fieldComponentState5:
          component.CurrentThirst = fieldComponentState5.CurrentThirst;
          break;
        case ThirstComponent.NextUpdateTime_FieldComponentState fieldComponentState6:
          component.NextUpdateTime = fieldComponentState6.NextUpdateTime;
          break;
        case ThirstComponent.UpdateRate_FieldComponentState fieldComponentState7:
          component.UpdateRate = fieldComponentState7.UpdateRate;
          break;
        case ThirstComponent.ThirstThresholds_FieldComponentState fieldComponentState8:
          Dictionary<ThirstThreshold, float> thirstThresholds = fieldComponentState8.ThirstThresholds;
          if (thirstThresholds == null)
          {
            component.ThirstThresholds = (Dictionary<ThirstThreshold, float>) null;
            break;
          }
          component.ThirstThresholds = new Dictionary<ThirstThreshold, float>((IDictionary<ThirstThreshold, float>) thirstThresholds);
          break;
        case ThirstComponent.ThirstComponent_AutoState componentAutoState:
          component.BaseDecayRate = componentAutoState.BaseDecayRate;
          component.ActualDecayRate = componentAutoState.ActualDecayRate;
          component.CurrentThirstThreshold = componentAutoState.CurrentThirstThreshold;
          component.LastThirstThreshold = componentAutoState.LastThirstThreshold;
          component.CurrentThirst = componentAutoState.CurrentThirst;
          component.NextUpdateTime = componentAutoState.NextUpdateTime;
          component.UpdateRate = componentAutoState.UpdateRate;
          component.ThirstThresholds = componentAutoState.ThirstThresholds == null ? (Dictionary<ThirstThreshold, float>) null : new Dictionary<ThirstThreshold, float>((IDictionary<ThirstThreshold, float>) componentAutoState.ThirstThresholds);
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BaseDecayRate_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float BaseDecayRate;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.BaseDecayRate = this.BaseDecayRate;
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ActualDecayRate_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float ActualDecayRate;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.ActualDecayRate = this.ActualDecayRate;
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CurrentThirstThreshold_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ThirstThreshold CurrentThirstThreshold;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.CurrentThirstThreshold = this.CurrentThirstThreshold;
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class LastThirstThreshold_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public ThirstThreshold LastThirstThreshold;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.LastThirstThreshold = this.LastThirstThreshold;
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CurrentThirst_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float CurrentThirst;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.CurrentThirst = this.CurrentThirst;
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextUpdateTime_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextUpdateTime;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.NextUpdateTime = this.NextUpdateTime;
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UpdateRate_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan UpdateRate;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.UpdateRate = this.UpdateRate;
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ThirstThresholds_FieldComponentState : 
    IComponentDeltaState<ThirstComponent.ThirstComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Dictionary<ThirstThreshold, float> ThirstThresholds;

    public void ApplyToFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      fullState.ThirstThresholds = this.ThirstThresholds == null ? (Dictionary<ThirstThreshold, float>) null : new Dictionary<ThirstThreshold, float>((IDictionary<ThirstThreshold, float>) this.ThirstThresholds);
    }

    public ThirstComponent.ThirstComponent_AutoState CreateNewFullState(
      ThirstComponent.ThirstComponent_AutoState fullState)
    {
      ThirstComponent.ThirstComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
