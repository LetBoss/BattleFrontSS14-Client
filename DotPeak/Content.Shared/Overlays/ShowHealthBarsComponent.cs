// Decompiled with JetBrains decompiler
// Type: Content.Shared.Overlays.ShowHealthBarsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.StatusIcon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Overlays;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class ShowHealthBarsComponent : 
  Component,
  ISerializationGenerated<ShowHealthBarsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<ProtoId<DamageContainerPrototype>> DamageContainers = new List<ProtoId<DamageContainerPrototype>>()
  {
    (ProtoId<DamageContainerPrototype>) "Biological"
  };
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HealthIconPrototype>? HealthStatusIcon = (ProtoId<HealthIconPrototype>?) "HealthIconFine";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ShowHealthBarsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ShowHealthBarsComponent) target1;
    if (serialization.TryCustomCopy<ShowHealthBarsComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ProtoId<DamageContainerPrototype>> target2 = (List<ProtoId<DamageContainerPrototype>>) null;
    if (this.DamageContainers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<DamageContainerPrototype>>>(this.DamageContainers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ProtoId<DamageContainerPrototype>>>(this.DamageContainers, hookCtx, context);
    target.DamageContainers = target2;
    ProtoId<HealthIconPrototype>? target3 = new ProtoId<HealthIconPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<HealthIconPrototype>?>(this.HealthStatusIcon, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<HealthIconPrototype>?>(this.HealthStatusIcon, hookCtx, context);
    target.HealthStatusIcon = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ShowHealthBarsComponent target,
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
    ShowHealthBarsComponent target1 = (ShowHealthBarsComponent) target;
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
    ShowHealthBarsComponent target1 = (ShowHealthBarsComponent) target;
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
    ShowHealthBarsComponent target1 = (ShowHealthBarsComponent) target;
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

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ShowHealthBarsComponent Component.Instantiate() => new ShowHealthBarsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ShowHealthBarsComponent_AutoState : IComponentState
  {
    public List<ProtoId<DamageContainerPrototype>> DamageContainers;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ShowHealthBarsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ShowHealthBarsComponent, ComponentGetState>(new ComponentEventRefHandler<ShowHealthBarsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ShowHealthBarsComponent, ComponentHandleState>(new ComponentEventRefHandler<ShowHealthBarsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ShowHealthBarsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ShowHealthBarsComponent.ShowHealthBarsComponent_AutoState()
      {
        DamageContainers = component.DamageContainers
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ShowHealthBarsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ShowHealthBarsComponent.ShowHealthBarsComponent_AutoState current))
        return;
      component.DamageContainers = current.DamageContainers == null ? (List<ProtoId<DamageContainerPrototype>>) null : new List<ProtoId<DamageContainerPrototype>>((IEnumerable<ProtoId<DamageContainerPrototype>>) current.DamageContainers);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, ShowHealthBarsComponent>(uid, component, ref args1);
    }
  }
}
