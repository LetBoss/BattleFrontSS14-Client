// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Components.HungerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Damage;
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
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
[Access(new Type[] {typeof (HungerSystem)})]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
public sealed class HungerComponent : 
  Component,
  ISerializationGenerated<HungerComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [AutoNetworkedField]
  public float LastAuthoritativeHungerValue;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan LastAuthoritativeHungerChangeTime;
  [DataField("baseDecayRate", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float BaseDecayRate = 0.0166666675f;
  [DataField("actualDecayRate", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float ActualDecayRate;
  [DataField("lastThreshold", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public HungerThreshold LastThreshold;
  [DataField("currentThreshold", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public HungerThreshold CurrentThreshold;
  [DataField("thresholds", false, 1, false, false, typeof (DictionarySerializer<HungerThreshold, float>))]
  [AutoNetworkedField]
  public Dictionary<HungerThreshold, float> Thresholds = new Dictionary<HungerThreshold, float>()
  {
    {
      HungerThreshold.Overfed,
      200f
    },
    {
      HungerThreshold.Okay,
      150f
    },
    {
      HungerThreshold.Peckish,
      100f
    },
    {
      HungerThreshold.Starving,
      50f
    },
    {
      HungerThreshold.Dead,
      0.0f
    }
  };
  [DataField("hungerThresholdAlerts", false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<HungerThreshold, ProtoId<AlertPrototype>> HungerThresholdAlerts = new Dictionary<HungerThreshold, ProtoId<AlertPrototype>>()
  {
    {
      HungerThreshold.Peckish,
      (ProtoId<AlertPrototype>) "Peckish"
    },
    {
      HungerThreshold.Starving,
      (ProtoId<AlertPrototype>) "Starving"
    },
    {
      HungerThreshold.Dead,
      (ProtoId<AlertPrototype>) "Starving"
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertCategoryPrototype> HungerAlertCategory = (ProtoId<AlertCategoryPrototype>) "Hunger";
  [DataField("hungerThresholdDecayModifiers", false, 1, false, false, typeof (DictionarySerializer<HungerThreshold, float>))]
  [AutoNetworkedField]
  public Dictionary<HungerThreshold, float> HungerThresholdDecayModifiers = new Dictionary<HungerThreshold, float>()
  {
    {
      HungerThreshold.Overfed,
      1.2f
    },
    {
      HungerThreshold.Okay,
      1f
    },
    {
      HungerThreshold.Peckish,
      0.8f
    },
    {
      HungerThreshold.Starving,
      0.6f
    },
    {
      HungerThreshold.Dead,
      0.6f
    }
  };
  [DataField("starvingSlowdownModifier", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public float StarvingSlowdownModifier = 0.75f;
  [DataField("starvationDamage", false, 1, false, false, null)]
  public DamageSpecifier? StarvationDamage;
  [DataField("nextUpdateTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextThresholdUpdateTime;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public TimeSpan ThresholdUpdateRate = TimeSpan.FromSeconds(1L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HungerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HungerComponent) target1;
    if (serialization.TryCustomCopy<HungerComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.LastAuthoritativeHungerValue, ref target2, hookCtx, false, context))
      target2 = this.LastAuthoritativeHungerValue;
    target.LastAuthoritativeHungerValue = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastAuthoritativeHungerChangeTime, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.LastAuthoritativeHungerChangeTime, hookCtx, context);
    target.LastAuthoritativeHungerChangeTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseDecayRate, ref target4, hookCtx, false, context))
      target4 = this.BaseDecayRate;
    target.BaseDecayRate = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ActualDecayRate, ref target5, hookCtx, false, context))
      target5 = this.ActualDecayRate;
    target.ActualDecayRate = target5;
    HungerThreshold target6 = HungerThreshold.Dead;
    if (!serialization.TryCustomCopy<HungerThreshold>(this.LastThreshold, ref target6, hookCtx, false, context))
      target6 = this.LastThreshold;
    target.LastThreshold = target6;
    HungerThreshold target7 = HungerThreshold.Dead;
    if (!serialization.TryCustomCopy<HungerThreshold>(this.CurrentThreshold, ref target7, hookCtx, false, context))
      target7 = this.CurrentThreshold;
    target.CurrentThreshold = target7;
    if (this.Thresholds == null)
      throw new NullNotAllowedException();
    Dictionary<HungerThreshold, float> target8 = (Dictionary<HungerThreshold, float>) null;
    serialization.CopyTo<Dictionary<HungerThreshold, float>, DictionarySerializer<HungerThreshold, float>>(this.Thresholds, ref target8, hookCtx, context, true);
    Dictionary<HungerThreshold, float> dictionary1 = target8;
    target.Thresholds = dictionary1;
    Dictionary<HungerThreshold, ProtoId<AlertPrototype>> target9 = (Dictionary<HungerThreshold, ProtoId<AlertPrototype>>) null;
    if (this.HungerThresholdAlerts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HungerThreshold, ProtoId<AlertPrototype>>>(this.HungerThresholdAlerts, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy<Dictionary<HungerThreshold, ProtoId<AlertPrototype>>>(this.HungerThresholdAlerts, hookCtx, context);
    target.HungerThresholdAlerts = target9;
    ProtoId<AlertCategoryPrototype> target10 = new ProtoId<AlertCategoryPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertCategoryPrototype>>(this.HungerAlertCategory, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<AlertCategoryPrototype>>(this.HungerAlertCategory, hookCtx, context);
    target.HungerAlertCategory = target10;
    if (this.HungerThresholdDecayModifiers == null)
      throw new NullNotAllowedException();
    Dictionary<HungerThreshold, float> target11 = (Dictionary<HungerThreshold, float>) null;
    serialization.CopyTo<Dictionary<HungerThreshold, float>, DictionarySerializer<HungerThreshold, float>>(this.HungerThresholdDecayModifiers, ref target11, hookCtx, context, true);
    Dictionary<HungerThreshold, float> dictionary2 = target11;
    target.HungerThresholdDecayModifiers = dictionary2;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StarvingSlowdownModifier, ref target12, hookCtx, false, context))
      target12 = this.StarvingSlowdownModifier;
    target.StarvingSlowdownModifier = target12;
    DamageSpecifier target13 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.StarvationDamage, ref target13, hookCtx, false, context))
    {
      if (this.StarvationDamage == null)
        target13 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.StarvationDamage, ref target13, hookCtx, context);
    }
    target.StarvationDamage = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextThresholdUpdateTime, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.NextThresholdUpdateTime, hookCtx, context);
    target.NextThresholdUpdateTime = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HungerComponent target,
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
    HungerComponent target1 = (HungerComponent) target;
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
    HungerComponent target1 = (HungerComponent) target;
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
    HungerComponent target1 = (HungerComponent) target;
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
    HungerComponent target1 = (HungerComponent) target;
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
  virtual HungerComponent Component.Instantiate() => new HungerComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HungerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HungerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<HungerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      HungerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextThresholdUpdateTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HungerComponent_AutoState : IComponentState
  {
    public float LastAuthoritativeHungerValue;
    public TimeSpan LastAuthoritativeHungerChangeTime;
    public float ActualDecayRate;
    public HungerThreshold LastThreshold;
    public HungerThreshold CurrentThreshold;
    public 
    #nullable enable
    Dictionary<HungerThreshold, float> Thresholds;
    public Dictionary<HungerThreshold, ProtoId<AlertPrototype>> HungerThresholdAlerts;
    public Dictionary<HungerThreshold, float> HungerThresholdDecayModifiers;
    public float StarvingSlowdownModifier;
    public TimeSpan NextThresholdUpdateTime;
    public TimeSpan ThresholdUpdateRate;

    public HungerComponent.HungerComponent_AutoState ShallowClone()
    {
      return new HungerComponent.HungerComponent_AutoState()
      {
        LastAuthoritativeHungerValue = this.LastAuthoritativeHungerValue,
        LastAuthoritativeHungerChangeTime = this.LastAuthoritativeHungerChangeTime,
        ActualDecayRate = this.ActualDecayRate,
        LastThreshold = this.LastThreshold,
        CurrentThreshold = this.CurrentThreshold,
        Thresholds = this.Thresholds,
        HungerThresholdAlerts = this.HungerThresholdAlerts,
        HungerThresholdDecayModifiers = this.HungerThresholdDecayModifiers,
        StarvingSlowdownModifier = this.StarvingSlowdownModifier,
        NextThresholdUpdateTime = this.NextThresholdUpdateTime,
        ThresholdUpdateRate = this.ThresholdUpdateRate
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HungerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<HungerComponent>("LastAuthoritativeHungerValue", "LastAuthoritativeHungerChangeTime", "ActualDecayRate", "LastThreshold", "CurrentThreshold", "Thresholds", "HungerThresholdAlerts", "HungerThresholdDecayModifiers", "StarvingSlowdownModifier", "NextThresholdUpdateTime", "ThresholdUpdateRate");
      this.SubscribeLocalEvent<HungerComponent, ComponentGetState>(new ComponentEventRefHandler<HungerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HungerComponent, ComponentHandleState>(new ComponentEventRefHandler<HungerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, HungerComponent component, ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        uint modifiedFields = this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick);
        if (modifiedFields <= 32U /*0x20*/)
        {
          if (modifiedFields <= 8U)
          {
            switch ((int) modifiedFields - 1)
            {
              case 0:
                args.State = (IComponentState) new HungerComponent.LastAuthoritativeHungerValue_FieldComponentState()
                {
                  LastAuthoritativeHungerValue = component.LastAuthoritativeHungerValue
                };
                return;
              case 1:
                args.State = (IComponentState) new HungerComponent.LastAuthoritativeHungerChangeTime_FieldComponentState()
                {
                  LastAuthoritativeHungerChangeTime = component.LastAuthoritativeHungerChangeTime
                };
                return;
              case 2:
                break;
              case 3:
                args.State = (IComponentState) new HungerComponent.ActualDecayRate_FieldComponentState()
                {
                  ActualDecayRate = component.ActualDecayRate
                };
                return;
              default:
                if (modifiedFields == 8U)
                {
                  args.State = (IComponentState) new HungerComponent.LastThreshold_FieldComponentState()
                  {
                    LastThreshold = component.LastThreshold
                  };
                  return;
                }
                break;
            }
          }
          else if (modifiedFields != 16U /*0x10*/)
          {
            if (modifiedFields == 32U /*0x20*/)
            {
              args.State = (IComponentState) new HungerComponent.Thresholds_FieldComponentState()
              {
                Thresholds = component.Thresholds
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new HungerComponent.CurrentThreshold_FieldComponentState()
            {
              CurrentThreshold = component.CurrentThreshold
            };
            return;
          }
        }
        else if (modifiedFields <= 128U /*0x80*/)
        {
          if (modifiedFields != 64U /*0x40*/)
          {
            if (modifiedFields == 128U /*0x80*/)
            {
              args.State = (IComponentState) new HungerComponent.HungerThresholdDecayModifiers_FieldComponentState()
              {
                HungerThresholdDecayModifiers = component.HungerThresholdDecayModifiers
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new HungerComponent.HungerThresholdAlerts_FieldComponentState()
            {
              HungerThresholdAlerts = component.HungerThresholdAlerts
            };
            return;
          }
        }
        else if (modifiedFields != 256U /*0x0100*/)
        {
          if (modifiedFields != 512U /*0x0200*/)
          {
            if (modifiedFields == 1024U /*0x0400*/)
            {
              args.State = (IComponentState) new HungerComponent.ThresholdUpdateRate_FieldComponentState()
              {
                ThresholdUpdateRate = component.ThresholdUpdateRate
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new HungerComponent.NextThresholdUpdateTime_FieldComponentState()
            {
              NextThresholdUpdateTime = component.NextThresholdUpdateTime
            };
            return;
          }
        }
        else
        {
          args.State = (IComponentState) new HungerComponent.StarvingSlowdownModifier_FieldComponentState()
          {
            StarvingSlowdownModifier = component.StarvingSlowdownModifier
          };
          return;
        }
      }
      args.State = (IComponentState) new HungerComponent.HungerComponent_AutoState()
      {
        LastAuthoritativeHungerValue = component.LastAuthoritativeHungerValue,
        LastAuthoritativeHungerChangeTime = component.LastAuthoritativeHungerChangeTime,
        ActualDecayRate = component.ActualDecayRate,
        LastThreshold = component.LastThreshold,
        CurrentThreshold = component.CurrentThreshold,
        Thresholds = component.Thresholds,
        HungerThresholdAlerts = component.HungerThresholdAlerts,
        HungerThresholdDecayModifiers = component.HungerThresholdDecayModifiers,
        StarvingSlowdownModifier = component.StarvingSlowdownModifier,
        NextThresholdUpdateTime = component.NextThresholdUpdateTime,
        ThresholdUpdateRate = component.ThresholdUpdateRate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HungerComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case HungerComponent.LastAuthoritativeHungerValue_FieldComponentState fieldComponentState1:
          component.LastAuthoritativeHungerValue = fieldComponentState1.LastAuthoritativeHungerValue;
          break;
        case HungerComponent.LastAuthoritativeHungerChangeTime_FieldComponentState fieldComponentState2:
          component.LastAuthoritativeHungerChangeTime = fieldComponentState2.LastAuthoritativeHungerChangeTime;
          break;
        case HungerComponent.ActualDecayRate_FieldComponentState fieldComponentState3:
          component.ActualDecayRate = fieldComponentState3.ActualDecayRate;
          break;
        case HungerComponent.LastThreshold_FieldComponentState fieldComponentState4:
          component.LastThreshold = fieldComponentState4.LastThreshold;
          break;
        case HungerComponent.CurrentThreshold_FieldComponentState fieldComponentState5:
          component.CurrentThreshold = fieldComponentState5.CurrentThreshold;
          break;
        case HungerComponent.Thresholds_FieldComponentState fieldComponentState6:
          Dictionary<HungerThreshold, float> thresholds = fieldComponentState6.Thresholds;
          if (thresholds == null)
          {
            component.Thresholds = (Dictionary<HungerThreshold, float>) null;
            break;
          }
          component.Thresholds = new Dictionary<HungerThreshold, float>((IDictionary<HungerThreshold, float>) thresholds);
          break;
        case HungerComponent.HungerThresholdAlerts_FieldComponentState fieldComponentState7:
          Dictionary<HungerThreshold, ProtoId<AlertPrototype>> hungerThresholdAlerts = fieldComponentState7.HungerThresholdAlerts;
          if (hungerThresholdAlerts == null)
          {
            component.HungerThresholdAlerts = (Dictionary<HungerThreshold, ProtoId<AlertPrototype>>) null;
            break;
          }
          component.HungerThresholdAlerts = new Dictionary<HungerThreshold, ProtoId<AlertPrototype>>((IDictionary<HungerThreshold, ProtoId<AlertPrototype>>) hungerThresholdAlerts);
          break;
        case HungerComponent.HungerThresholdDecayModifiers_FieldComponentState fieldComponentState8:
          Dictionary<HungerThreshold, float> thresholdDecayModifiers = fieldComponentState8.HungerThresholdDecayModifiers;
          if (thresholdDecayModifiers == null)
          {
            component.HungerThresholdDecayModifiers = (Dictionary<HungerThreshold, float>) null;
            break;
          }
          component.HungerThresholdDecayModifiers = new Dictionary<HungerThreshold, float>((IDictionary<HungerThreshold, float>) thresholdDecayModifiers);
          break;
        case HungerComponent.StarvingSlowdownModifier_FieldComponentState fieldComponentState9:
          component.StarvingSlowdownModifier = fieldComponentState9.StarvingSlowdownModifier;
          break;
        case HungerComponent.NextThresholdUpdateTime_FieldComponentState fieldComponentState10:
          component.NextThresholdUpdateTime = fieldComponentState10.NextThresholdUpdateTime;
          break;
        case HungerComponent.ThresholdUpdateRate_FieldComponentState fieldComponentState11:
          component.ThresholdUpdateRate = fieldComponentState11.ThresholdUpdateRate;
          break;
        case HungerComponent.HungerComponent_AutoState componentAutoState:
          component.LastAuthoritativeHungerValue = componentAutoState.LastAuthoritativeHungerValue;
          component.LastAuthoritativeHungerChangeTime = componentAutoState.LastAuthoritativeHungerChangeTime;
          component.ActualDecayRate = componentAutoState.ActualDecayRate;
          component.LastThreshold = componentAutoState.LastThreshold;
          component.CurrentThreshold = componentAutoState.CurrentThreshold;
          component.Thresholds = componentAutoState.Thresholds == null ? (Dictionary<HungerThreshold, float>) null : new Dictionary<HungerThreshold, float>((IDictionary<HungerThreshold, float>) componentAutoState.Thresholds);
          component.HungerThresholdAlerts = componentAutoState.HungerThresholdAlerts == null ? (Dictionary<HungerThreshold, ProtoId<AlertPrototype>>) null : new Dictionary<HungerThreshold, ProtoId<AlertPrototype>>((IDictionary<HungerThreshold, ProtoId<AlertPrototype>>) componentAutoState.HungerThresholdAlerts);
          component.HungerThresholdDecayModifiers = componentAutoState.HungerThresholdDecayModifiers == null ? (Dictionary<HungerThreshold, float>) null : new Dictionary<HungerThreshold, float>((IDictionary<HungerThreshold, float>) componentAutoState.HungerThresholdDecayModifiers);
          component.StarvingSlowdownModifier = componentAutoState.StarvingSlowdownModifier;
          component.NextThresholdUpdateTime = componentAutoState.NextThresholdUpdateTime;
          component.ThresholdUpdateRate = componentAutoState.ThresholdUpdateRate;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class LastAuthoritativeHungerValue_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float LastAuthoritativeHungerValue;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.LastAuthoritativeHungerValue = this.LastAuthoritativeHungerValue;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class LastAuthoritativeHungerChangeTime_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan LastAuthoritativeHungerChangeTime;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.LastAuthoritativeHungerChangeTime = this.LastAuthoritativeHungerChangeTime;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ActualDecayRate_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float ActualDecayRate;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.ActualDecayRate = this.ActualDecayRate;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class LastThreshold_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public HungerThreshold LastThreshold;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.LastThreshold = this.LastThreshold;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CurrentThreshold_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public HungerThreshold CurrentThreshold;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.CurrentThreshold = this.CurrentThreshold;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Thresholds_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Dictionary<HungerThreshold, float> Thresholds;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.Thresholds = this.Thresholds == null ? (Dictionary<HungerThreshold, float>) null : new Dictionary<HungerThreshold, float>((IDictionary<HungerThreshold, float>) this.Thresholds);
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class HungerThresholdAlerts_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Dictionary<HungerThreshold, ProtoId<AlertPrototype>> HungerThresholdAlerts;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.HungerThresholdAlerts = this.HungerThresholdAlerts == null ? (Dictionary<HungerThreshold, ProtoId<AlertPrototype>>) null : new Dictionary<HungerThreshold, ProtoId<AlertPrototype>>((IDictionary<HungerThreshold, ProtoId<AlertPrototype>>) this.HungerThresholdAlerts);
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class HungerThresholdDecayModifiers_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Dictionary<HungerThreshold, float> HungerThresholdDecayModifiers;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.HungerThresholdDecayModifiers = this.HungerThresholdDecayModifiers == null ? (Dictionary<HungerThreshold, float>) null : new Dictionary<HungerThreshold, float>((IDictionary<HungerThreshold, float>) this.HungerThresholdDecayModifiers);
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class StarvingSlowdownModifier_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float StarvingSlowdownModifier;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.StarvingSlowdownModifier = this.StarvingSlowdownModifier;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextThresholdUpdateTime_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextThresholdUpdateTime;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.NextThresholdUpdateTime = this.NextThresholdUpdateTime;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class ThresholdUpdateRate_FieldComponentState : 
    IComponentDeltaState<HungerComponent.HungerComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan ThresholdUpdateRate;

    public void ApplyToFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      fullState.ThresholdUpdateRate = this.ThresholdUpdateRate;
    }

    public HungerComponent.HungerComponent_AutoState CreateNewFullState(
      HungerComponent.HungerComponent_AutoState fullState)
    {
      HungerComponent.HungerComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
