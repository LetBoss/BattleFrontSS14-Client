// Decompiled with JetBrains decompiler
// Type: Content.Shared.ProximityDetection.Components.ProximityDetectorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ProximityDetection.Systems;
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
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ProximityDetection.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (ProximityDetectionSystem)})]
public sealed class ProximityDetectorComponent : 
  Component,
  ISerializationGenerated<ProximityDetectorComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, true, false, null)]
  public ComponentRegistry Components;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public EntityUid? Target;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public float Distance = float.PositiveInfinity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Range = 10f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateCooldown = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextUpdate = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProximityDetectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProximityDetectorComponent) target1;
    if (serialization.TryCustomCopy<ProximityDetectorComponent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry target2 = (ComponentRegistry) null;
    if (this.Components == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Components, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ComponentRegistry>(this.Components, hookCtx, context);
    target.Components = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref target3, hookCtx, false, context))
      target3 = this.Range;
    target.Range = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateCooldown, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.UpdateCooldown, hookCtx, context);
    target.UpdateCooldown = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context);
    target.NextUpdate = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProximityDetectorComponent target,
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
    ProximityDetectorComponent target1 = (ProximityDetectorComponent) target;
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
    ProximityDetectorComponent target1 = (ProximityDetectorComponent) target;
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
    ProximityDetectorComponent target1 = (ProximityDetectorComponent) target;
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
    ProximityDetectorComponent target1 = (ProximityDetectorComponent) target;
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
  virtual ProximityDetectorComponent Component.Instantiate() => new ProximityDetectorComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ProximityDetectorComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ProximityDetectorComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ProximityDetectorComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ProximityDetectorComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextUpdate += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ProximityDetectorComponent_AutoState : IComponentState
  {
    public NetEntity? Target;
    public float Distance;
    public float Range;
    public TimeSpan UpdateCooldown;
    public TimeSpan NextUpdate;

    public 
    #nullable enable
    ProximityDetectorComponent.ProximityDetectorComponent_AutoState ShallowClone()
    {
      return new ProximityDetectorComponent.ProximityDetectorComponent_AutoState()
      {
        Target = this.Target,
        Distance = this.Distance,
        Range = this.Range,
        UpdateCooldown = this.UpdateCooldown,
        NextUpdate = this.NextUpdate
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ProximityDetectorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<ProximityDetectorComponent>("Target", "Distance", "Range", "UpdateCooldown", "NextUpdate");
      this.SubscribeLocalEvent<ProximityDetectorComponent, ComponentGetState>(new ComponentEventRefHandler<ProximityDetectorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ProximityDetectorComponent, ComponentHandleState>(new ComponentEventRefHandler<ProximityDetectorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ProximityDetectorComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new ProximityDetectorComponent.Target_FieldComponentState()
            {
              Target = this.GetNetEntity(component.Target)
            };
            return;
          case 2:
            args.State = (IComponentState) new ProximityDetectorComponent.Distance_FieldComponentState()
            {
              Distance = component.Distance
            };
            return;
          case 4:
            args.State = (IComponentState) new ProximityDetectorComponent.Range_FieldComponentState()
            {
              Range = component.Range
            };
            return;
          case 8:
            args.State = (IComponentState) new ProximityDetectorComponent.UpdateCooldown_FieldComponentState()
            {
              UpdateCooldown = component.UpdateCooldown
            };
            return;
          case 16 /*0x10*/:
            args.State = (IComponentState) new ProximityDetectorComponent.NextUpdate_FieldComponentState()
            {
              NextUpdate = component.NextUpdate
            };
            return;
        }
      }
      args.State = (IComponentState) new ProximityDetectorComponent.ProximityDetectorComponent_AutoState()
      {
        Target = this.GetNetEntity(component.Target),
        Distance = component.Distance,
        Range = component.Range,
        UpdateCooldown = component.UpdateCooldown,
        NextUpdate = component.NextUpdate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ProximityDetectorComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case ProximityDetectorComponent.Target_FieldComponentState fieldComponentState1:
          component.Target = this.EnsureEntity<ProximityDetectorComponent>(fieldComponentState1.Target, uid);
          break;
        case ProximityDetectorComponent.Distance_FieldComponentState fieldComponentState2:
          component.Distance = fieldComponentState2.Distance;
          break;
        case ProximityDetectorComponent.Range_FieldComponentState fieldComponentState3:
          component.Range = fieldComponentState3.Range;
          break;
        case ProximityDetectorComponent.UpdateCooldown_FieldComponentState fieldComponentState4:
          component.UpdateCooldown = fieldComponentState4.UpdateCooldown;
          break;
        case ProximityDetectorComponent.NextUpdate_FieldComponentState fieldComponentState5:
          component.NextUpdate = fieldComponentState5.NextUpdate;
          break;
        case ProximityDetectorComponent.ProximityDetectorComponent_AutoState componentAutoState:
          component.Target = this.EnsureEntity<ProximityDetectorComponent>(componentAutoState.Target, uid);
          component.Distance = componentAutoState.Distance;
          component.Range = componentAutoState.Range;
          component.UpdateCooldown = componentAutoState.UpdateCooldown;
          component.NextUpdate = componentAutoState.NextUpdate;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Target_FieldComponentState : 
    IComponentDeltaState<ProximityDetectorComponent.ProximityDetectorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public NetEntity? Target;

    public void ApplyToFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      fullState.Target = this.Target;
    }

    public ProximityDetectorComponent.ProximityDetectorComponent_AutoState CreateNewFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Distance_FieldComponentState : 
    IComponentDeltaState<ProximityDetectorComponent.ProximityDetectorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float Distance;

    public void ApplyToFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      fullState.Distance = this.Distance;
    }

    public ProximityDetectorComponent.ProximityDetectorComponent_AutoState CreateNewFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Range_FieldComponentState : 
    IComponentDeltaState<ProximityDetectorComponent.ProximityDetectorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float Range;

    public void ApplyToFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      fullState.Range = this.Range;
    }

    public ProximityDetectorComponent.ProximityDetectorComponent_AutoState CreateNewFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class UpdateCooldown_FieldComponentState : 
    IComponentDeltaState<ProximityDetectorComponent.ProximityDetectorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan UpdateCooldown;

    public void ApplyToFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      fullState.UpdateCooldown = this.UpdateCooldown;
    }

    public ProximityDetectorComponent.ProximityDetectorComponent_AutoState CreateNewFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextUpdate_FieldComponentState : 
    IComponentDeltaState<ProximityDetectorComponent.ProximityDetectorComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan NextUpdate;

    public void ApplyToFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      fullState.NextUpdate = this.NextUpdate;
    }

    public ProximityDetectorComponent.ProximityDetectorComponent_AutoState CreateNewFullState(
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState)
    {
      ProximityDetectorComponent.ProximityDetectorComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
