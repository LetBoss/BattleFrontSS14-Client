// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Nest.XenoNestSurfaceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Weeds;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
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
namespace Content.Shared._RMC14.Xenonids.Construction.Nest;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoNestSystem)})]
public sealed class XenoNestSurfaceComponent : 
  Component,
  ISerializationGenerated<XenoNestSurfaceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId Nest = (EntProtoId) "XenoNest";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DoAfter = TimeSpan.FromSeconds(8L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<Direction, EntityUid> Nests = new Dictionary<Direction, EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (XenoNestSystem), typeof (SharedXenoWeedsSystem)})]
  public EntityUid? Weedable;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoNestSurfaceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoNestSurfaceComponent) target1;
    if (serialization.TryCustomCopy<XenoNestSurfaceComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Nest, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.Nest, hookCtx, context);
    target.Nest = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DoAfter, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.DoAfter, hookCtx, context);
    target.DoAfter = target3;
    Dictionary<Direction, EntityUid> target4 = (Dictionary<Direction, EntityUid>) null;
    if (this.Nests == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Direction, EntityUid>>(this.Nests, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<Dictionary<Direction, EntityUid>>(this.Nests, hookCtx, context);
    target.Nests = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Weedable, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Weedable, hookCtx, context);
    target.Weedable = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoNestSurfaceComponent target,
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
    XenoNestSurfaceComponent target1 = (XenoNestSurfaceComponent) target;
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
    XenoNestSurfaceComponent target1 = (XenoNestSurfaceComponent) target;
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
    XenoNestSurfaceComponent target1 = (XenoNestSurfaceComponent) target;
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
  virtual XenoNestSurfaceComponent Component.Instantiate() => new XenoNestSurfaceComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoNestSurfaceComponent_AutoState : IComponentState
  {
    public EntProtoId Nest;
    public TimeSpan DoAfter;
    public Dictionary<Direction, NetEntity> Nests;
    public NetEntity? Weedable;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoNestSurfaceComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoNestSurfaceComponent, ComponentGetState>(new ComponentEventRefHandler<XenoNestSurfaceComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoNestSurfaceComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoNestSurfaceComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoNestSurfaceComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoNestSurfaceComponent.XenoNestSurfaceComponent_AutoState()
      {
        Nest = component.Nest,
        DoAfter = component.DoAfter,
        Nests = this.GetNetEntityDictionary<Direction>(component.Nests),
        Weedable = this.GetNetEntity(component.Weedable)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoNestSurfaceComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoNestSurfaceComponent.XenoNestSurfaceComponent_AutoState current))
        return;
      component.Nest = current.Nest;
      component.DoAfter = current.DoAfter;
      this.EnsureEntityDictionary<XenoNestSurfaceComponent, Direction>(current.Nests, uid, component.Nests);
      component.Weedable = this.EnsureEntity<XenoNestSurfaceComponent>(current.Weedable, uid);
    }
  }
}
