// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Egg.XenoOvipositorCapableComponent
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
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Egg;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoEggSystem)})]
public sealed class XenoOvipositorCapableComponent : 
  Component,
  ISerializationGenerated<XenoOvipositorCapableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string AttachedState = "normal";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Spawn = (EntProtoId) "XenoEgg";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Offset = new Vector2(-1f, -1f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId[] ActionIds = new EntProtoId[3]
  {
    (EntProtoId) "ActionXenoLeader",
    (EntProtoId) "ActionXenoHeal",
    (EntProtoId) "ActionXenoQueenEye"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<EntProtoId, EntityUid> Actions = new Dictionary<EntProtoId, EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoOvipositorCapableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoOvipositorCapableComponent) target1;
    if (serialization.TryCustomCopy<XenoOvipositorCapableComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.AttachedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.AttachedState, ref target2, hookCtx, false, context))
      target2 = this.AttachedState;
    target.AttachedState = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target3;
    EntProtoId target4 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Spawn, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId>(this.Spawn, hookCtx, context);
    target.Spawn = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target5;
    EntProtoId[] target6 = (EntProtoId[]) null;
    if (this.ActionIds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<EntProtoId[]>(this.ActionIds, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<EntProtoId[]>(this.ActionIds, hookCtx, context);
    target.ActionIds = target6;
    Dictionary<EntProtoId, EntityUid> target7 = (Dictionary<EntProtoId, EntityUid>) null;
    if (this.Actions == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntProtoId, EntityUid>>(this.Actions, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<Dictionary<EntProtoId, EntityUid>>(this.Actions, hookCtx, context);
    target.Actions = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoOvipositorCapableComponent target,
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
    XenoOvipositorCapableComponent target1 = (XenoOvipositorCapableComponent) target;
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
    XenoOvipositorCapableComponent target1 = (XenoOvipositorCapableComponent) target;
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
    XenoOvipositorCapableComponent target1 = (XenoOvipositorCapableComponent) target;
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
  virtual XenoOvipositorCapableComponent Component.Instantiate()
  {
    return new XenoOvipositorCapableComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoOvipositorCapableComponent_AutoState : IComponentState
  {
    public string AttachedState;
    public TimeSpan Cooldown;
    public EntProtoId Spawn;
    public Vector2 Offset;
    public EntProtoId[] ActionIds;
    public Dictionary<EntProtoId, NetEntity> Actions;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoOvipositorCapableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoOvipositorCapableComponent, ComponentGetState>(new ComponentEventRefHandler<XenoOvipositorCapableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoOvipositorCapableComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoOvipositorCapableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoOvipositorCapableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoOvipositorCapableComponent.XenoOvipositorCapableComponent_AutoState()
      {
        AttachedState = component.AttachedState,
        Cooldown = component.Cooldown,
        Spawn = component.Spawn,
        Offset = component.Offset,
        ActionIds = component.ActionIds,
        Actions = this.GetNetEntityDictionary<EntProtoId>(component.Actions)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoOvipositorCapableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoOvipositorCapableComponent.XenoOvipositorCapableComponent_AutoState current))
        return;
      component.AttachedState = current.AttachedState;
      component.Cooldown = current.Cooldown;
      component.Spawn = current.Spawn;
      component.Offset = current.Offset;
      component.ActionIds = current.ActionIds;
      this.EnsureEntityDictionary<XenoOvipositorCapableComponent, EntProtoId>(current.Actions, uid, component.Actions);
    }
  }
}
