// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hook.XenoHookedComponent
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
namespace Content.Shared._RMC14.Xenonids.Hook;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class XenoHookedComponent : 
  Component,
  ISerializationGenerated<XenoHookedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Source;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId TailProto;
  [DataField(null, false, 1, false, false, null)]
  public List<EntityUid> Tail = new List<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  public bool StopUpdating;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoHookedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoHookedComponent) target1;
    if (serialization.TryCustomCopy<XenoHookedComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Source, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Source, hookCtx, context);
    target.Source = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.TailProto, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.TailProto, hookCtx, context);
    target.TailProto = target3;
    List<EntityUid> target4 = (List<EntityUid>) null;
    if (this.Tail == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntityUid>>(this.Tail, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntityUid>>(this.Tail, hookCtx, context);
    target.Tail = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.StopUpdating, ref target5, hookCtx, false, context))
      target5 = this.StopUpdating;
    target.StopUpdating = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoHookedComponent target,
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
    XenoHookedComponent target1 = (XenoHookedComponent) target;
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
    XenoHookedComponent target1 = (XenoHookedComponent) target;
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
    XenoHookedComponent target1 = (XenoHookedComponent) target;
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
  virtual XenoHookedComponent Component.Instantiate() => new XenoHookedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoHookedComponent_AutoState : IComponentState
  {
    public NetEntity Source;
    public EntProtoId TailProto;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoHookedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoHookedComponent, ComponentGetState>(new ComponentEventRefHandler<XenoHookedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoHookedComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoHookedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoHookedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoHookedComponent.XenoHookedComponent_AutoState()
      {
        Source = this.GetNetEntity(component.Source),
        TailProto = component.TailProto
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoHookedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoHookedComponent.XenoHookedComponent_AutoState current))
        return;
      component.Source = this.EnsureEntity<XenoHookedComponent>(current.Source, uid);
      component.TailProto = current.TailProto;
    }
  }
}
