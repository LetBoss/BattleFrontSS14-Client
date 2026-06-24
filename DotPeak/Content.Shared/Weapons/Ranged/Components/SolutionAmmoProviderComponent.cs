// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Components.SolutionAmmoProviderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
[Access(new Type[] {typeof (SharedGunSystem)})]
public sealed class SolutionAmmoProviderComponent : 
  Component,
  ISerializationGenerated<SolutionAmmoProviderComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public string SolutionId;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FireCost = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Shots;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxShots;
  [DataField("proto", false, 1, false, false, null)]
  public EntProtoId Prototype;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionAmmoProviderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SolutionAmmoProviderComponent) target1;
    if (serialization.TryCustomCopy<SolutionAmmoProviderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.SolutionId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionId, ref target2, hookCtx, false, context))
      target2 = this.SolutionId;
    target.SolutionId = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireCost, ref target3, hookCtx, false, context))
      target3 = this.FireCost;
    target.FireCost = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Shots, ref target4, hookCtx, false, context))
      target4 = this.Shots;
    target.Shots = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxShots, ref target5, hookCtx, false, context))
      target5 = this.MaxShots;
    target.MaxShots = target5;
    EntProtoId target6 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionAmmoProviderComponent target,
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
    SolutionAmmoProviderComponent target1 = (SolutionAmmoProviderComponent) target;
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
    SolutionAmmoProviderComponent target1 = (SolutionAmmoProviderComponent) target;
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
    SolutionAmmoProviderComponent target1 = (SolutionAmmoProviderComponent) target;
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
    SolutionAmmoProviderComponent target1 = (SolutionAmmoProviderComponent) target;
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
  virtual SolutionAmmoProviderComponent Component.Instantiate()
  {
    return new SolutionAmmoProviderComponent();
  }

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
  public sealed class SolutionAmmoProviderComponent_AutoState : IComponentState
  {
    public string SolutionId;
    public float FireCost;
    public int Shots;
    public int MaxShots;

    public SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState ShallowClone()
    {
      return new SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState()
      {
        SolutionId = this.SolutionId,
        FireCost = this.FireCost,
        Shots = this.Shots,
        MaxShots = this.MaxShots
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionAmmoProviderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<SolutionAmmoProviderComponent>("SolutionId", "FireCost", "Shots", "MaxShots");
      this.SubscribeLocalEvent<SolutionAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<SolutionAmmoProviderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SolutionAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<SolutionAmmoProviderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SolutionAmmoProviderComponent component,
      ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new SolutionAmmoProviderComponent.SolutionId_FieldComponentState()
            {
              SolutionId = component.SolutionId
            };
            return;
          case 2:
            args.State = (IComponentState) new SolutionAmmoProviderComponent.FireCost_FieldComponentState()
            {
              FireCost = component.FireCost
            };
            return;
          case 4:
            args.State = (IComponentState) new SolutionAmmoProviderComponent.Shots_FieldComponentState()
            {
              Shots = component.Shots
            };
            return;
          case 8:
            args.State = (IComponentState) new SolutionAmmoProviderComponent.MaxShots_FieldComponentState()
            {
              MaxShots = component.MaxShots
            };
            return;
        }
      }
      args.State = (IComponentState) new SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState()
      {
        SolutionId = component.SolutionId,
        FireCost = component.FireCost,
        Shots = component.Shots,
        MaxShots = component.MaxShots
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SolutionAmmoProviderComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case SolutionAmmoProviderComponent.SolutionId_FieldComponentState fieldComponentState1:
          component.SolutionId = fieldComponentState1.SolutionId;
          break;
        case SolutionAmmoProviderComponent.FireCost_FieldComponentState fieldComponentState2:
          component.FireCost = fieldComponentState2.FireCost;
          break;
        case SolutionAmmoProviderComponent.Shots_FieldComponentState fieldComponentState3:
          component.Shots = fieldComponentState3.Shots;
          break;
        case SolutionAmmoProviderComponent.MaxShots_FieldComponentState fieldComponentState4:
          component.MaxShots = fieldComponentState4.MaxShots;
          break;
        case SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState componentAutoState:
          component.SolutionId = componentAutoState.SolutionId;
          component.FireCost = componentAutoState.FireCost;
          component.Shots = componentAutoState.Shots;
          component.MaxShots = componentAutoState.MaxShots;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class SolutionId_FieldComponentState : 
    IComponentDeltaState<SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public string SolutionId;

    public void ApplyToFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      fullState.SolutionId = this.SolutionId;
    }

    public SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState CreateNewFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class FireCost_FieldComponentState : 
    IComponentDeltaState<SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public float FireCost;

    public void ApplyToFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      fullState.FireCost = this.FireCost;
    }

    public SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState CreateNewFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Shots_FieldComponentState : 
    IComponentDeltaState<SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int Shots;

    public void ApplyToFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      fullState.Shots = this.Shots;
    }

    public SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState CreateNewFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class MaxShots_FieldComponentState : 
    IComponentDeltaState<SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public int MaxShots;

    public void ApplyToFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      fullState.MaxShots = this.MaxShots;
    }

    public SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState CreateNewFullState(
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState)
    {
      SolutionAmmoProviderComponent.SolutionAmmoProviderComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
