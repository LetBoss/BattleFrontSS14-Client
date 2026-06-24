// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Spray.ActiveAcidSprayingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Line;
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
namespace Content.Shared._RMC14.Xenonids.Spray;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoSprayAcidSystem)})]
public sealed class ActiveAcidSprayingComponent : 
  Component,
  ISerializationGenerated<ActiveAcidSprayingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Acid;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<LineTile> Spawn = new List<LineTile>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Blocker;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Chain;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActiveAcidSprayingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActiveAcidSprayingComponent) target1;
    if (serialization.TryCustomCopy<ActiveAcidSprayingComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Acid, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Acid, hookCtx, context);
    target.Acid = target2;
    List<LineTile> target3 = (List<LineTile>) null;
    if (this.Spawn == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LineTile>>(this.Spawn, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<List<LineTile>>(this.Spawn, hookCtx, context);
    target.Spawn = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Blocker, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Blocker, hookCtx, context);
    target.Blocker = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Chain, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Chain, hookCtx, context);
    target.Chain = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActiveAcidSprayingComponent target,
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
    ActiveAcidSprayingComponent target1 = (ActiveAcidSprayingComponent) target;
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
    ActiveAcidSprayingComponent target1 = (ActiveAcidSprayingComponent) target;
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
    ActiveAcidSprayingComponent target1 = (ActiveAcidSprayingComponent) target;
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
  virtual ActiveAcidSprayingComponent Component.Instantiate() => new ActiveAcidSprayingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ActiveAcidSprayingComponent_AutoState : IComponentState
  {
    public EntProtoId Acid;
    public List<LineTile> Spawn;
    public NetEntity? Blocker;
    public NetEntity? Chain;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActiveAcidSprayingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ActiveAcidSprayingComponent, ComponentGetState>(new ComponentEventRefHandler<ActiveAcidSprayingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ActiveAcidSprayingComponent, ComponentHandleState>(new ComponentEventRefHandler<ActiveAcidSprayingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ActiveAcidSprayingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ActiveAcidSprayingComponent.ActiveAcidSprayingComponent_AutoState()
      {
        Acid = component.Acid,
        Spawn = component.Spawn,
        Blocker = this.GetNetEntity(component.Blocker),
        Chain = this.GetNetEntity(component.Chain)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ActiveAcidSprayingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ActiveAcidSprayingComponent.ActiveAcidSprayingComponent_AutoState current))
        return;
      component.Acid = current.Acid;
      component.Spawn = current.Spawn == null ? (List<LineTile>) null : new List<LineTile>((IEnumerable<LineTile>) current.Spawn);
      component.Blocker = this.EnsureEntity<ActiveAcidSprayingComponent>(current.Blocker, uid);
      component.Chain = this.EnsureEntity<ActiveAcidSprayingComponent>(current.Chain, uid);
    }
  }
}
