// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.NightVision.NightVisionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.NightVision;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedNightVisionSystem)})]
public sealed class NightVisionComponent : 
  Component,
  ISerializationGenerated<NightVisionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype>? Alert;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public NightVisionState State = NightVisionState.Full;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Overlay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Innate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool SeeThroughContainers;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Green;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Mesons;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BlockScopes;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OnlyHalf;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NightVisionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NightVisionComponent) target1;
    if (serialization.TryCustomCopy<NightVisionComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<AlertPrototype>? target2 = new ProtoId<AlertPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>?>(this.Alert, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<AlertPrototype>?>(this.Alert, hookCtx, context);
    target.Alert = target2;
    NightVisionState target3 = NightVisionState.Off;
    if (!serialization.TryCustomCopy<NightVisionState>(this.State, ref target3, hookCtx, false, context))
      target3 = this.State;
    target.State = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Overlay, ref target4, hookCtx, false, context))
      target4 = this.Overlay;
    target.Overlay = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Innate, ref target5, hookCtx, false, context))
      target5 = this.Innate;
    target.Innate = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.SeeThroughContainers, ref target6, hookCtx, false, context))
      target6 = this.SeeThroughContainers;
    target.SeeThroughContainers = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Green, ref target7, hookCtx, false, context))
      target7 = this.Green;
    target.Green = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Mesons, ref target8, hookCtx, false, context))
      target8 = this.Mesons;
    target.Mesons = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockScopes, ref target9, hookCtx, false, context))
      target9 = this.BlockScopes;
    target.BlockScopes = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnlyHalf, ref target10, hookCtx, false, context))
      target10 = this.OnlyHalf;
    target.OnlyHalf = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NightVisionComponent target,
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
    NightVisionComponent target1 = (NightVisionComponent) target;
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
    NightVisionComponent target1 = (NightVisionComponent) target;
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
    NightVisionComponent target1 = (NightVisionComponent) target;
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
  virtual NightVisionComponent Component.Instantiate() => new NightVisionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NightVisionComponent_AutoState : IComponentState
  {
    public NightVisionState State;
    public bool Overlay;
    public bool Innate;
    public bool SeeThroughContainers;
    public bool Green;
    public bool Mesons;
    public bool BlockScopes;
    public bool OnlyHalf;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NightVisionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<NightVisionComponent, ComponentGetState>(new ComponentEventRefHandler<NightVisionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<NightVisionComponent, ComponentHandleState>(new ComponentEventRefHandler<NightVisionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      NightVisionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new NightVisionComponent.NightVisionComponent_AutoState()
      {
        State = component.State,
        Overlay = component.Overlay,
        Innate = component.Innate,
        SeeThroughContainers = component.SeeThroughContainers,
        Green = component.Green,
        Mesons = component.Mesons,
        BlockScopes = component.BlockScopes,
        OnlyHalf = component.OnlyHalf
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NightVisionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is NightVisionComponent.NightVisionComponent_AutoState current))
        return;
      component.State = current.State;
      component.Overlay = current.Overlay;
      component.Innate = current.Innate;
      component.SeeThroughContainers = current.SeeThroughContainers;
      component.Green = current.Green;
      component.Mesons = current.Mesons;
      component.BlockScopes = current.BlockScopes;
      component.OnlyHalf = current.OnlyHalf;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, NightVisionComponent>(uid, component, ref args1);
    }
  }
}
