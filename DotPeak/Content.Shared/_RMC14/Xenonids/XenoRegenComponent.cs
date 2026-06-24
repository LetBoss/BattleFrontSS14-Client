// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.XenoRegenComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoSystem)})]
public sealed class XenoRegenComponent : 
  Component,
  ISerializationGenerated<XenoRegenComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 FlatHealing = (FixedPoint2) 0.5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 CritHealMultiplier = (FixedPoint2) 0.33;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 RestHealMultiplier = (FixedPoint2) 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 StandHealingMultiplier = (FixedPoint2) 0.4;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxHealthDivisorHeal = 65f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HealOffWeeds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RegenCooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextRegenTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoRegenComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoRegenComponent) target1;
    if (serialization.TryCustomCopy<XenoRegenComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.FlatHealing, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.FlatHealing, hookCtx, context);
    target.FlatHealing = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.CritHealMultiplier, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.CritHealMultiplier, hookCtx, context);
    target.CritHealMultiplier = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RestHealMultiplier, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.RestHealMultiplier, hookCtx, context);
    target.RestHealMultiplier = target4;
    FixedPoint2 target5 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.StandHealingMultiplier, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<FixedPoint2>(this.StandHealingMultiplier, hookCtx, context);
    target.StandHealingMultiplier = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxHealthDivisorHeal, ref target6, hookCtx, false, context))
      target6 = this.MaxHealthDivisorHeal;
    target.MaxHealthDivisorHeal = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.HealOffWeeds, ref target7, hookCtx, false, context))
      target7 = this.HealOffWeeds;
    target.HealOffWeeds = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RegenCooldown, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.RegenCooldown, hookCtx, context);
    target.RegenCooldown = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextRegenTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.NextRegenTime, hookCtx, context);
    target.NextRegenTime = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoRegenComponent target,
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
    XenoRegenComponent target1 = (XenoRegenComponent) target;
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
    XenoRegenComponent target1 = (XenoRegenComponent) target;
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
    XenoRegenComponent target1 = (XenoRegenComponent) target;
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
    XenoRegenComponent target1 = (XenoRegenComponent) target;
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
  virtual XenoRegenComponent Component.Instantiate() => new XenoRegenComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoRegenComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoRegenComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<XenoRegenComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      XenoRegenComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextRegenTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoRegenComponent_AutoState : IComponentState
  {
    public FixedPoint2 FlatHealing;
    public FixedPoint2 CritHealMultiplier;
    public FixedPoint2 RestHealMultiplier;
    public FixedPoint2 StandHealingMultiplier;
    public float MaxHealthDivisorHeal;
    public bool HealOffWeeds;
    public TimeSpan RegenCooldown;
    public TimeSpan NextRegenTime;

    public 
    #nullable enable
    XenoRegenComponent.XenoRegenComponent_AutoState ShallowClone()
    {
      return new XenoRegenComponent.XenoRegenComponent_AutoState()
      {
        FlatHealing = this.FlatHealing,
        CritHealMultiplier = this.CritHealMultiplier,
        RestHealMultiplier = this.RestHealMultiplier,
        StandHealingMultiplier = this.StandHealingMultiplier,
        MaxHealthDivisorHeal = this.MaxHealthDivisorHeal,
        HealOffWeeds = this.HealOffWeeds,
        RegenCooldown = this.RegenCooldown,
        NextRegenTime = this.NextRegenTime
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoRegenComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<XenoRegenComponent>("FlatHealing", "CritHealMultiplier", "RestHealMultiplier", "StandHealingMultiplier", "MaxHealthDivisorHeal", "HealOffWeeds", "RegenCooldown", "NextRegenTime");
      this.SubscribeLocalEvent<XenoRegenComponent, ComponentGetState>(new ComponentEventRefHandler<XenoRegenComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoRegenComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoRegenComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoRegenComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new XenoRegenComponent.FlatHealing_FieldComponentState()
            {
              FlatHealing = component.FlatHealing
            };
            return;
          case 2:
            args.State = (IComponentState) new XenoRegenComponent.CritHealMultiplier_FieldComponentState()
            {
              CritHealMultiplier = component.CritHealMultiplier
            };
            return;
          case 4:
            args.State = (IComponentState) new XenoRegenComponent.RestHealMultiplier_FieldComponentState()
            {
              RestHealMultiplier = component.RestHealMultiplier
            };
            return;
          case 8:
            args.State = (IComponentState) new XenoRegenComponent.StandHealingMultiplier_FieldComponentState()
            {
              StandHealingMultiplier = component.StandHealingMultiplier
            };
            return;
          case 16 /*0x10*/:
            args.State = (IComponentState) new XenoRegenComponent.MaxHealthDivisorHeal_FieldComponentState()
            {
              MaxHealthDivisorHeal = component.MaxHealthDivisorHeal
            };
            return;
          case 32 /*0x20*/:
            args.State = (IComponentState) new XenoRegenComponent.HealOffWeeds_FieldComponentState()
            {
              HealOffWeeds = component.HealOffWeeds
            };
            return;
          case 64 /*0x40*/:
            args.State = (IComponentState) new XenoRegenComponent.RegenCooldown_FieldComponentState()
            {
              RegenCooldown = component.RegenCooldown
            };
            return;
          case 128 /*0x80*/:
            args.State = (IComponentState) new XenoRegenComponent.NextRegenTime_FieldComponentState()
            {
              NextRegenTime = component.NextRegenTime
            };
            return;
        }
      }
      args.State = (IComponentState) new XenoRegenComponent.XenoRegenComponent_AutoState()
      {
        FlatHealing = component.FlatHealing,
        CritHealMultiplier = component.CritHealMultiplier,
        RestHealMultiplier = component.RestHealMultiplier,
        StandHealingMultiplier = component.StandHealingMultiplier,
        MaxHealthDivisorHeal = component.MaxHealthDivisorHeal,
        HealOffWeeds = component.HealOffWeeds,
        RegenCooldown = component.RegenCooldown,
        NextRegenTime = component.NextRegenTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoRegenComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case XenoRegenComponent.FlatHealing_FieldComponentState fieldComponentState1:
          component.FlatHealing = fieldComponentState1.FlatHealing;
          break;
        case XenoRegenComponent.CritHealMultiplier_FieldComponentState fieldComponentState2:
          component.CritHealMultiplier = fieldComponentState2.CritHealMultiplier;
          break;
        case XenoRegenComponent.RestHealMultiplier_FieldComponentState fieldComponentState3:
          component.RestHealMultiplier = fieldComponentState3.RestHealMultiplier;
          break;
        case XenoRegenComponent.StandHealingMultiplier_FieldComponentState fieldComponentState4:
          component.StandHealingMultiplier = fieldComponentState4.StandHealingMultiplier;
          break;
        case XenoRegenComponent.MaxHealthDivisorHeal_FieldComponentState fieldComponentState5:
          component.MaxHealthDivisorHeal = fieldComponentState5.MaxHealthDivisorHeal;
          break;
        case XenoRegenComponent.HealOffWeeds_FieldComponentState fieldComponentState6:
          component.HealOffWeeds = fieldComponentState6.HealOffWeeds;
          break;
        case XenoRegenComponent.RegenCooldown_FieldComponentState fieldComponentState7:
          component.RegenCooldown = fieldComponentState7.RegenCooldown;
          break;
        case XenoRegenComponent.NextRegenTime_FieldComponentState fieldComponentState8:
          component.NextRegenTime = fieldComponentState8.NextRegenTime;
          break;
        case XenoRegenComponent.XenoRegenComponent_AutoState componentAutoState:
          component.FlatHealing = componentAutoState.FlatHealing;
          component.CritHealMultiplier = componentAutoState.CritHealMultiplier;
          component.RestHealMultiplier = componentAutoState.RestHealMultiplier;
          component.StandHealingMultiplier = componentAutoState.StandHealingMultiplier;
          component.MaxHealthDivisorHeal = componentAutoState.MaxHealthDivisorHeal;
          component.HealOffWeeds = componentAutoState.HealOffWeeds;
          component.RegenCooldown = componentAutoState.RegenCooldown;
          component.NextRegenTime = componentAutoState.NextRegenTime;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FlatHealing_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 FlatHealing;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.FlatHealing = this.FlatHealing;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CritHealMultiplier_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 CritHealMultiplier;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.CritHealMultiplier = this.CritHealMultiplier;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RestHealMultiplier_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 RestHealMultiplier;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.RestHealMultiplier = this.RestHealMultiplier;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class StandHealingMultiplier_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public FixedPoint2 StandHealingMultiplier;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.StandHealingMultiplier = this.StandHealingMultiplier;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MaxHealthDivisorHeal_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float MaxHealthDivisorHeal;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.MaxHealthDivisorHeal = this.MaxHealthDivisorHeal;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class HealOffWeeds_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool HealOffWeeds;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.HealOffWeeds = this.HealOffWeeds;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class RegenCooldown_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan RegenCooldown;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.RegenCooldown = this.RegenCooldown;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextRegenTime_FieldComponentState : 
    IComponentDeltaState<XenoRegenComponent.XenoRegenComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextRegenTime;

    public void ApplyToFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      fullState.NextRegenTime = this.NextRegenTime;
    }

    public XenoRegenComponent.XenoRegenComponent_AutoState CreateNewFullState(
      XenoRegenComponent.XenoRegenComponent_AutoState fullState)
    {
      XenoRegenComponent.XenoRegenComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
