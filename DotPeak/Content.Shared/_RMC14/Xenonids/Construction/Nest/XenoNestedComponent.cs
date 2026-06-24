// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.Nest.XenoNestedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.Nest;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (XenoNestSystem)})]
public sealed class XenoNestedComponent : 
  Component,
  ISerializationGenerated<XenoNestedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Nest;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Detached;
  [DataField(null, false, 1, false, false, null)]
  public float IncubationMultiplier = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NetUserId? GhostedId;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref XenoNestedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (XenoNestedComponent) target1;
    if (serialization.TryCustomCopy<XenoNestedComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid target2 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Nest, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid>(this.Nest, hookCtx, context);
    target.Nest = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Detached, ref target3, hookCtx, false, context))
      target3 = this.Detached;
    target.Detached = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IncubationMultiplier, ref target4, hookCtx, false, context))
      target4 = this.IncubationMultiplier;
    target.IncubationMultiplier = target4;
    NetUserId? target5 = new NetUserId?();
    if (!serialization.TryCustomCopy<NetUserId?>(this.GhostedId, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<NetUserId?>(this.GhostedId, hookCtx, context);
    target.GhostedId = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref XenoNestedComponent target,
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
    XenoNestedComponent target1 = (XenoNestedComponent) target;
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
    XenoNestedComponent target1 = (XenoNestedComponent) target;
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
    XenoNestedComponent target1 = (XenoNestedComponent) target;
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
  virtual XenoNestedComponent Component.Instantiate() => new XenoNestedComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class XenoNestedComponent_AutoState : IComponentState
  {
    public NetEntity Nest;
    public bool Detached;
    public NetUserId? GhostedId;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XenoNestedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<XenoNestedComponent, ComponentGetState>(new ComponentEventRefHandler<XenoNestedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<XenoNestedComponent, ComponentHandleState>(new ComponentEventRefHandler<XenoNestedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      XenoNestedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new XenoNestedComponent.XenoNestedComponent_AutoState()
      {
        Nest = this.GetNetEntity(component.Nest),
        Detached = component.Detached,
        GhostedId = component.GhostedId
      };
    }

    private void OnHandleState(
      EntityUid uid,
      XenoNestedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is XenoNestedComponent.XenoNestedComponent_AutoState current))
        return;
      component.Nest = this.EnsureEntity<XenoNestedComponent>(current.Nest, uid);
      component.Detached = current.Detached;
      component.GhostedId = current.GhostedId;
    }
  }
}
