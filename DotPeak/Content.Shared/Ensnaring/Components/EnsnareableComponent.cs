// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ensnaring.Components.EnsnareableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
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
namespace Content.Shared.Ensnaring.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
public sealed class EnsnareableComponent : 
  Component,
  ISerializationGenerated<EnsnareableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WalkSpeed = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SprintSpeed = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsEnsnared;
  public Container Container;
  [DataField(null, false, 1, false, false, null)]
  public string? Sprite;
  [DataField(null, false, 1, false, false, null)]
  public string? State;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> EnsnaredAlert = (ProtoId<AlertPrototype>) "Ensnared";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EnsnareableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EnsnareableComponent) target1;
    if (serialization.TryCustomCopy<EnsnareableComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkSpeed, ref target2, hookCtx, false, context))
      target2 = this.WalkSpeed;
    target.WalkSpeed = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintSpeed, ref target3, hookCtx, false, context))
      target3 = this.SprintSpeed;
    target.SprintSpeed = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsEnsnared, ref target4, hookCtx, false, context))
      target4 = this.IsEnsnared;
    target.IsEnsnared = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Sprite, ref target5, hookCtx, false, context))
      target5 = this.Sprite;
    target.Sprite = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.State, ref target6, hookCtx, false, context))
      target6 = this.State;
    target.State = target6;
    ProtoId<AlertPrototype> target7 = new ProtoId<AlertPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(this.EnsnaredAlert, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<AlertPrototype>>(this.EnsnaredAlert, hookCtx, context);
    target.EnsnaredAlert = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EnsnareableComponent target,
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
    EnsnareableComponent target1 = (EnsnareableComponent) target;
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
    EnsnareableComponent target1 = (EnsnareableComponent) target;
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
    EnsnareableComponent target1 = (EnsnareableComponent) target;
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
  virtual EnsnareableComponent Component.Instantiate() => new EnsnareableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class EnsnareableComponent_AutoState : IComponentState
  {
    public float WalkSpeed;
    public float SprintSpeed;
    public bool IsEnsnared;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EnsnareableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EnsnareableComponent, ComponentGetState>(new ComponentEventRefHandler<EnsnareableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<EnsnareableComponent, ComponentHandleState>(new ComponentEventRefHandler<EnsnareableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      EnsnareableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new EnsnareableComponent.EnsnareableComponent_AutoState()
      {
        WalkSpeed = component.WalkSpeed,
        SprintSpeed = component.SprintSpeed,
        IsEnsnared = component.IsEnsnared
      };
    }

    private void OnHandleState(
      EntityUid uid,
      EnsnareableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is EnsnareableComponent.EnsnareableComponent_AutoState current))
        return;
      component.WalkSpeed = current.WalkSpeed;
      component.SprintSpeed = current.SprintSpeed;
      component.IsEnsnared = current.IsEnsnared;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, EnsnareableComponent>(uid, component, ref args1);
    }
  }
}
