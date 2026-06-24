// Decompiled with JetBrains decompiler
// Type: Content.Shared.Strip.Components.ThievingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Strip.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, true)]
public sealed class ThievingComponent : 
  Component,
  ISerializationGenerated<ThievingComponent>,
  ISerializationGenerated,
  IComponentDelta,
  IComponent,
  ISerializationGenerated<IComponent>,
  ISerializationGenerated<IComponentDelta>
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan StripTimeReduction = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Stealthy;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> StealthyAlertProtoId = (ProtoId<AlertPrototype>) nameof (Stealthy);

  public override bool SendOnlyToOwner => true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ThievingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ThievingComponent) target1;
    if (serialization.TryCustomCopy<ThievingComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.StripTimeReduction, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.StripTimeReduction, hookCtx, context);
    target.StripTimeReduction = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Stealthy, ref target3, hookCtx, false, context))
      target3 = this.Stealthy;
    target.Stealthy = target3;
    ProtoId<AlertPrototype> target4 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.StealthyAlertProtoId, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.StealthyAlertProtoId, hookCtx, context);
    target.StealthyAlertProtoId = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ThievingComponent target,
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
    ThievingComponent target1 = (ThievingComponent) target;
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
    ThievingComponent target1 = (ThievingComponent) target;
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
    ThievingComponent target1 = (ThievingComponent) target;
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
    ThievingComponent target1 = (ThievingComponent) target;
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
  virtual ThievingComponent Component.Instantiate() => new ThievingComponent();

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
  public sealed class ThievingComponent_AutoState : IComponentState
  {
    public TimeSpan StripTimeReduction;
    public bool Stealthy;

    public ThievingComponent.ThievingComponent_AutoState ShallowClone()
    {
      return new ThievingComponent.ThievingComponent_AutoState()
      {
        StripTimeReduction = this.StripTimeReduction,
        Stealthy = this.Stealthy
      };
    }
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ThievingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.EntityManager.ComponentFactory.RegisterNetworkedFields<ThievingComponent>("StripTimeReduction", "Stealthy");
      this.SubscribeLocalEvent<ThievingComponent, ComponentGetState>(new ComponentEventRefHandler<ThievingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ThievingComponent, ComponentHandleState>(new ComponentEventRefHandler<ThievingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, ThievingComponent component, ref ComponentGetState args)
    {
      IComponentDelta componentDelta = (IComponentDelta) component;
      if (componentDelta != null && args.FromTick > component.CreationTick && componentDelta.LastFieldUpdate >= args.FromTick)
      {
        switch (this.EntityManager.GetModifiedFields((IComponentDelta) component, args.FromTick))
        {
          case 1:
            args.State = (IComponentState) new ThievingComponent.StripTimeReduction_FieldComponentState()
            {
              StripTimeReduction = component.StripTimeReduction
            };
            return;
          case 2:
            args.State = (IComponentState) new ThievingComponent.Stealthy_FieldComponentState()
            {
              Stealthy = component.Stealthy
            };
            return;
        }
      }
      args.State = (IComponentState) new ThievingComponent.ThievingComponent_AutoState()
      {
        StripTimeReduction = component.StripTimeReduction,
        Stealthy = component.Stealthy
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ThievingComponent component,
      ref ComponentHandleState args)
    {
      switch (args.Current)
      {
        case ThievingComponent.StripTimeReduction_FieldComponentState fieldComponentState1:
          component.StripTimeReduction = fieldComponentState1.StripTimeReduction;
          break;
        case ThievingComponent.Stealthy_FieldComponentState fieldComponentState2:
          component.Stealthy = fieldComponentState2.Stealthy;
          break;
        case ThievingComponent.ThievingComponent_AutoState componentAutoState:
          component.StripTimeReduction = componentAutoState.StripTimeReduction;
          component.Stealthy = componentAutoState.Stealthy;
          break;
      }
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class StripTimeReduction_FieldComponentState : 
    IComponentDeltaState<ThievingComponent.ThievingComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public TimeSpan StripTimeReduction;

    public void ApplyToFullState(
      ThievingComponent.ThievingComponent_AutoState fullState)
    {
      fullState.StripTimeReduction = this.StripTimeReduction;
    }

    public ThievingComponent.ThievingComponent_AutoState CreateNewFullState(
      ThievingComponent.ThievingComponent_AutoState fullState)
    {
      ThievingComponent.ThievingComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }

  [NetSerializable]
  [Serializable]
  public sealed class Stealthy_FieldComponentState : 
    IComponentDeltaState<ThievingComponent.ThievingComponent_AutoState>,
    IComponentDeltaState,
    IComponentState
  {
    public bool Stealthy;

    public void ApplyToFullState(
      ThievingComponent.ThievingComponent_AutoState fullState)
    {
      fullState.Stealthy = this.Stealthy;
    }

    public ThievingComponent.ThievingComponent_AutoState CreateNewFullState(
      ThievingComponent.ThievingComponent_AutoState fullState)
    {
      ThievingComponent.ThievingComponent_AutoState fullState1 = fullState.ShallowClone();
      this.ApplyToFullState(fullState1);
      return fullState1;
    }
  }
}
