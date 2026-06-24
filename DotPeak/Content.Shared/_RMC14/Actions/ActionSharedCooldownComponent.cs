// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Actions.ActionSharedCooldownComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Actions;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCActionsSystem)})]
public sealed class ActionSharedCooldownComponent : 
  Component,
  ISerializationGenerated<ActionSharedCooldownComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? Id;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntProtoId> Ids = new HashSet<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntProtoId> ActiveIds = new HashSet<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnPerform = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActionSharedCooldownComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActionSharedCooldownComponent) target1;
    if (serialization.TryCustomCopy<ActionSharedCooldownComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId? target2 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.Id, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId?>(this.Id, hookCtx, context);
    target.Id = target2;
    HashSet<EntProtoId> target3 = (HashSet<EntProtoId>) null;
    if (this.Ids == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntProtoId>>(this.Ids, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<EntProtoId>>(this.Ids, hookCtx, context);
    target.Ids = target3;
    HashSet<EntProtoId> target4 = (HashSet<EntProtoId>) null;
    if (this.ActiveIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntProtoId>>(this.ActiveIds, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<EntProtoId>>(this.ActiveIds, hookCtx, context);
    target.ActiveIds = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnPerform, ref target6, hookCtx, false, context))
      target6 = this.OnPerform;
    target.OnPerform = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActionSharedCooldownComponent target,
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
    ActionSharedCooldownComponent target1 = (ActionSharedCooldownComponent) target;
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
    ActionSharedCooldownComponent target1 = (ActionSharedCooldownComponent) target;
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
    ActionSharedCooldownComponent target1 = (ActionSharedCooldownComponent) target;
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
  virtual ActionSharedCooldownComponent Component.Instantiate()
  {
    return new ActionSharedCooldownComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ActionSharedCooldownComponent_AutoState : IComponentState
  {
    public EntProtoId? Id;
    public HashSet<EntProtoId> Ids;
    public HashSet<EntProtoId> ActiveIds;
    public TimeSpan Cooldown;
    public bool OnPerform;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActionSharedCooldownComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ActionSharedCooldownComponent, ComponentGetState>(new ComponentEventRefHandler<ActionSharedCooldownComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ActionSharedCooldownComponent, ComponentHandleState>(new ComponentEventRefHandler<ActionSharedCooldownComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ActionSharedCooldownComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ActionSharedCooldownComponent.ActionSharedCooldownComponent_AutoState()
      {
        Id = component.Id,
        Ids = component.Ids,
        ActiveIds = component.ActiveIds,
        Cooldown = component.Cooldown,
        OnPerform = component.OnPerform
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ActionSharedCooldownComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ActionSharedCooldownComponent.ActionSharedCooldownComponent_AutoState current))
        return;
      component.Id = current.Id;
      component.Ids = current.Ids == null ? (HashSet<EntProtoId>) null : new HashSet<EntProtoId>((IEnumerable<EntProtoId>) current.Ids);
      component.ActiveIds = current.ActiveIds == null ? (HashSet<EntProtoId>) null : new HashSet<EntProtoId>((IEnumerable<EntProtoId>) current.ActiveIds);
      component.Cooldown = current.Cooldown;
      component.OnPerform = current.OnPerform;
    }
  }
}
