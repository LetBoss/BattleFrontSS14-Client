// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.Components.BorgSwitchableTypeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radio;
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
namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedBorgSwitchableTypeSystem)})]
public sealed class BorgSwitchableTypeComponent : 
  Component,
  ISerializationGenerated<BorgSwitchableTypeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? SelectTypeAction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<BorgTypePrototype>? SelectedBorgType;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype>[] InherentRadioChannels = Array.Empty<ProtoId<RadioChannelPrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BorgSwitchableTypeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (BorgSwitchableTypeComponent) target1;
    if (serialization.TryCustomCopy<BorgSwitchableTypeComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.SelectTypeAction, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.SelectTypeAction, hookCtx, context);
    target.SelectTypeAction = target2;
    ProtoId<BorgTypePrototype>? target3 = new ProtoId<BorgTypePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<BorgTypePrototype>?>(this.SelectedBorgType, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<BorgTypePrototype>?>(this.SelectedBorgType, hookCtx, context);
    target.SelectedBorgType = target3;
    ProtoId<RadioChannelPrototype>[] target4 = (ProtoId<RadioChannelPrototype>[]) null;
    if (this.InherentRadioChannels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<RadioChannelPrototype>[]>(this.InherentRadioChannels, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<ProtoId<RadioChannelPrototype>[]>(this.InherentRadioChannels, hookCtx, context);
    target.InherentRadioChannels = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BorgSwitchableTypeComponent target,
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
    BorgSwitchableTypeComponent target1 = (BorgSwitchableTypeComponent) target;
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
    BorgSwitchableTypeComponent target1 = (BorgSwitchableTypeComponent) target;
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
    BorgSwitchableTypeComponent target1 = (BorgSwitchableTypeComponent) target;
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
  virtual BorgSwitchableTypeComponent Component.Instantiate() => new BorgSwitchableTypeComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class BorgSwitchableTypeComponent_AutoState : IComponentState
  {
    public NetEntity? SelectTypeAction;
    public ProtoId<BorgTypePrototype>? SelectedBorgType;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class BorgSwitchableTypeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<BorgSwitchableTypeComponent, ComponentGetState>(new ComponentEventRefHandler<BorgSwitchableTypeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<BorgSwitchableTypeComponent, ComponentHandleState>(new ComponentEventRefHandler<BorgSwitchableTypeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      BorgSwitchableTypeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new BorgSwitchableTypeComponent.BorgSwitchableTypeComponent_AutoState()
      {
        SelectTypeAction = this.GetNetEntity(component.SelectTypeAction),
        SelectedBorgType = component.SelectedBorgType
      };
    }

    private void OnHandleState(
      EntityUid uid,
      BorgSwitchableTypeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is BorgSwitchableTypeComponent.BorgSwitchableTypeComponent_AutoState current))
        return;
      component.SelectTypeAction = this.EnsureEntity<BorgSwitchableTypeComponent>(current.SelectTypeAction, uid);
      component.SelectedBorgType = current.SelectedBorgType;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, BorgSwitchableTypeComponent>(uid, component, ref args1);
    }
  }
}
