// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.DamageMobStateComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
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
namespace Content.Shared._RMC14.Damage;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCDamageableSystem)})]
public sealed class DamageMobStateComponent : 
  Component,
  ISerializationGenerated<DamageMobStateComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier CritDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DamageSpecifier NonDeadDamage = new DamageSpecifier();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan DamageAt;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DamageMobStateComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DamageMobStateComponent) target1;
    if (serialization.TryCustomCopy<DamageMobStateComponent>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.CritDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CritDamage, ref target2, hookCtx, false, context))
    {
      if (this.CritDamage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CritDamage, ref target2, hookCtx, context, true);
    }
    target.CritDamage = target2;
    DamageSpecifier target3 = (DamageSpecifier) null;
    if (this.NonDeadDamage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.NonDeadDamage, ref target3, hookCtx, false, context))
    {
      if (this.NonDeadDamage == null)
        target3 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.NonDeadDamage, ref target3, hookCtx, context, true);
    }
    target.NonDeadDamage = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DamageAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.DamageAt, hookCtx, context);
    target.DamageAt = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DamageMobStateComponent target,
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
    DamageMobStateComponent target1 = (DamageMobStateComponent) target;
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
    DamageMobStateComponent target1 = (DamageMobStateComponent) target;
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
    DamageMobStateComponent target1 = (DamageMobStateComponent) target;
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
    DamageMobStateComponent target1 = (DamageMobStateComponent) target;
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
  virtual DamageMobStateComponent Component.Instantiate() => new DamageMobStateComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageMobStateComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DamageMobStateComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<DamageMobStateComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      DamageMobStateComponent component,
      ref EntityUnpausedEvent args)
    {
      component.DamageAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DamageMobStateComponent_AutoState : IComponentState
  {
    public 
    #nullable enable
    DamageSpecifier CritDamage;
    public DamageSpecifier NonDeadDamage;
    public TimeSpan Cooldown;
    public TimeSpan DamageAt;

    public DamageMobStateComponent.DamageMobStateComponent_AutoState ShallowClone()
    {
      return new DamageMobStateComponent.DamageMobStateComponent_AutoState()
      {
        CritDamage = this.CritDamage,
        NonDeadDamage = this.NonDeadDamage,
        Cooldown = this.Cooldown,
        DamageAt = this.DamageAt
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DamageMobStateComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<DamageMobStateComponent>("CritDamage", "NonDeadDamage", "Cooldown", "DamageAt");
      this.SubscribeLocalEvent<DamageMobStateComponent, ComponentGetState>(new ComponentEventRefHandler<DamageMobStateComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DamageMobStateComponent, ComponentHandleState>(new ComponentEventRefHandler<DamageMobStateComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      DamageMobStateComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new DamageMobStateComponent.CritDamage_FieldComponentState()
            {
              CritDamage = component.CritDamage
            };
            return;
          case 2:
            args.State = (IComponentState) new DamageMobStateComponent.NonDeadDamage_FieldComponentState()
            {
              NonDeadDamage = component.NonDeadDamage
            };
            return;
          case 4:
            args.State = (IComponentState) new DamageMobStateComponent.Cooldown_FieldComponentState()
            {
              Cooldown = component.Cooldown
            };
            return;
          case 8:
            args.State = (IComponentState) new DamageMobStateComponent.DamageAt_FieldComponentState()
            {
              DamageAt = component.DamageAt
            };
            return;
        }
      }
      args.State = (IComponentState) new DamageMobStateComponent.DamageMobStateComponent_AutoState()
      {
        CritDamage = component.CritDamage,
        NonDeadDamage = component.NonDeadDamage,
        Cooldown = component.Cooldown,
        DamageAt = component.DamageAt
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DamageMobStateComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case DamageMobStateComponent.CritDamage_FieldComponentState fieldComponentState1:
          component.CritDamage = fieldComponentState1.CritDamage;
          break;
        case DamageMobStateComponent.NonDeadDamage_FieldComponentState fieldComponentState2:
          component.NonDeadDamage = fieldComponentState2.NonDeadDamage;
          break;
        case DamageMobStateComponent.Cooldown_FieldComponentState fieldComponentState3:
          component.Cooldown = fieldComponentState3.Cooldown;
          break;
        case DamageMobStateComponent.DamageAt_FieldComponentState fieldComponentState4:
          component.DamageAt = fieldComponentState4.DamageAt;
          break;
        case DamageMobStateComponent.DamageMobStateComponent_AutoState componentAutoState:
          component.CritDamage = componentAutoState.CritDamage;
          component.NonDeadDamage = componentAutoState.NonDeadDamage;
          component.Cooldown = componentAutoState.Cooldown;
          component.DamageAt = componentAutoState.DamageAt;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class CritDamage_FieldComponentState : 
    IComponentDeltaState<DamageMobStateComponent.DamageMobStateComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public DamageSpecifier CritDamage;

    public void ApplyToFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      fullState.CritDamage = this.CritDamage;
    }

    public DamageMobStateComponent.DamageMobStateComponent_AutoState CreateNewFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NonDeadDamage_FieldComponentState : 
    IComponentDeltaState<DamageMobStateComponent.DamageMobStateComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public DamageSpecifier NonDeadDamage;

    public void ApplyToFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      fullState.NonDeadDamage = this.NonDeadDamage;
    }

    public DamageMobStateComponent.DamageMobStateComponent_AutoState CreateNewFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Cooldown_FieldComponentState : 
    IComponentDeltaState<DamageMobStateComponent.DamageMobStateComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan Cooldown;

    public void ApplyToFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      fullState.Cooldown = this.Cooldown;
    }

    public DamageMobStateComponent.DamageMobStateComponent_AutoState CreateNewFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class DamageAt_FieldComponentState : 
    IComponentDeltaState<DamageMobStateComponent.DamageMobStateComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan DamageAt;

    public void ApplyToFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      fullState.DamageAt = this.DamageAt;
    }

    public DamageMobStateComponent.DamageMobStateComponent_AutoState CreateNewFullState(
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState)
    {
      DamageMobStateComponent.DamageMobStateComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
