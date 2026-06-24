// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.MobCollisionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
public sealed class MobCollisionComponent : 
  Component,
  ISerializationGenerated<MobCollisionComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Colliding;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BufferAccumulator = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SpeedModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MinimumSpeedModifier = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Strength = 50f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string FixtureId = "flammable";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Direction;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MobCollisionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MobCollisionComponent) target1;
    if (serialization.TryCustomCopy<MobCollisionComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Colliding, ref target2, hookCtx, false, context))
      target2 = this.Colliding;
    target.Colliding = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BufferAccumulator, ref target3, hookCtx, false, context))
      target3 = this.BufferAccumulator;
    target.BufferAccumulator = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedModifier, ref target4, hookCtx, false, context))
      target4 = this.SpeedModifier;
    target.SpeedModifier = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MinimumSpeedModifier, ref target5, hookCtx, false, context))
      target5 = this.MinimumSpeedModifier;
    target.MinimumSpeedModifier = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Strength, ref target6, hookCtx, false, context))
      target6 = this.Strength;
    target.Strength = target6;
    string target7 = (string) null;
    if (this.FixtureId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FixtureId, ref target7, hookCtx, false, context))
      target7 = this.FixtureId;
    target.FixtureId = target7;
    Vector2 target8 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Direction, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2>(this.Direction, hookCtx, context);
    target.Direction = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MobCollisionComponent target,
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
    MobCollisionComponent target1 = (MobCollisionComponent) target;
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
    MobCollisionComponent target1 = (MobCollisionComponent) target;
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
    MobCollisionComponent target1 = (MobCollisionComponent) target;
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
    MobCollisionComponent target1 = (MobCollisionComponent) target;
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
  virtual MobCollisionComponent Component.Instantiate() => new MobCollisionComponent();

  IComponentDelta IComponentDelta.Instantiate() => (IComponentDelta) this.Instantiate();

  IComponentDelta ISerializationGenerated<IComponentDelta>.Instantiate()
  {
    return (IComponentDelta) this.Instantiate();
  }

  public GameTick LastFieldUpdate { get; set; } = GameTick.Zero;

  public GameTick[] LastModifiedFields { get; set; } = Array.Empty<GameTick>();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MobCollisionComponent_AutoState : IComponentState
  {
    public bool Colliding;
    public float BufferAccumulator;
    public float SpeedModifier;
    public float MinimumSpeedModifier;
    public float Strength;
    public string FixtureId;
    public Vector2 Direction;

    public MobCollisionComponent.MobCollisionComponent_AutoState ShallowClone()
    {
      return new MobCollisionComponent.MobCollisionComponent_AutoState()
      {
        Colliding = this.Colliding,
        BufferAccumulator = this.BufferAccumulator,
        SpeedModifier = this.SpeedModifier,
        MinimumSpeedModifier = this.MinimumSpeedModifier,
        Strength = this.Strength,
        FixtureId = this.FixtureId,
        Direction = this.Direction
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MobCollisionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<MobCollisionComponent>("Colliding", "BufferAccumulator", "SpeedModifier", "MinimumSpeedModifier", "Strength", "FixtureId", "Direction");
      this.SubscribeLocalEvent<MobCollisionComponent, ComponentGetState>(new ComponentEventRefHandler<MobCollisionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MobCollisionComponent, ComponentHandleState>(new ComponentEventRefHandler<MobCollisionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      MobCollisionComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        uint modifiedFields = this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick);
        if (modifiedFields <= 8U)
        {
          switch ((int) modifiedFields - 1)
          {
            case 0:
              args.State = (IComponentState) new MobCollisionComponent.Colliding_FieldComponentState()
              {
                Colliding = component.Colliding
              };
              return;
            case 1:
              args.State = (IComponentState) new MobCollisionComponent.BufferAccumulator_FieldComponentState()
              {
                BufferAccumulator = component.BufferAccumulator
              };
              return;
            case 2:
              break;
            case 3:
              args.State = (IComponentState) new MobCollisionComponent.SpeedModifier_FieldComponentState()
              {
                SpeedModifier = component.SpeedModifier
              };
              return;
            default:
              if (modifiedFields == 8U)
              {
                args.State = (IComponentState) new MobCollisionComponent.MinimumSpeedModifier_FieldComponentState()
                {
                  MinimumSpeedModifier = component.MinimumSpeedModifier
                };
                return;
              }
              break;
          }
        }
        else if (modifiedFields != 16U /*0x10*/)
        {
          if (modifiedFields != 32U /*0x20*/)
          {
            if (modifiedFields == 64U /*0x40*/)
            {
              args.State = (IComponentState) new MobCollisionComponent.Direction_FieldComponentState()
              {
                Direction = component.Direction
              };
              return;
            }
          }
          else
          {
            args.State = (IComponentState) new MobCollisionComponent.FixtureId_FieldComponentState()
            {
              FixtureId = component.FixtureId
            };
            return;
          }
        }
        else
        {
          args.State = (IComponentState) new MobCollisionComponent.Strength_FieldComponentState()
          {
            Strength = component.Strength
          };
          return;
        }
      }
      args.State = (IComponentState) new MobCollisionComponent.MobCollisionComponent_AutoState()
      {
        Colliding = component.Colliding,
        BufferAccumulator = component.BufferAccumulator,
        SpeedModifier = component.SpeedModifier,
        MinimumSpeedModifier = component.MinimumSpeedModifier,
        Strength = component.Strength,
        FixtureId = component.FixtureId,
        Direction = component.Direction
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MobCollisionComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case MobCollisionComponent.Colliding_FieldComponentState fieldComponentState1:
          component.Colliding = fieldComponentState1.Colliding;
          break;
        case MobCollisionComponent.BufferAccumulator_FieldComponentState fieldComponentState2:
          component.BufferAccumulator = fieldComponentState2.BufferAccumulator;
          break;
        case MobCollisionComponent.SpeedModifier_FieldComponentState fieldComponentState3:
          component.SpeedModifier = fieldComponentState3.SpeedModifier;
          break;
        case MobCollisionComponent.MinimumSpeedModifier_FieldComponentState fieldComponentState4:
          component.MinimumSpeedModifier = fieldComponentState4.MinimumSpeedModifier;
          break;
        case MobCollisionComponent.Strength_FieldComponentState fieldComponentState5:
          component.Strength = fieldComponentState5.Strength;
          break;
        case MobCollisionComponent.FixtureId_FieldComponentState fieldComponentState6:
          component.FixtureId = fieldComponentState6.FixtureId;
          break;
        case MobCollisionComponent.Direction_FieldComponentState fieldComponentState7:
          component.Direction = fieldComponentState7.Direction;
          break;
        case MobCollisionComponent.MobCollisionComponent_AutoState componentAutoState:
          component.Colliding = componentAutoState.Colliding;
          component.BufferAccumulator = componentAutoState.BufferAccumulator;
          component.SpeedModifier = componentAutoState.SpeedModifier;
          component.MinimumSpeedModifier = componentAutoState.MinimumSpeedModifier;
          component.Strength = componentAutoState.Strength;
          component.FixtureId = componentAutoState.FixtureId;
          component.Direction = componentAutoState.Direction;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Colliding_FieldComponentState : 
    IComponentDeltaState<MobCollisionComponent.MobCollisionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Colliding;

    public void ApplyToFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      fullState.Colliding = this.Colliding;
    }

    public MobCollisionComponent.MobCollisionComponent_AutoState CreateNewFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      MobCollisionComponent.MobCollisionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class BufferAccumulator_FieldComponentState : 
    IComponentDeltaState<MobCollisionComponent.MobCollisionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float BufferAccumulator;

    public void ApplyToFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      fullState.BufferAccumulator = this.BufferAccumulator;
    }

    public MobCollisionComponent.MobCollisionComponent_AutoState CreateNewFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      MobCollisionComponent.MobCollisionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SpeedModifier_FieldComponentState : 
    IComponentDeltaState<MobCollisionComponent.MobCollisionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float SpeedModifier;

    public void ApplyToFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      fullState.SpeedModifier = this.SpeedModifier;
    }

    public MobCollisionComponent.MobCollisionComponent_AutoState CreateNewFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      MobCollisionComponent.MobCollisionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MinimumSpeedModifier_FieldComponentState : 
    IComponentDeltaState<MobCollisionComponent.MobCollisionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float MinimumSpeedModifier;

    public void ApplyToFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      fullState.MinimumSpeedModifier = this.MinimumSpeedModifier;
    }

    public MobCollisionComponent.MobCollisionComponent_AutoState CreateNewFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      MobCollisionComponent.MobCollisionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Strength_FieldComponentState : 
    IComponentDeltaState<MobCollisionComponent.MobCollisionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float Strength;

    public void ApplyToFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      fullState.Strength = this.Strength;
    }

    public MobCollisionComponent.MobCollisionComponent_AutoState CreateNewFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      MobCollisionComponent.MobCollisionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FixtureId_FieldComponentState : 
    IComponentDeltaState<MobCollisionComponent.MobCollisionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string FixtureId;

    public void ApplyToFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      fullState.FixtureId = this.FixtureId;
    }

    public MobCollisionComponent.MobCollisionComponent_AutoState CreateNewFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      MobCollisionComponent.MobCollisionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Direction_FieldComponentState : 
    IComponentDeltaState<MobCollisionComponent.MobCollisionComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public Vector2 Direction;

    public void ApplyToFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      fullState.Direction = this.Direction;
    }

    public MobCollisionComponent.MobCollisionComponent_AutoState CreateNewFullState(
      MobCollisionComponent.MobCollisionComponent_AutoState fullState)
    {
      MobCollisionComponent.MobCollisionComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
