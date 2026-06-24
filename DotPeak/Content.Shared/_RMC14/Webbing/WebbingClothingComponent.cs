// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Webbing.WebbingClothingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Item;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Webbing;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedWebbingSystem)})]
public sealed class WebbingClothingComponent : 
  Component,
  ISerializationGenerated<WebbingClothingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Container = "cm_clothing_webbing_slot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Webbing;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ItemSizePrototype>? UnequippedSize;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<WebbingComponent>? StartingWebbing;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WebbingClothingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WebbingClothingComponent) target1;
    if (serialization.TryCustomCopy<WebbingClothingComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Container == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Container, ref target2, hookCtx, false, context))
      target2 = this.Container;
    target.Container = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Webbing, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Webbing, hookCtx, context);
    target.Webbing = target3;
    ProtoId<ItemSizePrototype>? target4 = new ProtoId<ItemSizePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<ItemSizePrototype>?>(this.UnequippedSize, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<ProtoId<ItemSizePrototype>?>(this.UnequippedSize, hookCtx, context);
    target.UnequippedSize = target4;
    EntProtoId<WebbingComponent>? target5 = new EntProtoId<WebbingComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<WebbingComponent>?>(this.StartingWebbing, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId<WebbingComponent>?>(this.StartingWebbing, hookCtx, context);
    target.StartingWebbing = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WebbingClothingComponent target,
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
    WebbingClothingComponent target1 = (WebbingClothingComponent) target;
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
    WebbingClothingComponent target1 = (WebbingClothingComponent) target;
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
    WebbingClothingComponent target1 = (WebbingClothingComponent) target;
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
  virtual WebbingClothingComponent Component.Instantiate() => new WebbingClothingComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class WebbingClothingComponent_AutoState : IComponentState
  {
    public string Container;
    public NetEntity? Webbing;
    public ProtoId<ItemSizePrototype>? UnequippedSize;
    public EntProtoId<WebbingComponent>? StartingWebbing;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class WebbingClothingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<WebbingClothingComponent, ComponentGetState>(new ComponentEventRefHandler<WebbingClothingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<WebbingClothingComponent, ComponentHandleState>(new ComponentEventRefHandler<WebbingClothingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      WebbingClothingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new WebbingClothingComponent.WebbingClothingComponent_AutoState()
      {
        Container = component.Container,
        Webbing = this.GetNetEntity(component.Webbing),
        UnequippedSize = component.UnequippedSize,
        StartingWebbing = component.StartingWebbing
      };
    }

    private void OnHandleState(
      EntityUid uid,
      WebbingClothingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is WebbingClothingComponent.WebbingClothingComponent_AutoState current))
        return;
      component.Container = current.Container;
      component.Webbing = this.EnsureEntity<WebbingClothingComponent>(current.Webbing, uid);
      component.UnequippedSize = current.UnequippedSize;
      component.StartingWebbing = current.StartingWebbing;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, WebbingClothingComponent>(uid, component, ref args1);
    }
  }
}
