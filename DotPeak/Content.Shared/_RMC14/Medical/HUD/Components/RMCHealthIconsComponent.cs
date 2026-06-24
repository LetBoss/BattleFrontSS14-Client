// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Medical.HUD.Components.RMCHealthIconsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Medical.HUD.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCHealthIconsComponent : 
  Component,
  ISerializationGenerated<RMCHealthIconsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>> Icons = new Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>()
  {
    [RMCHealthIconTypes.Healthy] = (ProtoId<HealthIconPrototype>) "CMHealthIconHealthy",
    [RMCHealthIconTypes.DeadDefib] = (ProtoId<HealthIconPrototype>) "CMHealthIconDeadDefib",
    [RMCHealthIconTypes.DeadClose] = (ProtoId<HealthIconPrototype>) "CMHealthIconDeadClose",
    [RMCHealthIconTypes.DeadAlmost] = (ProtoId<HealthIconPrototype>) "CMHealthIconDeadAlmost",
    [RMCHealthIconTypes.DeadDNR] = (ProtoId<HealthIconPrototype>) "CMHealthIconDeadDNR",
    [RMCHealthIconTypes.Dead] = (ProtoId<HealthIconPrototype>) "CMHealthIconDead",
    [RMCHealthIconTypes.HCDead] = (ProtoId<HealthIconPrototype>) "CMHealthIconHCDead"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCHealthIconsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCHealthIconsComponent) target1;
    if (serialization.TryCustomCopy<RMCHealthIconsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>> target2 = (Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>) null;
    if (this.Icons == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>>(this.Icons, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>>(this.Icons, hookCtx, context);
    target.Icons = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCHealthIconsComponent target,
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
    RMCHealthIconsComponent target1 = (RMCHealthIconsComponent) target;
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
    RMCHealthIconsComponent target1 = (RMCHealthIconsComponent) target;
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
    RMCHealthIconsComponent target1 = (RMCHealthIconsComponent) target;
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
  virtual RMCHealthIconsComponent Component.Instantiate() => new RMCHealthIconsComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCHealthIconsComponent_AutoState : IComponentState
  {
    public Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>> Icons;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCHealthIconsComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCHealthIconsComponent, ComponentGetState>(new ComponentEventRefHandler<RMCHealthIconsComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCHealthIconsComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCHealthIconsComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCHealthIconsComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCHealthIconsComponent.RMCHealthIconsComponent_AutoState()
      {
        Icons = component.Icons
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCHealthIconsComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCHealthIconsComponent.RMCHealthIconsComponent_AutoState current))
        return;
      component.Icons = current.Icons == null ? (Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>) null : new Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>((IDictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>) current.Icons);
    }
  }
}
